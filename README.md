# RaceDay

## Project Overview

RaceDay is a full-stack web-based event management system designed for the South African road running, walking and cycling community.

The purpose of RaceDay is to provide a central platform where Event Organisers can create and manage events, categories, participant enrolments and race results while Participants can browse available events, enter events and track their personal performance history.

The Portfolio of Evidence is developed progressively across three parts. Part 1 focuses on system planning and database design before the RESTful API is implemented in Part 2 and the MVC web application is developed in Part 3.

---

## System Roles

RaceDay supports two system roles:

### Organiser

An Organiser is responsible for managing RaceDay events.

An Organiser can:

- Create new events
- Update existing events
- Delete events
- Manage event categories
- View participants enrolled in an event
- Capture participant race results
- Update participant results

### Participant

A Participant uses RaceDay to discover and participate in events.

A Participant can:

- Register an account
- Log into RaceDay
- Manage their profile
- Browse available events
- View event information
- Enter an event
- View their event enrolments
- Cancel an eligible enrolment
- View their personal race results and performance history

Role-based access will be enforced at API level during Part 2 and reflected in the MVC interface developed in Part 3.

---

# Part 1 – System Planning and Database

Part 1 focuses on planning the RaceDay system before application code is developed.

The required planning documents and SQL database script are stored inside the `/docs` folder.

## Part 1 Deliverables

### Entity Relationship Diagram

The RaceDay ERD contains six database entities:

1. Users
2. UserProfiles
3. Categories
4. Events
5. Enrollments
6. Results

The ERD identifies the attributes, primary keys, foreign keys, unique constraints and cardinality of the relationships between the entities.

[View the RaceDay ERD](docs/ERD.png)

---

### API Endpoint Plan

The API Endpoint Plan defines the RESTful endpoints that will be implemented during Part 2.

The endpoint plan covers:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

Each endpoint identifies:

- HTTP Method
- Route
- Description
- Role Required
- Request Body
- Expected Response

Both successful and failure HTTP responses are included in the plan.

[View the API Endpoint Plan](docs/RaceDay_API_Endpoint_Plan.md)

---

### SQL Database Script

The RaceDay database is implemented using Microsoft SQL Server.

The SQL script creates all six entities represented in the ERD and includes:

- Primary keys
- Foreign keys
- NOT NULL constraints
- UNIQUE constraints
- DEFAULT constraints
- CHECK constraints
- Realistic sample data

The database is seeded with:

- Two Organisers
- Two Participants
- User profiles
- Three event categories
- Three RaceDay events
- Four sample event enrolments
- Two sample race results

[View the SQL Database Script](docs/RaceDay_Database.sql)

---

## Database Design Decisions

### Users and Roles

RaceDay uses one `Users` table for both Organisers and Participants.

The `Role` attribute identifies whether a user is an `Organiser` or `Participant`. This prevents duplication of common account information such as names, email addresses and passwords while also supporting role-based access control in Part 2.

### User Profiles

The `UserProfiles` entity stores additional personal information associated with a RaceDay user.

`UserID` is both a foreign key and a unique value in `UserProfiles`. This ensures that one user can have no more than one profile.

### Events and Categories

Every RaceDay event belongs to one category while a category can be associated with multiple events.

Each event also stores the `OrganiserID` of the Organiser responsible for creating and managing it.

### Event Enrolments

The `Enrollments` entity connects Participants to Events.

A Participant can enrol in several events and an Event can contain several Participants.

The combination of `EventID` and `ParticipantID` is unique which prevents the same Participant from enrolling in the same Event more than once.

### Results

The `Results` entity stores performance information for completed event enrolments.

`EnrollmentID` is unique in the Results table which ensures that an enrolment can have a maximum of one result.

---

## Database Setup Instructions

To create the RaceDay database:

1. Open Microsoft SQL Server Management Studio (SSMS).
2. Connect to a SQL Server instance.
3. Open the file `docs/RaceDay_Database.sql`.
4. Select the complete SQL script.
5. Click **Execute**.
6. The script will recreate the `RaceDayDB` database from a clean state.
7. Confirm that the script executes successfully.
8. Confirm that the following six tables are created:

   - Users
   - UserProfiles
   - Categories
   - Events
   - Enrollments
   - Results

9. Confirm that the sample data is displayed successfully in the Results section of SSMS.

---

## GitHub and Version Control

GitHub is used to manage the RaceDay source code and project documentation.

Development work is committed progressively using meaningful commit messages that describe the work completed.

All Part 1 planning documents are stored inside the `/docs` folder.

---

## Continuous Integration

GitHub Actions is used to validate the structure of the RaceDay Part 1 repository.

The CI workflow checks that the required documentation files are available and performs validation of the API Endpoint Plan and SQL database script.

The workflow is stored in:

`.github/workflows/validate-docs.yml`

### Successful CI/CD Validation

The following screenshot shows a successful GitHub Actions validation run:

![Successful RaceDay CI/CD Validation](docs/ci-success.png)

---

## Repository Structure

```text
ST10490502-NKM-RaceDay/
│
├── .github/
│   └── workflows/
│       └── validate-docs.yml
│
├── docs/
│   ├── ERD.png
│   ├── RaceDay_API_Endpoint_Plan.md
│   ├── RaceDay_Database.sql
│   ├── README.md
│   └── ci-success.png
│
└── README.md
