using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCategoryAndItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: local DBs may already have a minimal ItemTypes table
            // from earlier donation-request experiments (Id, Name, IsDeleted only).
            migrationBuilder.Sql(@"
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'[ItemCategories]', N'U') IS NULL
BEGIN
    CREATE TABLE [ItemCategories] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Name] nvarchar(200) NOT NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ItemCategories] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ItemCategories_Name' AND object_id = OBJECT_ID(N'ItemCategories'))
    CREATE UNIQUE INDEX [IX_ItemCategories_Name]
        ON [ItemCategories] ([Name])
        WHERE [IsDeleted] = 0;

IF OBJECT_ID(N'[ItemTypes]', N'U') IS NULL
BEGIN
    CREATE TABLE [ItemTypes] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [CategoryId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [OutfitUnits] decimal(18,4) NOT NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ItemTypes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ItemTypes_ItemCategories_CategoryId]
            FOREIGN KEY ([CategoryId]) REFERENCES [ItemCategories] ([Id])
    );

    CREATE INDEX [IX_ItemTypes_CategoryId] ON [ItemTypes] ([CategoryId]);
    CREATE UNIQUE INDEX [IX_ItemTypes_CategoryId_Name]
        ON [ItemTypes] ([CategoryId], [Name])
        WHERE [IsDeleted] = 0;
END
ELSE IF COL_LENGTH(N'ItemTypes', N'CategoryId') IS NULL
BEGIN
    DECLARE @DefaultCategoryId uniqueidentifier = NEWID();

    IF NOT EXISTS (SELECT 1 FROM [ItemCategories])
    BEGIN
        INSERT INTO [ItemCategories] ([Id], [Name], [IsDeleted], [CreatedAt])
        VALUES (@DefaultCategoryId, N'General', 0, SYSUTCDATETIME());
    END
    ELSE
    BEGIN
        SELECT TOP 1 @DefaultCategoryId = [Id] FROM [ItemCategories] ORDER BY [CreatedAt];
    END

    ALTER TABLE [ItemTypes] ADD [CategoryId] uniqueidentifier NULL;
    ALTER TABLE [ItemTypes] ADD [OutfitUnits] decimal(18,4) NOT NULL CONSTRAINT [DF_ItemTypes_OutfitUnits] DEFAULT (1);
    ALTER TABLE [ItemTypes] ADD [DeletedAt] datetime2 NULL;
    ALTER TABLE [ItemTypes] ADD [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_ItemTypes_CreatedAt] DEFAULT (SYSUTCDATETIME());
    ALTER TABLE [ItemTypes] ADD [UpdatedAt] datetime2 NULL;

    DECLARE @sql nvarchar(max) = N'
UPDATE [ItemTypes] SET [CategoryId] = ''' + CAST(@DefaultCategoryId AS nvarchar(36)) + N''' WHERE [CategoryId] IS NULL;
ALTER TABLE [ItemTypes] ALTER COLUMN [CategoryId] uniqueidentifier NOT NULL;
ALTER TABLE [ItemTypes] ALTER COLUMN [Name] nvarchar(200) NOT NULL;
IF OBJECT_ID(N''[FK_ItemTypes_ItemCategories_CategoryId]'', N''F'') IS NULL
  ALTER TABLE [ItemTypes] WITH CHECK ADD CONSTRAINT [FK_ItemTypes_ItemCategories_CategoryId]
    FOREIGN KEY ([CategoryId]) REFERENCES [ItemCategories] ([Id]);';
    EXEC sp_executesql @sql;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ItemTypes_CategoryId' AND object_id = OBJECT_ID(N'ItemTypes'))
        CREATE INDEX [IX_ItemTypes_CategoryId] ON [ItemTypes] ([CategoryId]);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ItemTypes_CategoryId_Name' AND object_id = OBJECT_ID(N'ItemTypes'))
        CREATE UNIQUE INDEX [IX_ItemTypes_CategoryId_Name]
            ON [ItemTypes] ([CategoryId], [Name])
            WHERE [IsDeleted] = 0;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[ItemTypes]', N'U') IS NOT NULL
    DROP TABLE [ItemTypes];

IF OBJECT_ID(N'[ItemCategories]', N'U') IS NOT NULL
    DROP TABLE [ItemCategories];
");
        }
    }
}
