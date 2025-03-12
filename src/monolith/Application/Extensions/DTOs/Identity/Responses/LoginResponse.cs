namespace Application.Extensions.DTOs.Identity.Responses;

public readonly record struct LoginResponse(
    DateTimeOffset StartTime,
    DateTimeOffset ExpiresAt
);