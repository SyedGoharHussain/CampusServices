# Campus Services

Campus Services is a campus help-desk application for submitting and managing service requests. It includes an ASP.NET Core Web API backed by PostgreSQL and a .NET MAUI Blazor client for Windows.

## Features

- Student registration and login with ASP.NET Core Identity
- JWT-based authentication for protected API requests
- Student request creation with category selection
- Student view limited to the student's own requests
- Staff view of all requests
- Staff assignment of requests to themselves
- Controlled request workflow:
  `Open -> Assigned -> InProgress -> Resolved -> Closed`
- Staff-only status updates and assignment
- Swagger/OpenAPI documentation in the API development environment
- EF Core migrations applied automatically when the API starts
- Initial categories and an optional staff account seeded on startup

## Technology stack

| Area | Technology |
| --- | --- |
| API | ASP.NET Core Web API on .NET 10 |
| Authentication | ASP.NET Core Identity and JWT bearer tokens |
| Data access | Entity Framework Core 10 |
| Database | PostgreSQL |
| Client | .NET MAUI Blazor Hybrid |
| API documentation | Swashbuckle / Swagger UI |

## Repository structure

```text
CampusServices/
├── Campus.Api/       ASP.NET Core API, authentication, controllers, startup
├── Campus.Business/  Request workflow and business rules
├── Campus.Client/    .NET MAUI Blazor client
├── Campus.Data/      EF Core context, migrations, repositories, database setup
├── Campus.Models/    Entities, DTOs, and request status definitions
└── CampusServices.slnx
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL 14 or later
- .NET MAUI workload for building the client
- Windows 10 version 1809 or later for running the current client target

Check the installed SDK and workloads with:

```bash
dotnet --version
dotnet workload list
```

## Configuration

The API reads secrets through .NET user-secrets. From the repository root, configure the API project with a PostgreSQL connection string and JWT signing key:

```bash
cd Campus.Api

dotnet user-secrets set "ConnectionStrings:Default" \
  "Host=localhost;Port=5432;Database=campus_services;Username=postgres;Password=your-password"

dotnet user-secrets set "Jwt:Key" \
  "replace-this-with-a-long-random-secret-key"
```

To create a staff account automatically on first startup, also set:

```bash
dotnet user-secrets set "SeedStaffPassword" "Change-this-password1!"
```

The seeded account is:

```text
Email:    staff@campus.com
Password: the value configured as SeedStaffPassword
```

Do not commit passwords, database credentials, or JWT keys. The repository ignores local settings, database files, and build artifacts.

## Running the API

Start PostgreSQL, create the configured database if it does not already exist, and run:

```bash
dotnet run --project Campus.Api
```

The API uses these development URLs:

- HTTP: `http://localhost:5080`
- HTTPS: `https://localhost:7255`
- Swagger UI: `http://localhost:5080/swagger`

On startup, the API applies the checked-in EF Core migrations, creates the `Student` and `Staff` roles, seeds the default categories, and creates the seeded staff user when `SeedStaffPassword` is configured.

## Running the client

The current client project targets Windows:

```bash
dotnet build Campus.Client/Campus.Client.csproj
dotnet run --project Campus.Client/Campus.Client.csproj
```

Start the API before launching the client. The client is configured to call `http://localhost:5080/` in `Campus.Client/MauiProgram.cs`. If the API is hosted at another address, update that base address before running the client.

The client supports:

1. Registering a student account
2. Logging in as a student or staff member
3. Creating and viewing service requests
4. Assigning requests and advancing their status when logged in as staff

The `Counter` and `Weather` pages are still present as default MAUI/Blazor sample pages.

## API overview

All request-management endpoints require a bearer token from `POST /api/auth/login`.

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | Public | Register a student |
| `POST` | `/api/auth/login` | Public | Authenticate and receive a JWT |
| `GET` | `/api/categories` | Authenticated | List request categories |
| `GET` | `/api/requests` | Authenticated | List own requests, or all requests for staff |
| `GET` | `/api/requests/{id}` | Authenticated | Get an accessible request |
| `POST` | `/api/requests` | Authenticated | Create a service request |
| `PUT` | `/api/requests/{id}/assign` | Staff | Assign a request to the current staff user |
| `PUT` | `/api/requests/{id}/status` | Staff | Move a request to an allowed next status |

Example login:

```bash
curl -X POST http://localhost:5080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"staff@campus.com","password":"Change-this-password1!"}'
```

Use the returned token as:

```text
Authorization: Bearer <token>
```

## Request workflow

Status changes are validated by the business layer. The allowed transitions are:

```text
Open       -> Assigned
Assigned   -> InProgress
InProgress -> Resolved
Resolved   -> Closed
Resolved   -> InProgress
Closed     -> (no further transitions)
```

Students can create requests and view their own requests. Only staff can see every request, assign requests, or change request status. These rules are enforced by the API, not only by the client UI.

## Development

Restore dependencies and build the solution with:

```bash
dotnet restore CampusServices.slnx
dotnet build CampusServices.slnx
```

The API's OpenAPI definition is available through Swagger while the environment is `Development`. Database migrations are stored in `Campus.Data/Migrations`.

## License

No license has been specified for this repository yet.
