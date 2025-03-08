namespace Application.Extensions.ResultPattern;

public class BaseResult : IResult
{
    public bool IsSuccess { get; }
    public int? Code { get; }
    public string? Message { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<string> Errors { get; }

    protected BaseResult(bool isSuccess, int? code, string? message, ErrorType errorType, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Code = code;
        Message = message;
        ErrorType = errorType;
        Errors = errors ?? new List<string>();
    }

    public static BaseResult Success(string message = "Ok") =>
        new(true, 200, message, ErrorType.None);

    public static BaseResult Failure(ResultPatternError error) =>
        new(false, error.Code, error.Message, error.ErrorType, error.Details);
}