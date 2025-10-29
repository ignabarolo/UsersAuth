using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;
using UsersAuth.Identity;

namespace UsersAuth.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Rol> _roleManager;
    private readonly IEmailSender _emailSender;

    public RegisterModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<RegisterModel> logger,
        RoleManager<Rol> roleManager,
        IEmailSender emailSender
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _roleManager = roleManager;
    }

    [BindProperty]
    public RegisterDto Register { get; set; }

    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (ModelState.IsValid)
        {
            var usernameToUse = string.IsNullOrWhiteSpace(Register.UserName) ? Register.Email : Register.UserName;
            var user = new User { UserName = usernameToUse, Email = Register.Email };
            var result = await _userManager.CreateAsync(user, Register.Password);
            if (result.Succeeded)
            {
                ;
                var addResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error al añadir roles.");
                    return Page();
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, code },
                    protocol: Request.Scheme);

                if (callbackUrl == null)
                {
                    ModelState.AddModelError(string.Empty, "Error interno al crear enlace de confirmación.");
                    return Page();
                }

                await _emailSender.SendEmailAsync(Register.Email, "Confirma tu email",
                    $"Por favor confirma tu email <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clickeando aquí</a>.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}
