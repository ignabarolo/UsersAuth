using Microsoft.AspNetCore.Identity;

namespace UsersAuth.Common;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    // Mensaje para Password.RequiredLength = 8
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"La contraseña debe tener al menos {length} caracteres."
        };
    }

    // Mensaje para Password.RequireNonAlphanumeric = false
    // (Aunque lo tienes en false, si lo cambias a true en el futuro)
    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "La contraseña requiere al menos un carácter no alfanumérico (ej: #, $, !)."
        };
    }

    // Mensaje para Password.RequireDigit = true
    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "La contraseña requiere al menos un dígito ('0'-'9')."
        };
    }

    // Mensaje para Password.RequireLowercase = true
    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "La contraseña requiere al menos una letra minúscula ('a'-'z')."
        };
    }

    // Mensaje para Password.RequireUppercase = true
    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "La contraseña requiere al menos una letra mayúscula ('A'-'Z')."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"El usuario '{userName}' ya está registrado."
        };
    }

    // Traducción para el error "DuplicateEmail" (si es que lo tienes separado)
    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"El correo '{email}' ya está registrado."
        };
    }
}
