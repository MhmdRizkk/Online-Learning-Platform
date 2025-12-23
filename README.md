Online Learning Platform – Backend API

This project is a secure ASP.NET Core Web API built for an online learning platform.
It implements authentication, authorization, and role management using ASP.NET Core Identity, JWT Bearer tokens, and refresh token rotation, following backend security and architecture best practices.

🚀 Features

User registration and login

Secure password hashing via ASP.NET Core Identity

JWT Bearer authentication

Refresh token generation and rotation

Role-based authorization (Admin / User)

Protected API endpoints

Entity Framework Core with SQL Server

Clean project structure with separation of concerns

Swagger (OpenAPI) documentation with authentication support

🛠️ Technologies Used

ASP.NET Core 8

ASP.NET Core Identity

Entity Framework Core

SQL Server

JWT (JSON Web Tokens)

Swagger / OpenAPI

C#

📁 Project Structure
OnlineLearningPlatform.API/
├── Controllers/
├── Data/
├── DTOs/
├── Models/
├── Services/
├── Migrations/
├── Program.cs
├── appsettings.json
├── OnlineLearningPlatformReal.API.csproj
└── README.md


Controllers: API endpoints

Models: Database entities

DTOs: Request/response objects

Services: Authentication and token logic

Data: EF Core DbContext and configuration

Migrations: Database schema migrations

🔐 Authentication & Authorization

Authentication is handled using JWT Bearer tokens

Passwords are hashed and salted by ASP.NET Core Identity

Refresh tokens are stored securely and rotated on use

Role-based authorization is implemented using [Authorize(Roles = "...")]

Example Roles

Admin

User

🧪 Running the Project Locally
Prerequisites

.NET 8 SDK

SQL Server (local or remote)

Steps

Clone the repository

Configure the database connection in appsettings.json

Apply migrations:

dotnet ef database update


Run the application:

dotnet run


Open Swagger:

http://localhost:5000/swagger

🔑 Configuration Notes

appsettings.json contains placeholder values

Environment-specific files like:

appsettings.Development.json

appsettings.Production.json

are intentionally ignored via .gitignore to avoid leaking secrets.

📌 API Security

All endpoints (except authentication endpoints) require authorization

Unauthorized access returns proper HTTP status codes:

401 Unauthorized

403 Forbidden

📦 Git & Repository Hygiene

Build artifacts (bin/, obj/, .vs/) are excluded via .gitignore

Only source code and configuration templates are committed

📄 License

This project is intended for educational and internship purposes.

✅ Status

✔ Complete
✔ Secure
✔ Mentor-aligned
✔ Submission-ready
