CREATE TABLE `AspNetRoles` (
    `Id` VARCHAR(450) NOT NULL,
    `Name` VARCHAR(256) DEFAULT NULL,
    `NormalizedName` VARCHAR(256) DEFAULT NULL,
    `ConcurrencyStamp` TEXT DEFAULT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
);

CREATE TABLE `AspNetUsers` (
    `Id` VARCHAR(450) NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    `RefreshToken` TEXT DEFAULT NULL,
    `UserName` VARCHAR(256) DEFAULT NULL,
    `NormalizedUserName` VARCHAR(256) DEFAULT NULL,
    `Email` VARCHAR(256) DEFAULT NULL,
    `NormalizedEmail` VARCHAR(256) DEFAULT NULL,
    `EmailConfirmed` TINYINT(1) NOT NULL,
    `PasswordHash` TEXT DEFAULT NULL,
    `SecurityStamp` TEXT DEFAULT NULL,
    `ConcurrencyStamp` TEXT DEFAULT NULL,
    `PhoneNumber` TEXT DEFAULT NULL,
    `PhoneNumberConfirmed` TINYINT(1) NOT NULL,
    `TwoFactorEnabled` TINYINT(1) NOT NULL,
    `LockoutEnd` DATETIME DEFAULT NULL,
    `LockoutEnabled` TINYINT(1) NOT NULL,
    `AccessFailedCount` INT NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `EmailIndex` (`NormalizedEmail`),
    UNIQUE KEY `UserNameIndex` (`NormalizedUserName`)
);

CREATE TABLE `AspNetRoleClaims` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `RoleId` VARCHAR(450) NOT NULL,
    `ClaimType` TEXT DEFAULT NULL,
    `ClaimValue` TEXT DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` VARCHAR(450) NOT NULL,
    `ClaimType` TEXT DEFAULT NULL,
    `ClaimValue` TEXT DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_AspNetUserClaims_UserId` (`UserId`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` VARCHAR(450) NOT NULL,
    `ProviderKey` VARCHAR(450) NOT NULL,
    `ProviderDisplayName` TEXT DEFAULT NULL,
    `UserId` VARCHAR(450) NOT NULL,
    PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    KEY `IX_AspNetUserLogins_UserId` (`UserId`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles` (
    `UserId` VARCHAR(200) NOT NULL,
    `RoleId` VARCHAR(200) NOT NULL,
    PRIMARY KEY (`UserId`, `RoleId`),
    KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens` (
    `UserId` VARCHAR(200) NOT NULL,
    `LoginProvider` VARCHAR(200) NOT NULL,
    `Name` VARCHAR(200) NOT NULL,
    `Value` TEXT DEFAULT NULL,
    PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE Todo (
    TodoId CHAR(36) NOT NULL PRIMARY KEY,
    Day VARCHAR(255) NOT NULL,
    TodayDate DATETIME NOT NULL,
    Note VARCHAR(255) NOT NULL,
    DetailCount INT NOT NULL
);

CREATE TABLE TodoDetail (
    TodoDetailId CHAR(36) NOT NULL PRIMARY KEY,
    TodoId CHAR(36) NOT NULL,
    Activity VARCHAR(255) NOT NULL,
    Category VARCHAR(255) NOT NULL,
    DetailNote TEXT NOT NULL,
    FOREIGN KEY (TodoId) REFERENCES Todo(TodoId)
);

CREATE TABLE UserToken (
    UserId VARCHAR(255) NOT NULL PRIMARY KEY,
    Token VARCHAR(1024) NOT NULL,
    Expiry DATETIME NOT NULL
);

