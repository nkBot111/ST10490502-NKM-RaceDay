# RaceDay

RaceDay is a web-based event management system designed for road running, walking and cycling events in South Africa.

The system provides a central platform where Organisers can create and manage sporting events while Participants can register for events, select event categories, manage their enrolments and view race results.

The RaceDay project is developed progressively as part of the Portfolio of Evidence.

---

# System Roles

RaceDay supports two main user roles:

## Organiser

An Organiser is responsible for creating and managing RaceDay events.

An Organiser can:

- Register and log in to the system.
- Manage their personal profile.
- Create new events.
- Update events that they created.
- Delete eligible events.
- Create and manage event categories.
- View Participants enrolled in their events.
- Capture race results.
- Update race results.

## Participant

A Participant uses RaceDay to discover and enter sporting events.

A Participant can:

- Register and log in.
- Manage their personal profile.
- Browse available events.
- View event categories.
- Enrol in an event.
- Select a category when enrolling.
- View their personal enrolments.
- Cancel eligible enrolments.
- View their race results.

Role-based restrictions are enforced by the API so that Organisers and Participants can only access functionality appropriate to their role.

---

# Part 1 – System Planning and Database Design

Part 1 focused on planning the RaceDay system before implementation.

The Part 1 documentation is stored in the `/docs` folder.

The documentation includes:

- `ERD.png` – Entity Relationship Diagram for RaceDay.
- `RaceDay_API_Endpoint_Plan.md` – planned REST API endpoints.
- `RaceDay_Database.sql` – SQL Server database creation script.
- SQL execution evidence.
- GitHub Actions Continuous Integration evidence.
- Supporting RaceDay planning documentation.

Part 1 established the database structure, entities, relationships and planned API functionality that were implemented during Part 2.

---

# Part 2 – ASP.NET Core REST API

Part 2 implements the RaceDay backend as an ASP.NET Core REST API.

The API was developed using C#, .NET 8, Entity Framework Core and SQL Server.

The backend provides functionality for:

- User registration.
- User login and logout.
- Session management.
- User profiles.
- Event management.
- Event categories.
- Participant enrolments.
- Race results.
- Role-based access control.
- Input validation.
- Automated testing.
- Swagger/OpenAPI documentation.

---

# Technologies Used

The RaceDay project uses the following technologies:

- C#
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- Entity Framework Core Code First
- Microsoft SQL Server
- SQL Server LocalDB
- Swagger / OpenAPI
- xUnit
- Entity Framework Core InMemory Testing
- Git
- GitHub
- GitHub Actions
- Visual Studio 2022

---

# Project Structure

Important Part 2 files and folders include:

```text
Controllers/
DTOs/
Data/
Migrations/
Models/
Properties/
docs/

AuthControllerTests.cs
RoleAndEnrollmentTests.cs
TestSession.cs
RaceDay.Tests.csproj

Program.cs
Raceday.API.csproj
Raceday.API.sln
Raceday.API.http
appsettings.json
appsettings.Development.json
