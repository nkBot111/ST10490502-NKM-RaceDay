# RaceDay Part 1 Documentation

This folder contains the planning and database deliverables completed for Part 1 of the RaceDay Portfolio of Evidence.

## Entity Relationship Diagram

**File:** `ERD.png`

The Entity Relationship Diagram represents the complete RaceDay relational database design.

The database contains six entities:

- Users
- UserProfiles
- Categories
- Events
- Enrollments
- Results

The ERD identifies the attributes, primary keys, foreign keys, unique constraints and relationship cardinalities required by the RaceDay system.

## API Endpoint Plan

**File:** `RaceDay_API_Endpoint_Plan.md`

The API Endpoint Plan defines the RESTful endpoints that will be implemented during Part 2.

The plan covers:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

Each endpoint includes:

- HTTP Method
- Route
- Description
- Role Required
- Request Body
- Expected Response

The plan also includes relevant success and failure HTTP response codes.

## SQL Database Script

**File:** `RaceDay_Database.sql`

The SQL script creates the complete RaceDay database schema using Microsoft SQL Server.

The database contains the same six entities represented in the ERD:

- Users
- UserProfiles
- Categories
- Events
- Enrollments
- Results

The script includes:

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
- Four User Profiles
- Three Event Categories
- Three Events
- Four Event Enrolments
- Two Results

The complete script was tested in SQL Server Management Studio from a clean RaceDayDB database and executed successfully.

## Database Design Decisions

### Users and Roles

The Users table stores both Organisers and Participants. The Role field identifies the type of user and will support role-based access control in Part 2.

### User Profiles

UserProfiles stores additional information about a user. UserID is both a foreign key and a unique value which ensures that a user cannot have more than one profile.

### Events and Categories

Each Event belongs to one Category while one Category may be associated with several Events.

Each Event also references the Organiser responsible for managing it through OrganiserID.

### Event Enrolments

Enrollments connects Participants to Events.

The combination of EventID and ParticipantID is unique which prevents a Participant from enrolling in the same Event more than once.

### Results

Results stores the performance information recorded for completed event enrolments.

EnrollmentID is unique which ensures that each enrolment can have a maximum of one result.

## CI/CD Evidence

**File:** `CI-Success.png`

GitHub Actions is used to validate the RaceDay Part 1 repository structure and required documentation.

The CI/CD screenshot provides evidence of a successful GitHub Actions workflow run.
