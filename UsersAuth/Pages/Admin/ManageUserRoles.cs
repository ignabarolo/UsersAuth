using Microsoft.AspNetCore.Mvc;

namespace UsersAuth.Pages.Admin;

public class ManageUserRolesViewModel
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    [BindProperty]
    public List<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}
