namespace Application.Extensions.DTOs.Identity.Requests;

public readonly record struct LoginRequest(
    string Login,
    [Required, MinLength(8), MaxLength(128)]
    string Password);