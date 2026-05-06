using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UsersAuth.Identity;

namespace UsersAuth.Pages.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    public class ManageUserRolesModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Rol> _roleManager;

        public ManageUserRolesModel(UserManager<User> userManager, RoleManager<Rol> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public ManageUserRolesViewModel ViewModel { get; set; } = new ManageUserRolesViewModel();

        [TempData]
        public bool IsValid { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("A user ID must be provided.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"User with ID '{userId}' was not found.");
            }

            ViewModel.UserId = user.Id;
            ViewModel.UserEmail = user.Email ?? "Email not available";

            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            ViewModel.RoleAssignments = allRoles.Select(roleName => new RoleAssignment
            {
                RoleName = roleName,
                IsAssigned = userRoles.Contains(roleName)
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            if (!ModelState.IsValid || ViewModel.RoleAssignments == null)
            {
                return await OnGetAsync(userId);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"User with ID '{userId}' was not found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToUpdate = ViewModel.RoleAssignments;

            var rolesToAdd = rolesToUpdate
                .Where(r => r.IsAssigned && !currentRoles.Contains(r.RoleName))
                .Select(r => r.RoleName)
                .ToList();

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error adding roles.");
                    return await OnGetAsync(userId);
                }
                IsValid = true;
                StatusMessage = $"Roles for user '{user.Email}' updated successfully.";
            }

            var rolesToRemove = rolesToUpdate
                .Where(r => !r.IsAssigned && currentRoles.Contains(r.RoleName))
                .Select(r => r.RoleName)
                .ToList();

            if (rolesToRemove.Any())
            {
                if (currentRoles.Count <= 1)
                {
                    IsValid = false;
                    StatusMessage = $"User '{user.Email}' must have at least 1 role.";
                }
                else
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error removing roles.");
                        return await OnGetAsync(userId);
                    }

                    IsValid = true;
                    StatusMessage = $"Roles for user '{user.Email}' updated successfully.";
                }
            }

            return RedirectToPage("./Roles");
        }
    }
}