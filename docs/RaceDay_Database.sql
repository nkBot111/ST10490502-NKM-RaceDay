SET NOCOUNT ON;
GO


/*==============================================================
  STEP 1: CREATE DATABASE FROM A CLEAN STATE
==============================================================*/

USE master;
GO

IF DB_ID('RaceDayDB') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE RaceDayDB;
END;
GO

CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO


/*==============================================================
  TABLE 1: USERS
==============================================================*/

CREATE TABLE dbo.Users
(
    UserID INT IDENTITY(1,1) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,

    CreatedAt DATETIME2(7) NOT NULL
        CONSTRAINT DF_Users_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Users
        PRIMARY KEY (UserID),

    CONSTRAINT UQ_Users_Email
        UNIQUE (Email),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);
GO


/*==============================================================
  TABLE 2: USER PROFILES
==============================================================*/

CREATE TABLE dbo.UserProfiles
(
    ProfileID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    DateOfBirth DATE NULL,
    EmergencyContactName NVARCHAR(100) NULL,
    EmergencyContactNumber NVARCHAR(20) NULL,

    CONSTRAINT PK_UserProfiles
        PRIMARY KEY (ProfileID),

    CONSTRAINT UQ_UserProfiles_UserID
        UNIQUE (UserID),

    CONSTRAINT FK_UserProfiles_Users
        FOREIGN KEY (UserID)
        REFERENCES dbo.Users(UserID)
);
GO


/*==============================================================
  TABLE 3: CATEGORIES
==============================================================*/

CREATE TABLE dbo.Categories
(
    CategoryID INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(300) NULL,

    CONSTRAINT PK_Categories
        PRIMARY KEY (CategoryID),

    CONSTRAINT UQ_Categories_CategoryName
        UNIQUE (CategoryName)
);
GO


/*==============================================================
  TABLE 4: EVENTS
==============================================================*/

CREATE TABLE dbo.Events
(
    EventID INT IDENTITY(1,1) NOT NULL,
    OrganiserID INT NOT NULL,
    CategoryID INT NOT NULL,
    EventName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    EventDate DATE NOT NULL,
    StartTime TIME(0) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    MaximumParticipants INT NOT NULL,

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Events_Status
        DEFAULT 'Open',

    CreatedAt DATETIME2(7) NOT NULL
        CONSTRAINT DF_Events_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Events
        PRIMARY KEY (EventID),

    CONSTRAINT FK_Events_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES dbo.Users(UserID),

    CONSTRAINT FK_Events_Category
        FOREIGN KEY (CategoryID)
        REFERENCES dbo.Categories(CategoryID),

    CONSTRAINT CK_Events_DistanceKm
        CHECK (DistanceKm > 0),

    CONSTRAINT CK_Events_MaximumParticipants
        CHECK (MaximumParticipants > 0),

    CONSTRAINT CK_Events_Status
        CHECK
        (
            Status IN
            ('Open', 'Closed', 'Completed', 'Cancelled')
        )
);
GO


/*==============================================================
  TABLE 5: ENROLLMENTS
==============================================================*/

CREATE TABLE dbo.Enrollments
(
    EnrollmentID INT IDENTITY(1,1) NOT NULL,
    EventID INT NOT NULL,
    ParticipantID INT NOT NULL,

    EnrollmentDate DATETIME2(7) NOT NULL
        CONSTRAINT DF_Enrollments_EnrollmentDate
        DEFAULT SYSUTCDATETIME(),

    EnrollmentStatus NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Enrollments_Status
        DEFAULT 'Active',

    CONSTRAINT PK_Enrollments
        PRIMARY KEY (EnrollmentID),

    CONSTRAINT FK_Enrollments_Events
        FOREIGN KEY (EventID)
        REFERENCES dbo.Events(EventID),

    CONSTRAINT FK_Enrollments_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES dbo.Users(UserID),

    CONSTRAINT UQ_Enrollments_Event_Participant
        UNIQUE (EventID, ParticipantID),

    CONSTRAINT CK_Enrollments_Status
        CHECK
        (
            EnrollmentStatus IN
            ('Active', 'Completed', 'Cancelled')
        )
);
GO


/*==============================================================
  TABLE 6: RESULTS
==============================================================*/

CREATE TABLE dbo.Results
(
    ResultID INT IDENTITY(1,1) NOT NULL,
    EnrollmentID INT NOT NULL,
    FinishTime TIME(0) NOT NULL,
    OverallPosition INT NOT NULL,
    CategoryPosition INT NOT NULL,

    RecordedAt DATETIME2(7) NOT NULL
        CONSTRAINT DF_Results_RecordedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Results
        PRIMARY KEY (ResultID),

    CONSTRAINT FK_Results_Enrollments
        FOREIGN KEY (EnrollmentID)
        REFERENCES dbo.Enrollments(EnrollmentID),

    CONSTRAINT UQ_Results_EnrollmentID
        UNIQUE (EnrollmentID),

    CONSTRAINT CK_Results_OverallPosition
        CHECK (OverallPosition > 0),

    CONSTRAINT CK_Results_CategoryPosition
        CHECK (CategoryPosition > 0)
);
GO


/*==============================================================
  SEED DATA
==============================================================*/


/*==============================================================
  USERS
  Minimum Requirement:
  - 2 Organisers
  - 2 Participants
==============================================================*/

INSERT INTO dbo.Users
(
    FirstName,
    LastName,
    Email,
    PasswordHash,
    Role,
    CreatedAt
)
VALUES
(
    'Nandi',
    'Mabuza',
    'nandi.mabuza@raceday.co.za',
    LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256', 'NandiRaceDay@2026'), 2)),
    'Organiser',
    '2026-09-01T15:04:30.2961983'
),
(
    'Tumelo',
    'Radebe',
    'tumelo.radebe@raceday.co.za',
    LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256', 'TumeloRaceDay@2026'), 2)),
    'Organiser',
    '2026-09-01T15:04:30.2961983'
),
(
    'Lesedi',
    'Nkomo',
    'lesedi.nkomo@email.co.za',
    LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256', 'LesediRaceDay@2026'), 2)),
    'Participant',
    '2026-09-01T15:04:30.2961983'
),
(
    'Ayanda',
    'Mkhize',
    'ayanda.mkhize@email.co.za',
    LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256', 'AyandaRaceDay@2026'), 2)),
    'Participant',
    '2026-09-01T15:04:30.2961983'
);
GO


/*==============================================================
  USER PROFILES
==============================================================*/

INSERT INTO dbo.UserProfiles
(
    UserID,
    PhoneNumber,
    DateOfBirth,
    EmergencyContactName,
    EmergencyContactNumber
)
VALUES
(
    1,
    '0825550115',
    '1991-04-17',
    'Bongani Mabuza',
    '0825550215'
),
(
    2,
    '0835550126',
    '1993-08-24',
    'Lebo Radebe',
    '0835550226'
),
(
    3,
    '0715550137',
    '2002-02-11',
    'Thandi Nkomo',
    '0715550237'
),
(
    4,
    '0725550148',
    '2001-10-06',
    'Sanele Mkhize',
    '0725550248'
);
GO


/*==============================================================
  CATEGORIES
==============================================================*/

INSERT INTO dbo.Categories
(
    CategoryName,
    Description
)
VALUES
(
    'Road Running',
    'Timed road-running events for recreational and competitive participants.'
),
(
    'Community Walking',
    'Community walking events that promote fitness, wellness and social participation.'
),
(
    'Road Cycling',
    'Organised cycling events held on approved road routes for participating cyclists.'
);
GO


/*==============================================================
  EVENTS
  Minimum Requirement:
  - 3 Events
  - Category assigned to each Event
==============================================================*/

INSERT INTO dbo.Events
(
    OrganiserID,
    CategoryID,
    EventName,
    Description,
    EventDate,
    StartTime,
    Location,
    DistanceKm,
    MaximumParticipants,
    Status,
    CreatedAt
)
VALUES
(
    1,
    1,
    'Hartbeespoort Sunrise 10K',
    'A 10 kilometre road-running event designed for recreational and competitive runners.',
    '2026-08-15',
    '07:00:00',
    'Hartbeespoort, North West',
    10.00,
    500,
    'Completed',
    '2026-09-01T15:04:30.3187168'
),
(
    2,
    2,
    'Mbombela Wellness Community Walk',
    'A community walking event focused on promoting health, wellness and social participation.',
    '2026-09-20',
    '08:00:00',
    'Mbombela, Mpumalanga',
    5.00,
    300,
    'Open',
    '2026-09-01T15:04:30.3187168'
),
(
    1,
    3,
    'Kimberley Diamond Charity Cycle',
    'A charity cycling event for participants travelling through selected road routes around Kimberley.',
    '2026-10-03',
    '06:30:00',
    'Kimberley, Northern Cape',
    40.00,
    400,
    'Open',
    '2026-09-01T15:04:30.3187168'
);
GO


/*==============================================================
  SAMPLE ENROLLMENTS
==============================================================*/

INSERT INTO dbo.Enrollments
(
    EventID,
    ParticipantID,
    EnrollmentDate,
    EnrollmentStatus
)
VALUES
(
    1,
    3,
    '2026-07-20T10:15:00',
    'Completed'
),
(
    1,
    4,
    '2026-07-22T14:30:00',
    'Completed'
),
(
    2,
    3,
    '2026-08-25T09:20:00',
    'Active'
),
(
    3,
    4,
    '2026-08-26T11:45:00',
    'Active'
);
GO


/*==============================================================
  SAMPLE RESULTS
==============================================================*/

INSERT INTO dbo.Results
(
    EnrollmentID,
    FinishTime,
    OverallPosition,
    CategoryPosition,
    RecordedAt
)
VALUES
(
    1,
    '00:48:32',
    18,
    12,
    '2026-08-15T10:30:00'
),
(
    2,
    '00:53:14',
    31,
    20,
    '2026-08-15T10:35:00'
);
GO


/*==============================================================
  FINAL VERIFICATION
  Displays the data from all six RaceDay tables.
==============================================================*/

SELECT *
FROM dbo.Users;
GO

SELECT *
FROM dbo.UserProfiles;
GO

SELECT *
FROM dbo.Categories;
GO

SELECT *
FROM dbo.Events;
GO

SELECT *
FROM dbo.Enrollments;
GO

SELECT *
FROM dbo.Results;
GO