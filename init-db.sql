-- init-db.sql

USE master;
GO

-- Create the scoringdb database with specific file locations in the container
CREATE DATABASE scoringdb
ON
(
    NAME = scoringdb_dat,
    FILENAME = '/var/opt/mssql/data/scoringdbdat.mdf',  -- Container file system path
    SIZE = 10MB,
    MAXSIZE = 50MB,
    FILEGROWTH = 5MB
)
LOG ON
(
    NAME = scoringdb_log,
    FILENAME = '/var/opt/mssql/data/scoringdblog.ldf',  -- Container file system path
    SIZE = 5MB,
    MAXSIZE = 25MB,
    FILEGROWTH = 5MB
);
GO

-- Ensure the SA user has full access to the database
ALTER AUTHORIZATION ON DATABASE::scoringdb TO SA;
GO
