using UsersAuth.Identity;

namespace UsersAuth.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user);
}
