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

INSERT INTO Branch (Name, Address, Phone) VALUES
(N'Центральное отделение',    N'г. Москва, ул. Тверская, д. 12',          '74952301415'),
(N'Отделение «Арбатское»',    N'г. Москва, ул. Новый Арбат, д. 21',       '74952912603'),
(N'Отделение «Таганское»',    N'г. Москва, ул. Таганская, д. 31/22',      '74959117420'),
(N'Отделение «Сокольники»',   N'г. Москва, ул. Русаковская, д. 8',        '74952648831'),
(N'Отделение «Южное»',        N'г. Москва, Варшавское шоссе, д. 87',      '74953175562'),
(N'Отделение «Химкинское»',   N'г. Химки, ул. Московская, д. 15',         '74955720948');
GO

DECLARE @AdminHash nvarchar(100) = CONVERT(nvarchar(100), HASHBYTES('SHA1', 'Admin123'), 2);
DECLARE @ClientHash nvarchar(100) = CONVERT(nvarchar(100), HASHBYTES('SHA1', 'Client123'), 2);

INSERT INTO Users (Login, Password, Role, FIO, Photo) VALUES
('admin',      @AdminHash,  'Admin', N'Смирнова Ольга Викторовна',     NULL),
('admin2',     @AdminHash,  'Admin', N'Белов Андрей Николаевич',       NULL),
('ivanov',     @ClientHash, 'User',  N'Иванов Сергей Петрович',        NULL),
('kuznetsova', @ClientHash, 'User',  N'Кузнецова Анна Михайловна',     NULL),
('petrov',     @ClientHash, 'User',  N'Петров Дмитрий Александрович',  NULL),
('sokolova',   @ClientHash, 'User',  N'Соколова Елена Игоревна',       NULL),
('morozov',    @ClientHash, 'User',  N'Морозов Алексей Владимирович',  NULL),
('volkova',    @ClientHash, 'User',  N'Волкова Мария Сергеевна',       NULL),
('lebedev',    @ClientHash, 'User',  N'Лебедев Игорь Олегович',        NULL),
('novikova',   @ClientHash, 'User',  N'Новикова Татьяна Андреевна',    NULL),
('fedorov',    @ClientHash, 'User',  N'Фёдоров Павел Викторович',      NULL),
('orlova',     @ClientHash, 'User',  N'Орлова Наталья Дмитриевна',     NULL),
('zaitsev',    @ClientHash, 'User',  N'Зайцев Максим Евгеньевич',      NULL);
GO

INSERT INTO Account (UserID, BranchID, Number, Balance, OpenDate) VALUES
(3,  1, '40817810500010000137',  154320.50, '2018-03-15'),
(3,  2, '40817810200020000214',   12500.00, '2020-07-02'),
(3,  5, '40817810800050000391',       0.00, '2024-11-20'),
(4,  1, '40817810100010000245',   87340.75, '2019-05-28'),
(4,  3, '40817810700030000118', 1250000.00, '2022-02-14'),
(5,  2, '40817810900020000376',  342100.00, '2018-09-10'),
(5,  4, '40817810400040000052',    5600.30, '2021-04-19'),
(5,  5, '40817810600050000127', 2875400.00, '2023-08-01'),
(5,  6, '40817810300060000089',   46000.00, '2026-03-12'),
(6,  3, '40817810500030000263',  198750.20, '2020-12-03'),
(7,  1, '40817810800010000318',  730000.00, '2019-10-22'),
(7,  4, '40817810200040000171',   15420.90, '2023-01-17'),
(7,  6, '40817810600060000144', 3500000.00, '2025-06-30'),
(8,  2, '40817810400020000425',   64300.00, '2021-08-09'),
(8,  5, '40817810100050000208',  410250.45, '2024-02-27'),
(9,  1, '40817810300010000402',  925000.00, '2018-06-05'),
(9,  3, '40817810900030000337',    1840.00, '2020-03-23'),
(9,  4, '40817810700040000296',  268900.10, '2022-10-11'),
(9,  6, '40817810500060000215', 4750000.00, '2026-07-08'),
(10, 5, '40817810700050000452',   39800.00, '2019-11-14'),
(10, 6, '40817810200060000307', 1120500.00, '2023-05-25'),
(11, 2, '40817810600020000519',  257300.80, '2021-01-29'),
(12, 1, '40817810400010000526',   18900.00, '2018-12-18'),
(12, 3, '40817810100030000481',  640000.00, '2022-06-07'),
(12, 5, '40817810900050000533', 2100000.00, '2025-09-16'),
(13, 4, '40817810800040000384',   97350.60, '2020-08-31'),
(13, 6, '40817810400060000398',       0.00, '2026-01-20');
GO
