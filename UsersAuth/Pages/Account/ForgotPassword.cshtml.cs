using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using UsersAuth.Identity;

namespace UsersAuth.Pages.Account;


[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordModel(UserManager<User> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);

            await UserValidations(user);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { code, email = Input.Email },
                protocol: Request.Scheme);

            if (callbackUrl == null)
            {
                ModelState.AddModelError(string.Empty, "Internal error creating password reset link.");
                return Page();
            }

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Reset Password",
                $"Please reset your password <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>by clicking here</a>.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        return Page();
    }

    private async Task UserValidations(User user)
    {
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError(string.Empty, "You must confirm your email before resetting your password.");
            return;
        }
    }
}
