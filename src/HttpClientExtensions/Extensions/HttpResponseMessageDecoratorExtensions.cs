using System;
using System.Threading.Tasks;
using HttpClientExtensions.Models;
using OneOf;

namespace HttpClientExtensions.Extensions;

public static class HttpResponseMessageDecoratorExtensions
{
    public static async Task<OneOf<HttpResponseResult, HttpResponseError>> OnSuccess(this Task<OneOf<HttpResponseResult, HttpResponseError>> response, Action<HttpResponseResult> onSuccess)
    {
        var responseResult = await response;
        if (responseResult.TryPickT0(out HttpResponseResult result, out _))
        {
            onSuccess(result);
        }

        return responseResult;
    }

    public static async Task<OneOf<HttpResponseResult, HttpResponseError>> OnFailure(this Task<OneOf<HttpResponseResult, HttpResponseError>> response, Action<HttpResponseError> onError)
    {
        var responseResult = await response;
        if (responseResult.TryPickT1(out HttpResponseError error, out _))
        {
            onError(error);
        }

        return responseResult;
    }
}
