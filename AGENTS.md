# UsersAuth — Agent Guide

Single .NET 9 ASP.NET Core MVC + Razor Pages app. Not a monorepo.

## Commands

```powershell
dotnet build                     # build
dotnet run --launch-profile Dev  # dev server on http://localhost:5025
dotnet run --launch-profile Prod # production-mode server on http://localhost:5025
dotnet libman restore            # restore SweetAlert2 client lib (see libman.json)
```

No tests exist in this repo. No CI/CD pipelines.

## Architecture

- **Entrypoint**: `UsersAuth/Program.cs` — configures EF Core (Npgsql), ASP.NET Core Identity, Razor Pages, authorization (AdminPolicy), email, and auto-seeds Admin/User roles + admin user at startup.
- **Identity**: Custom `User` (IdentityUser\<Guid\>) and `Rol` (IdentityRole\<Guid\>) in `UsersAuth/Identity/`. DbContext: `AppIdentityDBContext`.
- **UI**: Razor Pages under `UsersAuth/Pages/Account/` (login, register, forgot/reset password, confirm email, logout) and `UsersAuth/Pages/Admin/` (role management). All pages require authorization except login/register/public. Admin pages require `AdminPolicy` (role "Admin").
- **Controllers**: Minimal — only `HomeController` (returns empty view).
- **Services**: `EmailSender` (SMTP via Gmail App Password) in `UsersAuth/Services/`.
- **Localization**: All user-facing strings are in English (custom `IdentityErrorDescriber`, view models, validation attributes).

## Configuration Requirements

| Key | Source | Purpose |
|-----|--------|---------|
| `ConnectionStrings:DefaultConnection` | user-secrets / env / appsettings | PostgreSQL connection string |
| `EmailSettings:Host` | appsettings / user-secrets | SMTP host (default smtp.gmail.com) |
| `EmailSettings:Port` | appsettings / user-secrets | SMTP port (587) |
| `EmailSettings:Username` | appsettings / user-secrets | Gmail address |
| `EmailSettings:Password` | appsettings / user-secrets | Gmail App Password (16 chars) |
| `EmailSettings:FromEmail` | appsettings / user-secrets | Sender address |
| `EmailSettings:FromName` | appsettings / user-secrets | Sender display name |
| `AdminUser:Email` | user-secrets / env | Email seeded as Admin role on startup |

Set user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=UsersAuth;Username=...;Password=..."
dotnet user-secrets set "AdminUser:Email" "admin@example.com"
dotnet user-secrets set "EmailSettings:Username" "youremail@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "aaaa aaaa aaaa aaaa"
dotnet user-secrets set "EmailSettings:FromEmail" "youremail@gmail.com"
dotnet user-secrets set "EmailSettings:FromName" "MyApp"
```

Email is mandatory for registration and password reset.

## Docker

```
docker build -t usersauth -f Dockerfile .
docker run -p 8080:8080 usersauth
```

Container exposes ports 8080 (HTTP) and 8081 (HTTPS).

## Notable

- EF Core auto-migrates + seeds on startup (`Program.cs:74`). No manual migration step needed in dev.
- New registrations auto-assign "User" role. Admin role only assigned to hardcoded `AdminUser:Email`.
- Lockout: 5 failed attempts → 5 min lockout.
- Password rules: 8+ chars, digit, upper+lower, no special chars required.
- No test projects exist.
