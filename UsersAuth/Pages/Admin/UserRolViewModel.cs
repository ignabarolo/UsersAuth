namespace UsersAuth.Pages.Admin;

public class UserRoleViewModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;   
    public List<string> Roles { get; set; } = new List<string>();
    public List<RoleAssignment> AvailableRoles { get; set; } = new List<RoleAssignment>();
}

public class RoleAssignment
{
    public string RoleName { get; set; } = string.Empty;    
    public bool IsAssigned { get; set; }
}
