using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace HttpClientExtensions.Models;

public class HttpResponseError
{
    public static HttpResponseError CreateStatusCodeError(HttpStatusCode httpStatusCode, string? errorMessage) =>
        new (false, null, httpStatusCode, errorMessage);

    public static HttpResponseError CreateParsingError(Exception? exception) =>
        new(true, exception, null, null);

    public static HttpResponseError CreateRequestExceptionError(Exception exception) =>
        new (false, exception, null, null);

    private HttpResponseError(bool isParsingError, Exception? exception, HttpStatusCode? httpStatusCode, string? errorMessage)
    {
        IsParsingError = isParsingError;
        ErrorMessage = errorMessage;
        HttpStatusCode = httpStatusCode;
        Exception = exception;

        if (IsParsingError)
        {
            return;
        }

        switch (Exception)
        {
            case OperationCanceledException:
                IsCancelled = true;
                break;
            case HttpRequestException:
            case SocketException:
                IsSocketError = true;
                break;
            default:
            {
                if (HttpStatusCode != null)
                {
                    IsStatusCodeError = true;
                }

                break;
            }
        }
    }

    public bool IsStatusCodeError { get; }

    public bool IsCancelled { get; }

    public bool IsSocketError { get; }

    public bool IsParsingError { get; }

    public string? ErrorMessage { get; }

    public HttpStatusCode? HttpStatusCode { get; }

    public Exception? Exception { get; }
}
