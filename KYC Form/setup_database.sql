-- Simple solution: Just use the existing database
-- This script will work even if database already exists

USE KYC_DB;
GO

-- Drop tables if they exist (in reverse order due to foreign keys)
IF OBJECT_ID('dbo.KYCSubmissions', 'U') IS NOT NULL DROP TABLE dbo.KYCSubmissions;
IF OBJECT_ID('dbo.FundSources', 'U') IS NOT NULL DROP TABLE dbo.FundSources;
IF OBJECT_ID('dbo.IncomeRanges', 'U') IS NOT NULL DROP TABLE dbo.IncomeRanges;
IF OBJECT_ID('dbo.OccupationTypes', 'U') IS NOT NULL DROP TABLE dbo.OccupationTypes;
IF OBJECT_ID('dbo.AddressTypes', 'U') IS NOT NULL DROP TABLE dbo.AddressTypes;
IF OBJECT_ID('dbo.ResidentialStatuses', 'U') IS NOT NULL DROP TABLE dbo.ResidentialStatuses;
IF OBJECT_ID('dbo.Nationalities', 'U') IS NOT NULL DROP TABLE dbo.Nationalities;
IF OBJECT_ID('dbo.MaritalStatuses', 'U') IS NOT NULL DROP TABLE dbo.MaritalStatuses;
IF OBJECT_ID('dbo.Genders', 'U') IS NOT NULL DROP TABLE dbo.Genders;
IF OBJECT_ID('dbo.Branches', 'U') IS NOT NULL DROP TABLE dbo.Branches;
IF OBJECT_ID('dbo.CustomerTypes', 'U') IS NOT NULL DROP TABLE dbo.CustomerTypes;
IF OBJECT_ID('dbo.AccountTypes', 'U') IS NOT NULL DROP TABLE dbo.AccountTypes;
GO

-- Create Lookup Tables

-- Account Type
CREATE TABLE AccountTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO AccountTypes (TypeName) VALUES 
('Savings Account'), ('Current Account'), ('Fixed Deposit'), ('Recurring Deposit');

-- Customer Type
CREATE TABLE CustomerTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO CustomerTypes (TypeName) VALUES 
('Individual'), ('Non-Individual');

-- Preferred Branch
CREATE TABLE Branches (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BranchName NVARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO Branches (BranchName) VALUES 
('Mumbai Main'), ('Delhi Central'), ('Bangalore South'), ('Chennai Anna Nagar'), ('Kolkata Park Street');

-- Gender
CREATE TABLE Genders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    GenderName NVARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO Genders (GenderName) VALUES 
('Male'), ('Female'), ('Other');

-- Marital Status
CREATE TABLE MaritalStatuses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO MaritalStatuses (StatusName) VALUES 
('Single'), ('Married'), ('Widowed'), ('Divorced');

-- Nationality
CREATE TABLE Nationalities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NationalityName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO Nationalities (NationalityName) VALUES 
('Indian'), ('American'), ('British'), ('Other');

-- Residential Status
CREATE TABLE ResidentialStatuses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO ResidentialStatuses (StatusName) VALUES 
('Resident'), ('NRI'), ('OCI'), ('PIO');

-- Address Type
CREATE TABLE AddressTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO AddressTypes (TypeName) VALUES 
('Residential'), ('Office'), ('Other');

-- Occupation Type
CREATE TABLE OccupationTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO OccupationTypes (TypeName) VALUES 
('Salaried'), ('Business'), ('Retired'), ('Student'), ('Housewife'), ('Other');

-- Annual Income
CREATE TABLE IncomeRanges (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RangeText NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO IncomeRanges (RangeText) VALUES 
('Below ₹1 Lakh'), ('₹1-3 Lakh'), ('₹3-5 Lakh'), ('₹5-10 Lakh'), ('Above ₹10 Lakh');

-- Source of Funds
CREATE TABLE FundSources (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SourceName NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

INSERT INTO FundSources (SourceName) VALUES 
('Salary'), ('Business'), ('Investments'), ('Others');


-- Create Main KYC Table
CREATE TABLE KYCSubmissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    
    -- Section 1
    AccountTypeId INT FOREIGN KEY REFERENCES AccountTypes(Id),
    CustomerTypeId INT FOREIGN KEY REFERENCES CustomerTypes(Id),
    BranchId INT FOREIGN KEY REFERENCES Branches(Id),
    ApplicationDate DATE,

    -- Section 2
    Email NVARCHAR(100),
    Mobile NVARCHAR(15),
    AlternateMobile NVARCHAR(15),

    -- Section 3
    AadhaarNumber NVARCHAR(20),
    AadhaarName NVARCHAR(100),
    AadhaarDob DATE,
    GenderId INT FOREIGN KEY REFERENCES Genders(Id),

    -- Section 4
    FirstName NVARCHAR(50),
    MiddleName NVARCHAR(50),
    LastName NVARCHAR(50),
    FatherName NVARCHAR(100),
    MotherName NVARCHAR(100),
    SpouseName NVARCHAR(100),
    MaritalStatusId INT FOREIGN KEY REFERENCES MaritalStatuses(Id),
    NationalityId INT FOREIGN KEY REFERENCES Nationalities(Id),
    Religion NVARCHAR(50),
    ResidentialStatusId INT FOREIGN KEY REFERENCES ResidentialStatuses(Id),
    PlaceOfBirth NVARCHAR(100),
    CountryOfBirth NVARCHAR(100),

    -- Section 5
    Street NVARCHAR(200),
    Area NVARCHAR(100),
    Location NVARCHAR(100),
    PostOffice NVARCHAR(100),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    Pincode NVARCHAR(10),
    AddressTypeId INT FOREIGN KEY REFERENCES AddressTypes(Id),
    IsPermanentAddressSame BIT,
    PermanentAddress NVARCHAR(MAX),

    -- Section 6
    OccupationTypeId INT FOREIGN KEY REFERENCES OccupationTypes(Id),
    EmployerName NVARCHAR(100),
    NatureOfBusiness NVARCHAR(100),
    Designation NVARCHAR(100),
    IncomeRangeId INT FOREIGN KEY REFERENCES IncomeRanges(Id),
    FundSourceId INT FOREIGN KEY REFERENCES FundSources(Id),

    -- Section 7
    PanNumber NVARCHAR(20),
    PanHolderName NVARCHAR(100),
    DlNumber NVARCHAR(50),
    DlDob DATE,
    DlName NVARCHAR(100),

    -- Section 8 (File Paths)
    AadhaarFilePath NVARCHAR(500),
    PanFilePath NVARCHAR(500),
    PassportDlFilePath NVARCHAR(500),
    AddressProofFilePath NVARCHAR(500),
    SignatureFilePath NVARCHAR(500),

    Status NVARCHAR(20) DEFAULT 'Pending',

    SubmissionDate DATETIME DEFAULT GETDATE()
);
GO

PRINT 'Tables created successfully!';
PRINT 'Verifying data...';

SELECT 'AccountTypes' AS TableName, COUNT(*) AS RecordCount FROM AccountTypes
UNION ALL
SELECT 'Genders', COUNT(*) FROM Genders
UNION ALL
SELECT 'Branches', COUNT(*) FROM Branches;
