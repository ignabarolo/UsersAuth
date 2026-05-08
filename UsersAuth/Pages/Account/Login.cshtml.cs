using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UsersAuth.Identity;
using UsersAuth.Services;

namespace UsersAuth.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public LoginModel(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ITokenService tokenService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    [BindProperty]
    public LoginDto Login { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }


    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }


    #region snippet
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(Login.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user exists with this email.");
                return Page();
            }

            var result = await _signInManager
                .PasswordSignInAsync(user.UserName, Login.Password, true, true);
            if (result.Succeeded)
            {
                var token = await _tokenService.GenerateTokenAsync(user);
                return new JsonResult(new { token, email = user.Email, userId = user.Id.ToString() });
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new
                {
                    ReturnUrl = returnUrl,
                });
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login failed.");
                return Page();
            }
        }

        return Page();
    }
    #endregion
}
