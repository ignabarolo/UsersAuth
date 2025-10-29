using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UsersAuth.Identity;

public class AppIdentityDBContext : IdentityDbContext<User, Rol, Guid>
{
    public AppIdentityDBContext(DbContextOptions<AppIdentityDBContext> options)
       : base(options)
    {
    }
}
