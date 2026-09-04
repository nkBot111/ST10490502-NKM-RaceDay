# RaceDay Part 1 Documentation

This folder contains the planning and database deliverables for Part 1 of the RaceDay Portfolio of Evidence.

## Entity Relationship Diagram

**File:** `ERD.png`

The Entity Relationship Diagram represents the complete RaceDay database structure.

It contains the following six entities:

- Users
- UserProfiles
- Categories
- Events
- Enrollments
- Results

The ERD identifies all attributes, primary keys, foreign keys, unique constraints and relationship cardinalities used by the RaceDay database.

## API Endpoint Plan

**File:** `RaceDay_API_Endpoint_Plan.md`

The API Endpoint Plan defines the RESTful endpoints that will be implemented in Part 2.

The plan covers:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

Each endpoint specifies the HTTP method, route, description, required role, request body and expected success and failure responses.

## SQL Database Script

**File:** `RaceDay_Database.sql`

The SQL script creates the complete RaceDay database using Microsoft SQL Server.

The database contains six tables that match the ERD:

- Users
- UserProfiles
- Categories
- Events
- Enrollments
- Results

The script includes primary keys, foreign keys, NOT NULL constraints, UNIQUE constraints, DEFAULT constraints and CHECK constraints.

The database is also seeded with realistic sample data including:

- Two Organisers
- Two Participants
- Four user profiles
- Three event categories
- Three events
- Four sample enrolments
- Two sample results

The complete SQL script was tested in SQL Server Management Studio from a clean RaceDayDB database and executed successfully.

## Database Design Decisions

The Users table stores both Organisers and Participants. The Role attribute distinguishes between the two types of users and supports role-based access control for the API developed in Part 2.

The UserProfiles table has a one-to-zero-or-one relationship with Users. The UserID foreign key is unique which prevents a user from having more than one profile.

The Categories table groups RaceDay events according to their event type. One category can be associated with several events while each event belongs to one category.

The Enrollments table connects Participants and Events. The combination of EventID and ParticipantID is unique which prevents a Participant from entering the same event more than once.

The Results table is linked to Enrollments. EnrollmentID is unique which ensures that each completed enrolment can have a maximum of one race result.

## CI/CD Evidence

**File:** `ci-success.png`

The screenshot provides evidence that the GitHub Actions workflow successfully validated the RaceDay Part 1 repository and required documentation.
