using System;
using HttpClientExtensions.Models;

namespace HttpClientExtensions.Extensions;

public static class OptionExtensions
{
    public static Option<TResult> Map<TSource, TResult>(this Option<TSource> source, Func<TSource, TResult> mapResult)
    {
        return source.TryPickT0(out var value, out _)
            ? Option<TResult>.Some(mapResult(value))
            : Option<TResult>.None;
    }

    public static TResult ExtractOrException<TResult>(this Option<TResult> source)
    {
        return source.TryPickT0(out var value, out _)
            ? value
            : throw new ArgumentNullException(nameof(source));
    }

    public static TResult? ExtractOrDefault<TResult>(this Option<TResult> source)
    {
        return source.TryPickT0(out var value, out _) ? value : default;
    }
}
