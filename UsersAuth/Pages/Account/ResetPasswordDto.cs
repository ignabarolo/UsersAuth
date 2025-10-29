using System.ComponentModel.DataAnnotations;

namespace UsersAuth.Pages.Account;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "Por favor introduzca un email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña de confirmacion es requerida.")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty; 

    public string Code { get; set; } = string.Empty;
}
