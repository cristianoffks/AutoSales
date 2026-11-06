USE [master];
GO

IF DB_ID(N'AutoSales') IS NULL
    CREATE DATABASE [AutoSales];
GO

USE [AutoSales];
GO

SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE TABLE [Brands] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_Brands] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE TABLE [People] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(150) NOT NULL,
        [Cpf] nvarchar(11) NOT NULL,
        [BirthDate] date NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_People] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE TABLE [VehicleModels] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [BrandId] int NOT NULL,
        CONSTRAINT [PK_VehicleModels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleModels_Brands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [Brands] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [Id] int NOT NULL IDENTITY,
        [Plate] nvarchar(7) NOT NULL,
        [ManufactureYear] int NOT NULL,
        [ModelId] int NOT NULL,
        [PersonId] int NOT NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vehicles_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [People] ([Id]),
        CONSTRAINT [FK_Vehicles_VehicleModels_ModelId] FOREIGN KEY ([ModelId]) REFERENCES [VehicleModels] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Brands_Name] ON [Brands] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_People_Cpf] ON [People] ([Cpf]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VehicleModels_BrandId_Name] ON [VehicleModels] ([BrandId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Vehicles_ModelId] ON [Vehicles] ([ModelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Vehicles_PersonId] ON [Vehicles] ([PersonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicles_Plate] ON [Vehicles] ([Plate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104202221_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104202221_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    ALTER TABLE [Vehicles] ADD [Brand] nvarchar(80) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    ALTER TABLE [Vehicles] ADD [Model] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    EXEC(N'
        UPDATE vehicle
        SET vehicle.Brand = brand.Name, vehicle.Model = model.Name
        FROM Vehicles vehicle
        INNER JOIN VehicleModels model ON vehicle.ModelId = model.Id
        INNER JOIN Brands brand ON model.BrandId = brand.Id;
    ');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    ALTER TABLE [Vehicles] DROP CONSTRAINT [FK_Vehicles_VehicleModels_ModelId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    DROP TABLE [VehicleModels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    DROP TABLE [Brands];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    DROP INDEX [IX_Vehicles_ModelId] ON [Vehicles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'ModelId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [ModelId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105234856_SimplifyVehicleBrandAndModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261105234856_SimplifyVehicleBrandAndModel', N'10.0.10');
END;

COMMIT;
GO

