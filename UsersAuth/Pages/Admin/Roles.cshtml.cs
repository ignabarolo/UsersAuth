using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UsersAuth.Identity;

namespace UsersAuth.Pages.Admin;

[Authorize(Policy = "AdminPolicy")]
public class RolesModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Rol> _roleManager;

    [TempData]
    public bool IsValid { get; set; }

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public IList<UserRoleViewModel> UsersWithRoles { get; set; } = new List<UserRoleViewModel>();

    public RolesModel(UserManager<User> userManager, RoleManager<Rol> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        UsersWithRoles = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = userRoles.ToList()
            };
            
            // listar los roles para el formulario de gestion de roles
            model.AvailableRoles = allRoles.Select(role => new RoleAssignment
            {
                RoleName = role!,
                IsAssigned = userRoles.Contains(role!)
            }).ToList();

            UsersWithRoles.Add(model);
        }
    }
}