using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using HttpClientExtensions.Models;
using Newtonsoft.Json;

namespace HttpClientExtensions.Extensions;

public static class HttpRequestBuilderExtensions
{
    public static HttpRequestBuilder CreateRequest(this HttpClient httpClient) => new (httpClient);

    public static HttpRequestBuilder SetUrl(this HttpRequestBuilder builder, string url)
    {
        builder.SetUrl(url);

        return builder;
    }

    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string username, string password)
    {
        builder.SetBasicAuthentication(username, password);

        return builder;
    }

    public static HttpRequestBuilder WithBearerToken(this HttpRequestBuilder builder, string token)
    {
        builder.SetBearerAuthentication(token);

        return builder;
    }

    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, string? value)
    {
        builder.SetHeaderValue(key, value);

        return builder;
    }

    public static HttpRequestBuilder WithHeaders(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string?>>? headers)
    {
        builder.SetHeaders(headers);

        return builder;
    }

    public static HttpRequestBuilder WithQueryParam(this HttpRequestBuilder builder, string key, string value)
    {
        builder.SetQueryParamValue(key, value);

        return builder;
    }

    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string?>>? queryParams)
    {
        builder.SetQueryParams(queryParams);

        return builder;
    }

    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, HttpContent content)
    {
        builder.SetContent(content);

        return builder;
    }

    public static HttpRequestBuilder WithJsonContent(this HttpRequestBuilder builder, object? content)
    {
        var json = content == null ? null : JsonConvert.SerializeObject(content);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        return builder.WithContent(httpContent);
    }

    public static HttpRequestBuilder WithFormUrlEncodedContent(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>>? content)
    {
        if (content.IsNullOrEmpty())
        {
            return builder;
        }

        var httpContent = new FormUrlEncodedContent(content!);

        return builder.WithContent(httpContent);
    }

    public static HttpRequestBuilder WithTimeout(this HttpRequestBuilder builder, TimeSpan timeout)
    {
        builder.SetTimeout(timeout);

        return builder;
    }
}
