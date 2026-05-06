using System.ComponentModel.DataAnnotations;

namespace UsersAuth.Pages.Account;

public class RegisterDto
{
    public string? UserName { get; set; } = null;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string Password2 { get; set; } = string.Empty;
}
