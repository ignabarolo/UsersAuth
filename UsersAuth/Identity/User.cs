using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UsersAuth.Identity;

public class User : IdentityUser<Guid>
{
    [Required(ErrorMessage = "Username is required.")]
    [Display(Name = "Username")]
    [StringLength(20, ErrorMessage = "The {0} must be fewer than {1} characters.")]
    [RegularExpression("^[a-zA-Z0-9_]{5,20}$", ErrorMessage = "Username must be 5-20 characters, no spaces or special symbols allowed.")]
    public override string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [Display(Name = "Email")]
    [StringLength(50, ErrorMessage = "The {0} must be fewer than {1} characters.")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
    public override string Email { get; set; } = string.Empty;
}
