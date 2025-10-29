using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UsersAuth.Identity;

public class User : IdentityUser<Guid>
{
    [Required(ErrorMessage = "Debe ingresar el nombre de usuario.")]
    [Display(Name = "Usuario")]
    [StringLength(20, ErrorMessage = "El {0} debe ser menor a {1} caracteres.")]
    [RegularExpression("^[a-zA-Z0-9_]{5,20}$", ErrorMessage = "el nombre de usuario debe tener entre 5 y 20 caracteres, y no permite espacios o simbolos especiales")]
    public override string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar la casilla de email.")]
    [Display(Name = "Email")]
    [StringLength(50, ErrorMessage = "El {0} debe ser menor a {1} caracteres.")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail no es valido")]
    public override string Email { get; set; } = string.Empty;
}
