CREATE DATABASE AttendanceDB;
GO
USE AttendanceDB;
GO
CREATE TABLE Students (
    StudentId             INT IDENTITY(1,1) PRIMARY KEY,
    StudentName           NVARCHAR(100) NOT NULL,
    RegistrationNumber    NVARCHAR(30)  NOT NULL,
    Department            NVARCHAR(50)  NOT NULL,
    AttendancePercentage  DECIMAL(5,2)  NOT NULL
);
GO