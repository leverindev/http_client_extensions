using System;
using System.Threading.Tasks;
using HttpClientExtensions.Models;
using Newtonsoft.Json;
using OneOf;

namespace HttpClientExtensions.Extensions;

public static class JsonExtensions
{
    public static async Task<TResult?> DeserializeJsonResultOrDefaultAsync<TResult>(
        this Task<OneOf<HttpResponseResult, HttpResponseError>> response,
        bool allowEmptyContent = false)
    {

        var result = await response.DeserializeJsonResultAsync<TResult>(allowEmptyContent);
        return result.Match(
            option => option.ExtractOrDefault(),
            _ => default);
    }

    public static async Task<OneOf<Option<TResult>, HttpResponseError>> DeserializeJsonResultAsync<TResult>(
        this Task<OneOf<HttpResponseResult, HttpResponseError>> response,
        bool allowEmptyContent = false)
    {
        var result = await response;

        return result.Match(
            message => ParsingSuccess<TResult>(message.ContentMessage, allowEmptyContent).Match(
                OneOf<Option<TResult>, HttpResponseError>.FromT0,
                OneOf<Option<TResult>, HttpResponseError>.FromT1),
            OneOf<Option<TResult>, HttpResponseError>.FromT1);
    }

    public static async Task<OneOf<Option<TResult>, Option<TError>, HttpResponseError>> DeserializeJsonResultWithErrorAsync<TResult, TError>(
            this Task<OneOf<HttpResponseResult, HttpResponseError>> response,
            bool allowEmptyContent = false)
    {
        var result = await response;

        return result.Match(
            message => ParsingSuccess<TResult>(message.ContentMessage, allowEmptyContent).Match(
                OneOf<Option<TResult>, Option<TError>, HttpResponseError>.FromT0,
                OneOf<Option<TResult>, Option<TError>, HttpResponseError>.FromT2),
            error => error.IsStatusCodeError
                ? ParsingError<TError>(error.ErrorMessage, allowEmptyContent).Match(
                    OneOf<Option<TResult>, Option<TError>, HttpResponseError>.FromT1,
                    OneOf<Option<TResult>, Option<TError>, HttpResponseError>.FromT2)
                : OneOf<Option<TResult>, Option<TError>, HttpResponseError>.FromT2(error));
    }

    private static OneOf<Option<TResult>, HttpResponseError> ParsingSuccess<TResult>(string? message, bool allowEmptyContent)
    {
        if (message == null)
        {
            if (allowEmptyContent)
            {
                return OneOf<Option<TResult>, HttpResponseError>.FromT0(Option<TResult>.None);
            }

            return OneOf<Option<TResult>, HttpResponseError>.FromT1(
                HttpResponseError.CreateParsingError(new ArgumentNullException(nameof(message))));
        }

        try
        {
            var resultObj = JsonConvert.DeserializeObject<TResult>(message);
            if (resultObj == null)
            {
                if (allowEmptyContent)
                {
                    return OneOf<Option<TResult>, HttpResponseError>.FromT0(Option<TResult>.None);
                }

                return OneOf<Option<TResult>, HttpResponseError>.FromT1(
                    HttpResponseError.CreateParsingError(new ArgumentNullException(nameof(resultObj))));
            }

            return OneOf<Option<TResult>, HttpResponseError>.FromT0(Option<TResult>.Some(resultObj));
        }
        catch (Exception e)
        {
            return OneOf<Option<TResult>, HttpResponseError>.FromT1(
                HttpResponseError.CreateParsingError(e));
        }
    }

    private static OneOf<Option<TError>, HttpResponseError> ParsingError<TError>(string? message, bool allowEmptyContent)
    {
        if (message == null)
        {
            if (allowEmptyContent)
            {
                return OneOf<Option<TError>, HttpResponseError>.FromT0(Option<TError>.None);
            }

            return OneOf<Option<TError>, HttpResponseError>.FromT1(
                HttpResponseError.CreateParsingError(new ArgumentNullException(nameof(message))));
        }

        try
        {
            var resultObj = JsonConvert.DeserializeObject<TError>(message);
            if (resultObj == null)
            {
                if (allowEmptyContent)
                {
                    return OneOf<Option<TError>, HttpResponseError>.FromT0(Option<TError>.None);
                }

                return OneOf<Option<TError>, HttpResponseError>.FromT1(
                    HttpResponseError.CreateParsingError(new ArgumentNullException(nameof(resultObj))));
            }

            return OneOf<Option<TError>, HttpResponseError>.FromT0(Option<TError>.Some(resultObj));
        }
        catch (Exception e)
        {
            return OneOf<Option<TError>, HttpResponseError>.FromT1(HttpResponseError.CreateParsingError(e));
        }
    }
}
