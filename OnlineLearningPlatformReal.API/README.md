# Online Learning Platform API

## Overview
This project is a backend REST API for an online learning platform.  
It is built using **ASP.NET Core** and provides authentication, user management, and course management functionalities.

The API is designed following clean separation of concerns using Controllers, DTOs, and Entity Framework Core for data access.

---

## Technologies Used
- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- RESTful API architecture

---

## Project Structure
OnlineLearningPlatform/
├── OnlineLearningPlatform.sln
├── README.md
└── OnlineLearningPlatform.API/
├── Controllers/
├── DTOs/
├── Entities/
├── Data/
├── Services/ (if applicable)
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── OnlineLearningPlatform.API.csproj

---

## Features
- User registration and login
- JWT-based authentication and authorization
- User profile management
- Course creation, update, deletion, and retrieval
- Protected endpoints using role-based access (where applicable)

---

## Authentication
The API uses **JWT Bearer Tokens** to secure protected endpoints.

### Authentication Flow
1. The user logs in using the authentication endpoint
2. The API generates and returns a JWT token
3. The client includes the token in the request headers when accessing protected endpoints

### Authorization Header Example
Authorization: Bearer <YOUR_JWT_TOKEN>

---

## How to Run the Project (Step by Step)

### 1. Open the project
- Extract the project ZIP file
- Open `OnlineLearningPlatform.sln` using Visual Studio

---

### 2. Configure the database
Update the connection string in one of the following files:
- `appsettings.json`
- `appsettings.Development.json`

Example:
`json`
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=OnlineLearningPlatform;Trusted_Connection=True;TrustServerCertificate=True;"
}
### 3. Apply database migrations
Open a terminal in the project directory and run:

dotnet ef database update

### 4. Run the API
Press Run (▶) in Visual Studio

The API will start and listen on the configured port

### Testing the API
The API can be tested using:

-Postman
-Swagger UI (if enabled)

### For protected endpoints:

-Authenticate to receive a JWT token
-Add the token to the request headers
-Send requests to secured endpoints

### Environment Configuration

-Development settings are stored in appsettings.Development.json
-Sensitive information such as secrets or keys should not be committed

### Notes

-Build artifacts (bin, obj) are excluded from the project
-The project contains only source code and configuration files
-This API is intended for educational and internship evaluation purposes

### Author
Mohammad Rizk