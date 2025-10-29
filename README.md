# 🚀 [UsersAuth] - .NET Core MVC Web Application

This project is a web application developed using **.NET Core (MVC)**, designed to demonstrate the implementation of security and identity management, using Identity Framework.

The deployment follows a **Zero Cost Infrastructure** approach, utilizing free cloud services for both hosting and the database.

## 🛠️ Technology Stack

* **Backend:** .NET Core 9.0
* **Frameworks:** ASP.NET Core MVC
* **Libraries:** Bootstrap 5, SweetAlert2
* **Database:** PostgreSQL
* **Hosting (Web Service):** Render (Free Tier)
* **Database (DBaaS):** Neon (Free Tier)

---

## 🌐 Access

The application is currently deployed and publicly accessible at the following address:

* **Production URL (Render):** `[no ready yet]`

### ⚠️ Important Note on Free Hosting (Render)

**CRITICAL WARNING:** Because the project uses the Render Free Tier, the web service will **automatically spin down after 15 minutes of inactivity**. The application will become available again automatically upon request, but the first user may experience a cold start delay of 20-30 seconds.

---

## ⚙️ Local Environment Setup

### Prerequisites

Before running the project locally, ensure you have the following installed:

* .[NET SDK] 9.0

* Git

* PostgreSQL

#### 📧 Email Sending Configuration

**Note:** *This configuration is mandatory for features requiring email (e.g., registration, password reset) to function correctly.*

This application uses Gmail SMTP services. Due to Google's security policies, direct use of your main Google password will fail. You must generate an App Password.

Follow these steps to set up your credentials:

1.  Enable Two-Step Verification (2FA) on your Google Account (this is a prerequisite).

2.  Navigate to the Security settings of your Google Account.

3.  Under "Signing in to Google," generate a new App Password.

4.  Use this generated 16-character App Password as the value for the required SMTP environment variable in your project's configuration ***(appsettings.Development.json)***. 

    **appsettings.Development.json**

        {
          "EmailSettings": {
          "Host": "smtp.gmail.com", // By default, do not change
          "Port": 587, // Standard port for STARTTLS/TLS
          "Username": "youremail@gmail.com",
          "Password": "aaaa aaaa aaaa aaaa", // Use App Password
          "FromEmail": "youremail@gmail.com", // It must match the user if you use Gmail.
          "FromName": "Name of your application",
          "EnableSsl": true
          }
        }

**Need more detail?** Search for: "Generate Google Gmail App Password"

#### 🛡️ Role Seeding & Initialization

This code block runs once during application startup (after database migration, if applicable).

1.  It ensures the security foundation is properly set by: Creating mandatory roles (Admin, User) if they do not exist in the database.

2.  Assigning the Admin role to the hardcoded test email (youremail@gmail.com) to guarantee that an initial administrative user is always available upon first execution.

**ACTION REQUIRED:** If you intend to use a different administrator account for testing, you must update the hardcoded email address in the application's startup code.
