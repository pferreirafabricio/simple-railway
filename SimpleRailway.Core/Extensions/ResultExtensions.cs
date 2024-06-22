using SimpleRailway.Core.Domain;

namespace SimpleRailway.Core.Extensions;

public static class ResultExtensions
{
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
        => result.IsSuccess
            ? bind(result.Value)
            : Result.Failure<TOut>(result.Error);

    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bind)
        => result.IsSuccess
            ? await bind(result.Value)
            : Result.Failure<TOut>(result.Error);

    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Result<TOut>> bind)
    {
        var t = await result;
        return t.IsSuccess
            ? bind(t.Value)
            : Result.Failure<TOut>(t.Error);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Task<Result<TOut>>> bind)
    {
        var t = await result;
        return t.IsSuccess
            ? await bind(t.Value)
            : Result.Failure<TOut>(t.Error);
    }

    public static Result<TOut> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, Error error)
    {
        try
        {
            return result.IsSuccess
                ? Result.Success(func(result.Value))
                : Result.Failure<TOut>(result.Error);
        }
        catch
        {
            return Result.Failure<TOut>(error);
        }
    }

    public static async Task<Result<TOut>> TryCatch<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, TOut> func, Error error)
        => (await result).TryCatch(func, error);

    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    public static async Task<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> result, Action<TIn> action)
        => (await result).Tap(action);

    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> success, Func<Error, TOut> failure)
        => result.IsSuccess
            ? success(result.Value)
            : failure(result.Error);

    public static async Task<TOut> Match<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, TOut> success, Func<Error, TOut> failure)
       => (await result).Match(success, failure);
}
