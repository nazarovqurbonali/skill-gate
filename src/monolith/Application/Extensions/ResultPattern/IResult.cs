namespace Application.Extensions.ResultPattern;

public interface IResult
{
    bool IsSuccess { get; }
    int? Code { get; }
    string? Message { get; }
    ErrorType ErrorType { get; }
    IReadOnlyList<string> Errors { get; }
}