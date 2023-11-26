using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HttpClientExtensions.Models;
using OneOf;

namespace HttpClientExtensions.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<OneOf<HttpResponseResult, HttpResponseError>> SendAsync(
        this HttpRequestBuilder builder,
        HttpMethod method,
        CancellationToken? cancellationToken = null)
    {
        return await builder.SendAsync(method, cancellationToken).ConfigureAwait(false);
    }

    public static Task<OneOf<HttpResponseResult, HttpResponseError>> GetAsync(
        this HttpRequestBuilder builder,
        CancellationToken? cancellationToken = null) =>
        SendAsync(builder, HttpMethod.Get, cancellationToken);

    public static Task<OneOf<HttpResponseResult, HttpResponseError>> PostAsync(
        this HttpRequestBuilder builder,
        CancellationToken? cancellationToken = null) =>
        SendAsync(builder, HttpMethod.Post, cancellationToken);

    public static Task<OneOf<HttpResponseResult, HttpResponseError>> PutAsync(
        this HttpRequestBuilder builder,
        CancellationToken? cancellationToken = null) =>
        SendAsync(builder, HttpMethod.Put, cancellationToken);

    public static Task<OneOf<HttpResponseResult, HttpResponseError>> DeleteAsync(
        this HttpRequestBuilder builder,
        CancellationToken? cancellationToken = null) =>
        SendAsync(builder, HttpMethod.Delete, cancellationToken);
}
