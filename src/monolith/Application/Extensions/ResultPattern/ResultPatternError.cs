namespace Application.Extensions.ResultPattern;

public sealed record ResultPatternError
{
    public int Code { get; }
    public string Message { get; }
    public ErrorType ErrorType { get; }
    public List<string> Details { get; } = [];

    private ResultPatternError(int code, string message, ErrorType errorType, List<string>? details = null)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
        Details = details ?? new List<string>();
    }

    public static ResultPatternError None(string message = "Ok") =>
        new(200, message, ErrorType.None);

    public static ResultPatternError NotFound(string message = "Data not found!") =>
        new(404, message, ErrorType.NotFound);

    public static ResultPatternError BadRequest(string message = "Bad request!", params string[] details) =>
        new(400, message, ErrorType.BadRequest, details.ToList());

    public static ResultPatternError AlreadyExist(string message = "Already exist!", params string[] details) =>
        new(409, message, ErrorType.AlreadyExist, details.ToList());

    public static ResultPatternError Conflict(string message = "Conflict!", params string[] details) =>
        new(409, message, ErrorType.Conflict, details.ToList());

    public static ResultPatternError InternalServerError(string message = "Internal server error!", params string[] details) =>
        new(500, message, ErrorType.InternalServerError, details.ToList());

    public static ResultPatternError Unauthorized(string message = "Unauthorized!", params string[] details) =>
        new(401, message, ErrorType.Unauthorized, details.ToList());

    public static ResultPatternError Forbidden(string message = "Forbidden!", params string[] details) =>
        new(403, message, ErrorType.Forbidden, details.ToList());

    public static ResultPatternError ValidationError(string message = "Validation error!", params string[] details) =>
        new(422, message, ErrorType.ValidationError, details.ToList());

    public static ResultPatternError Timeout(string message = "Request timed out!", params string[] details) =>
        new(408, message, ErrorType.Timeout, details.ToList());

    public static ResultPatternError ServiceUnavailable(string message = "Service unavailable!", params string[] details) =>
        new(503, message, ErrorType.ServiceUnavailable, details.ToList());

    public static ResultPatternError Custom(int code, string message, params string[] details) =>
        new(code, message, ErrorType.Custom, details.ToList());
}
