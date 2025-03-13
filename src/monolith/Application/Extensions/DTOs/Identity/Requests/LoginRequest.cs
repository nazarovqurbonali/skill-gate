namespace Application.Extensions.DTOs.Identity.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Login is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MinLength(3, ErrorMessage = "Login must be at least 3 characters long.")]
    [MaxLength(128, ErrorMessage = "Login must not exceed 128 characters.")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128, ErrorMessage = "Password must not exceed 128 characters.")]
    public string Password { get; set; } = string.Empty;
}