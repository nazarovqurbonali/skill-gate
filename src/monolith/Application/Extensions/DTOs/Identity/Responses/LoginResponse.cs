namespace Application.DTOs.Identity.Responses;

public readonly record struct LoginResponse(
    string Token,
    DateTimeOffset StartTime,
    DateTimeOffset ExpiresAt
);