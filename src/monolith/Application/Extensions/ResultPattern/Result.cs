namespace Application.Extensions.ResultPattern;

public sealed class Result<T> : BaseResult
{
    public T? Value { get; }

    private Result(bool isSuccess, int? code, string? message, ErrorType errorType, T? value, List<string>? errors = null)
        : base(isSuccess, code, message, errorType, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string message = "Success") =>
        new(true, 200, message, ErrorType.None, value);

    public new static Result<T> Failure(ResultPatternError error) =>
        new(false, error.Code, error.Message, error.ErrorType, default, error.Details);
}