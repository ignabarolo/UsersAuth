using Microsoft.AspNetCore.Mvc;

namespace UsersAuth.Pages.Admin;

public class ManageUserRolesViewModel
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    // Lista de todos los roles disponibles con su estado actual para el usuario
    [BindProperty]
    public List<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}
