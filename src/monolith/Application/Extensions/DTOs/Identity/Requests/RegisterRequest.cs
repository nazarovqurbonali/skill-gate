namespace Application.Extensions.DTOs.Identity.Requests;

public class RegisterRequest
{
    [Required, EmailAddress] 
    public string EmailAddress { get; set; } = string.Empty;

    [Required, MinLength(4), MaxLength(128)]
    public string UserName { get; set; } = string.Empty;
    [Required,Phone]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password),
         ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}