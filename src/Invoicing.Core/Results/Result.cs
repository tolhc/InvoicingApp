namespace Invoicing.Core.Results;

// DISCLAIMER: This is partially copied and thinned down, quickly and dirtily from https://github.com/vkhorikov/CSharpFunctionalExtensions
// The thought process behind it is that, I didn't want to introduce a library which maybe is not known by the evaluator of the task
// In a real world scenario we would have a talk about using it ;)

public readonly struct Result<T, TError>
{
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    private readonly TError _error;
    public TError Error => IsFailure ? _error : throw new Exception("Result is Success, so no Error exists");

    private readonly T _value;
    public T Value => IsSuccess ? _value : throw new Exception("Result is Failure, so no Value exists");

    private Result(bool isFailure, T value, TError error)
    {
        IsFailure = isFailure;
        _error = error;
        _value = value;
    }

    public static Result<T, TError> Success(T value)
    {
        return new Result<T, TError>(false, value, default!);
    }

    public static Result<T, TError> Failure(TError error)
    {
        return new Result<T, TError>(true, default!, error);
    }


    public static implicit operator Result<T, TError>(T value)
    {
        if (value is Result<T, TError> result)
        {
            return result;
        }

        return Success(value);
    }

    public static implicit operator Result<T, TError>(TError error)
    {
        if (error is Result<T, TError> result)
        {
            return result;
        }

        return Failure(error);
    }
}
