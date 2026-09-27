CREATE DATABASE DB_Prokofev_Bank;
GO

USE DB_Prokofev_Bank;
GO

CREATE TABLE Users
(
    ID       int IDENTITY(1,1) NOT NULL,
    Login    nvarchar(50)      NOT NULL,
    Password nvarchar(100)     NOT NULL,
    Role     nvarchar(20)      NOT NULL,
    FIO      nvarchar(100)     NOT NULL,
    Photo    nvarchar(MAX)     NULL,
    CONSTRAINT PK_Users PRIMARY KEY (ID),
    CONSTRAINT UQ_Users_Login UNIQUE (Login),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('User', 'Admin'))
);
GO

CREATE TABLE Branch
(
    ID      int IDENTITY(1,1) NOT NULL,
    Name    nvarchar(100)     NOT NULL,
    Address nvarchar(200)     NOT NULL,
    Phone   char(11)          NOT NULL,
    CONSTRAINT PK_Branch PRIMARY KEY (ID),
    CONSTRAINT CK_Branch_Phone CHECK (LEN(Phone) = 11 AND Phone NOT LIKE '%[^0-9]%')
);
GO

CREATE TABLE Account
(
    ID       int IDENTITY(1,1) NOT NULL,
    UserID   int               NOT NULL,
    BranchID int               NOT NULL,
    Number   char(20)          NOT NULL,
    Balance  decimal(18,2)     NOT NULL,
    OpenDate date              NOT NULL,
    CONSTRAINT PK_Account PRIMARY KEY (ID),
    CONSTRAINT FK_Account_Users FOREIGN KEY (UserID)
        REFERENCES Users (ID),
    CONSTRAINT FK_Account_Branch FOREIGN KEY (BranchID)
        REFERENCES Branch (ID),
    CONSTRAINT UQ_Account_Number UNIQUE (Number),
    CONSTRAINT CK_Account_Number CHECK (LEN(Number) = 20 AND Number NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_Account_Balance CHECK (Balance BETWEEN 0 AND 100000000)
);
GO
