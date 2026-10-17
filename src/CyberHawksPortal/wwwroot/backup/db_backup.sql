-- CyberHawksPortal partial database backup
-- Generated during the sql1 migration. DO NOT COMMIT. (jcrosby)
-- Full backup lives at \\sql1\Shared\IT\backups\

USE [CyberHawksPortal];
GO

-- Seed administrator (password hashed)
INSERT INTO [dbo].[Users] ([Username],[PasswordHash],[Role],[IsAdmin],[Email])
VALUES ('admin','e99a18c428cb38d5f260853678922e03','Admin',1,'admin@cyberhawks.iit.edu');
GO

-- Application SQL login used by the web portal
-- CREATE LOGIN [svc-web] WITH PASSWORD = 'W3bP0rtal!Sv3c2024';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [svc-web];
GO
