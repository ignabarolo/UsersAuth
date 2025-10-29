using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UsersAuth.Identity; // Asumo 'User' está aquí

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
                return NotFound("Debe proporcionar un ID de usuario.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"No se encontró al usuario con ID '{userId}'.");
            }

            ViewModel.UserId = user.Id;
            ViewModel.UserEmail = user.Email ?? "Email no disponible";

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
                // Si la validación falla (lo cual es poco probable aquí), recarga la página
                return await OnGetAsync(userId);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"No se encontró al usuario con ID '{userId}'.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToUpdate = ViewModel.RoleAssignments;

            // --- Lógica para AÑADIR y REMOVER roles ---

            // Roles a añadir: aquellos marcados en el formulario que el usuario NO tiene
            var rolesToAdd = rolesToUpdate
                .Where(r => r.IsAssigned && !currentRoles.Contains(r.RoleName))
                .Select(r => r.RoleName)
                .ToList();

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error al añadir roles.");
                    return await OnGetAsync(userId);
                }
                IsValid = true;
                StatusMessage = $"Roles del usuario '{user.Email}' actualizados correctamente.";
            }

            // Roles a remover: aquellos NO marcados en el formulario que el usuario SÍ tiene
            var rolesToRemove = rolesToUpdate
                .Where(r => !r.IsAssigned && currentRoles.Contains(r.RoleName))
                .Select(r => r.RoleName)
                .ToList();

            if (rolesToRemove.Any())
            {
                if (currentRoles.Count <= 1)
                {
                    IsValid = false;
                    StatusMessage = $"El usuario '{user.Email}' debe tener al menos 1 rol.";
                }
                else
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error al remover roles.");
                        return await OnGetAsync(userId);
                    }

                    IsValid = true;
                    StatusMessage = $"Roles del usuario '{user.Email}' actualizados correctamente.";
                }
            }

            return RedirectToPage("./Roles");
        }
    }
}