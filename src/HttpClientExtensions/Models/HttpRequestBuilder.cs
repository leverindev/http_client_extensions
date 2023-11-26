using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpClientExtensions.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using OneOf;

namespace HttpClientExtensions.Models;

public class HttpRequestBuilder
{
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, string?> _headers = new ();
    private readonly Dictionary<string, string?> _queryParams = new ();

    private string? _url;
    private HttpContent? _content;
    private string? _token;
    private string? _username;
    private string? _password;
    private TimeSpan? _timeout;

    internal HttpRequestBuilder(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    internal void SetUrl(string url)
    {
        _url = url;
    }

    internal void SetBasicAuthentication(string username, string password)
    {
        _token = null;
        _username = username;
        _password = password;
    }

    internal void SetBearerAuthentication(string token)
    {
        _username = null;
        _password = null;
        _token = token;
    }

    internal void SetHeaderValue(string key, string? value)
    {
        SetKeyValue(_headers, key, value);
    }

    internal void SetHeaders(IEnumerable<KeyValuePair<string, string?>>? headers)
    {
        SetKeyValueCollection(_headers, headers);
    }

    internal void SetQueryParamValue(string key, string? value)
    {
        SetKeyValue(_queryParams, key, value);
    }

    internal void SetQueryParams(IEnumerable<KeyValuePair<string, string?>>? queryParams)
    {
        SetKeyValueCollection(_queryParams, queryParams);
    }

    internal void SetContent(HttpContent content)
    {
        _content = content;
    }

    internal void SetTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
    }

    internal async Task<OneOf<HttpResponseResult, HttpResponseError>> SendAsync(
        HttpMethod method,
        CancellationToken? cancellationToken = null)
    {
        var path = _url ?? string.Empty;
        var url = string.IsNullOrWhiteSpace(_httpClient.BaseAddress?.ToString())
            ? path
            : _httpClient.BaseAddress!.ToString().BuildUri(path);

        if (!_queryParams.IsNullOrEmpty())
        {
            url = QueryHelpers.AddQueryString(url, _queryParams);
        }

        var request = new HttpRequestMessage(method, url) { Content = _content };

        foreach (var header in _headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        if (!string.IsNullOrWhiteSpace(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        if (!string.IsNullOrWhiteSpace(_username))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));
        }

        if (_timeout != null)
        {
            _httpClient.Timeout = _timeout.Value;
        }

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var responseMessage = await response.Content.ReadAsStringAsync();

                return OneOf<HttpResponseResult, HttpResponseError>.FromT0(new HttpResponseResult(response, responseMessage));
            }

            var errorMessage = await response.Content.ReadAsStringAsync();

            return OneOf<HttpResponseResult, HttpResponseError>.FromT1(HttpResponseError.CreateStatusCodeError(response.StatusCode, errorMessage));
        }
        catch (Exception e)
        {
            return OneOf<HttpResponseResult, HttpResponseError>.FromT1(HttpResponseError.CreateRequestExceptionError(e));
        }
    }

    private static void SetKeyValueCollection(Dictionary<string, string?> destination, IEnumerable<KeyValuePair<string, string?>>? source)
    {
        if (source is null)
        {
            return;
        }

        foreach (var item in source)
        {
            destination[item.Key] = item.Value;
        }
    }

    private static void SetKeyValue(Dictionary<string, string?> destination, string key, string? value)
    {
        destination[key] = value;
    }
}
