# UsersAuth — ASP.NET Core Identity Demo

ASP.NET Core 9.0 Razor Pages application demonstrating security and identity management with ASP.NET Core Identity Framework. Uses PostgreSQL (EF Core/Npgsql) and is deployable on free-tier services (Render + Neon).

## Tech Stack

- **Backend:** .NET 9.0, ASP.NET Core Razor Pages
- **Identity:** ASP.NET Core Identity with custom `User`/`Rol` (Guid PKs)
- **Database:** PostgreSQL via Entity Framework Core + Npgsql
- **UI:** Bootstrap 5, SweetAlert2
- **Email:** SMTP via Gmail App Password
- **Hosting:** Render (free tier) + Neon (free tier PostgreSQL)

## Local Setup

### Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- PostgreSQL (local or remote)
- Gmail account with [App Password](https://support.google.com/accounts/answer/185833) generated

### Configuration

Set all secrets via `dotnet user-secrets` (do not edit `appsettings.Development.json` directly):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=UsersAuth;Username=postgres;Password=postgres"
dotnet user-secrets set "AdminUser:Email" "admin@example.com"
dotnet user-secrets set "EmailSettings:Username" "youremail@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-16-char-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "youremail@gmail.com"
dotnet user-secrets set "EmailSettings:FromName" "UsersAuth"
```

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `AdminUser:Email` | Email that auto-gets the Admin role on startup |
| `EmailSettings:Username` | Gmail address |
| `EmailSettings:Password` | Gmail App Password (16 characters) |
| `EmailSettings:FromEmail` | Sender address (must match Username for Gmail) |
| `EmailSettings:FromName` | Display name for sent emails |

### Run

```powershell
dotnet restore                    # restore NuGet packages
dotnet libman restore             # restore SweetAlert2 client library
dotnet run --launch-profile Dev   # start on http://localhost:5025
dotnet run --launch-profile Prod  # production mode on http://localhost:5025
```

EF Core auto-migrates and seeds on startup — no manual migration step needed.

### Docker

```powershell
docker build -t usersauth -f Dockerfile .
docker run -p 8080:8080 usersauth
```

Container exposes ports 8080 (HTTP) and 8081 (HTTPS).

## Architecture

- **Entrypoint:** `Program.cs` — configures EF Core, Identity, Razor Pages, authorization (AdminPolicy), email, and seeds roles + admin user at startup
- **Identity:** `Identity/` — `User` (IdentityUser\<Guid\>), `Rol` (IdentityRole\<Guid\>), `AppIdentityDBContext`
- **UI:** Razor Pages in `Pages/` — `Account/` (login, register, password reset, email confirmation, logout), `Admin/` (role management)
- **Authorization:** `AdminPolicy` requires `"Admin"` role. Admin pages are restricted. Public pages: login, register, forgot/reset password
- **Controllers:** Minimal — only `HomeController`
- **Services:** `EmailSender` — SMTP via Gmail App Password

### Security Defaults

- Password: min 8 chars, requires digit + uppercase + lowercase, no special chars required
- Lockout: 5 failed attempts → 5 minute lockout
- New registrations auto-assigned `"User"` role
- Admin role auto-assigned on startup to the email configured in `AdminUser:Email`
