using System.ComponentModel.DataAnnotations;

namespace UsersAuth.Pages.Account;

public class LoginDto
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "Por favor introduzca un email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
