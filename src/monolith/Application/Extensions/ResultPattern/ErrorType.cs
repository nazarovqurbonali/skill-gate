namespace Application.Extensions.ResultPattern;

public enum ErrorType
{
    None,
    BadRequest,
    NotFound,
    AlreadyExist,
    Conflict,
    InternalServerError,
    Unauthorized,
    Forbidden,
    ValidationError,
    Timeout,
    ServiceUnavailable,
    Custom
}