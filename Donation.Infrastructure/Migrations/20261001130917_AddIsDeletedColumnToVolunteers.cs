using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedColumnToVolunteers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Separate batches so SQL Server can resolve IsDeleted after it is added.
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Volunteers', 'IsDeleted') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Volunteers]
                    ADD [IsDeleted] bit NOT NULL
                        CONSTRAINT [DF_Volunteers_IsDeleted] DEFAULT (CONVERT([bit],(0)));
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Volunteers', 'DeletedAt') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Volunteers]
                    ADD [DeletedAt] datetime2 NULL;
                END
                """);

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns c
                    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID(N'dbo.Volunteers')
                      AND c.name = N'Status'
                      AND t.name = N'int')
                BEGIN
                    ALTER TABLE [dbo].[Volunteers] ALTER COLUMN [Status] nvarchar(50) NOT NULL;
                END
                """);

            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Volunteers_UserId' AND object_id = OBJECT_ID(N'dbo.Volunteers'))
                BEGIN
                    DROP INDEX [IX_Volunteers_UserId] ON [dbo].[Volunteers];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Volunteers', 'IsDeleted') IS NOT NULL
                   AND NOT EXISTS (
                       SELECT 1 FROM sys.indexes
                       WHERE name = N'IX_Volunteers_UserId' AND object_id = OBJECT_ID(N'dbo.Volunteers'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Volunteers_UserId]
                    ON [dbo].[Volunteers]([UserId])
                    WHERE [IsDeleted] = 0;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Volunteers_UserId' AND object_id = OBJECT_ID(N'dbo.Volunteers'))
                BEGIN
                    DROP INDEX [IX_Volunteers_UserId] ON [dbo].[Volunteers];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Volunteers', 'DeletedAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[Volunteers] DROP COLUMN [DeletedAt];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Volunteers', 'IsDeleted') IS NOT NULL
                BEGIN
                    DECLARE @df sysname;
                    SELECT @df = dc.name
                    FROM sys.default_constraints dc
                    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Volunteers') AND c.name = N'IsDeleted';

                    IF @df IS NOT NULL
                        EXEC(N'ALTER TABLE [dbo].[Volunteers] DROP CONSTRAINT [' + @df + N']');

                    ALTER TABLE [dbo].[Volunteers] DROP COLUMN [IsDeleted];
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Volunteers_UserId' AND object_id = OBJECT_ID(N'dbo.Volunteers'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Volunteers_UserId]
                    ON [dbo].[Volunteers]([UserId]);
                END
                """);
        }
    }
}
