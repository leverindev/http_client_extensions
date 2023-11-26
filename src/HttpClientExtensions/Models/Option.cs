using OneOf;
using OneOf.Types;

namespace HttpClientExtensions.Models;

public class Option<T> : OneOfBase<T, None>
{
    public static Option<T> Some(T value)
    {
        return new Option<T>(OneOf<T, None>.FromT0(value));
    }

    public static Option<T> None { get; } = new Option<T>(OneOf<T, None>.FromT1(new None()));

    private Option(OneOf<T, None> input)
        : base(input)
    {
    }
}
