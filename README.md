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
```

---

# Part 3 – MVC Website (NKM)

The ASP.NET Core MVC frontend is in `RaceDay.Web/` and communicates with the separate ASP.NET Core REST API in `Raceday.API/`. Test code is in `RaceDay.Tests/`.

## Website features

- Registration, login and session handling for Participant and Organiser accounts
- Organiser dashboard, event creation, editing and categories
- Organiser enrolment list and result recording
- Participant event search, event details and category-based enrolments
- Participant enrolment history, cancellation and personal race results
- Bootstrap responsive layouts

## Running in GitHub Codespaces

Open a Codespace for this repository and use three separate terminals:

**Terminal 1 – one-time setup**

```bash
bash scripts/setup-codespaces.sh
```

**Terminal 2 – API (leave running)**

```bash
bash scripts/run-api.sh
```

**Terminal 3 – MVC website (leave running)**

```bash
bash scripts/run-web.sh
```

Open the forwarded **port 5080** in the Codespaces Ports tab. The API listens on **5081**. Opening the API's `/` route may return HTTP 404 (no homepage); try `/health` or `/swagger` instead.

Once API and MVC both run, open a fourth terminal and run:

```bash
python3 scripts/smoke-test.py
```

The smoke test creates temporary Organiser/Participant accounts and checks event creation, category creation, enrolment, race results, login and MVC navigation. It writes demo data to the local development database.

For a basic build and xUnit check:

```bash
dotnet build Raceday.API.sln
dotnet test Raceday.API.sln
```

### Database and Docker

`compose.yaml` starts a **local SQL Server 2022 Developer container** on `127.0.0.1:14333`. The setup script creates a random local SQL Server password in `.env` (ignored by Git). The development API applies the existing EF Core migrations at startup. **Do not commit `.env`**. Your Windows LocalDB database is separate from the Codespaces database.

Optional API and MVC Docker images can be built from the repository root:

```bash
docker build -t raceday-nkm-api -f Raceday.API/Dockerfile .
docker build -t raceday-nkm-web -f RaceDay.Web/Dockerfile .
```

Those images are not a public deployment. For production, use HTTPS, managed session storage and appropriately secured database credentials.

### GitHub Actions

`.github/workflows/part3-ci.yml` restores, builds and tests the three-project .NET 8 solution on pushes to `main` and pull requests. Confirm the actual run is green on your GitHub Actions page.

### Image storage

Azure Blob Storage was **not included in the uploaded nkBot111 code**. If required by the Part 3 rubric, it still needs to be implemented and configured with an Azure account; the presence of Docker and MVC alone does not satisfy an Azure upload requirement.

### Demo video and screenshots

Add the final unlisted demonstration-video URL and screenshots when available. Avoid sharing passwords, tokens or database connection strings in screenshots.


---

# RaceDay Part 3 – MVC Web Application

## Project Overview

RaceDay Part 3 is an ASP.NET Core MVC web application connected to the existing RaceDay REST API.

The application allows Organisers to manage running events and Participants to register for races, enrol in events and view their results.

## Features

- Participant and Organiser registration and login
- Role-based dashboards
- Event creation and management
- Event categories
- Participant enrolment
- Race result recording and viewing
- Responsive web interface
- API integration

## Technologies Used

- C# and .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Bootstrap
- GitHub Codespaces
- GitHub Actions
- Docker

## Running the Application

Open the repository in GitHub Codespaces.

Run the database setup:

bash scripts/setup-codespaces.sh

Start the API:

bash scripts/run-api.sh

Start the MVC website in a separate terminal:

bash scripts/run-web.sh

Open port 5080 to view the website.

## Testing

Run the automated tests:

dotnet test Raceday.API.sln

## Part 3 Demonstration Video

Watch the completed RaceDay Part 3 demonstration:

[Watch RaceDay Part 3 on YouTube](https://youtu.be/O4Rw3qGG3eY?si=SRRYVJtGWvghUUJv)

## GitHub Repository

https://github.com/nkBot111/ST10490502-NKM-RaceDay

