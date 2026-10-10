using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureVolunteerProfileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Volunteers', 'OrganizationId') IS NULL
                BEGIN
                    ALTER TABLE Volunteers ADD OrganizationId uniqueidentifier NULL;

                    IF NOT EXISTS (SELECT 1 FROM Organizations WHERE IsDeleted = 0)
                    BEGIN
                        INSERT INTO Organizations (Id, Name, IsActive, IsDeleted, CreatedAt)
                        VALUES (NEWID(), N'Hope Donation Center', 1, 0, GETUTCDATE());
                    END

                    UPDATE v
                    SET v.OrganizationId = o.Id
                    FROM Volunteers v
                    CROSS APPLY (
                        SELECT TOP 1 Id FROM Organizations WHERE IsDeleted = 0 ORDER BY CreatedAt
                    ) o
                    WHERE v.OrganizationId IS NULL;

                    ALTER TABLE Volunteers ALTER COLUMN OrganizationId uniqueidentifier NOT NULL;

                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Volunteers_Organizations_OrganizationId')
                    BEGIN
                        ALTER TABLE Volunteers WITH CHECK
                        ADD CONSTRAINT FK_Volunteers_Organizations_OrganizationId
                        FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id);
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Volunteers_OrganizationId' AND object_id = OBJECT_ID('Volunteers'))
                    BEGIN
                        CREATE INDEX IX_Volunteers_OrganizationId ON Volunteers(OrganizationId);
                    END
                END

                IF COL_LENGTH('Volunteers', 'Days') IS NULL
                    ALTER TABLE Volunteers ADD Days nvarchar(500) NOT NULL CONSTRAINT DF_Volunteers_Days DEFAULT(N'');

                IF COL_LENGTH('Volunteers', 'HoursCount') IS NULL
                    ALTER TABLE Volunteers ADD HoursCount int NOT NULL CONSTRAINT DF_Volunteers_HoursCount DEFAULT(0);

                IF COL_LENGTH('Volunteers', 'Hobbies') IS NULL
                    ALTER TABLE Volunteers ADD Hobbies nvarchar(500) NULL;

                IF COL_LENGTH('Volunteers', 'Skills') IS NULL
                    ALTER TABLE Volunteers ADD Skills nvarchar(500) NULL;

                IF COL_LENGTH('Volunteers', 'Experiences') IS NULL
                    ALTER TABLE Volunteers ADD Experiences nvarchar(1000) NULL;

                IF COL_LENGTH('Volunteers', 'NeglectedTasksCount') IS NULL
                    ALTER TABLE Volunteers ADD NeglectedTasksCount int NOT NULL CONSTRAINT DF_Volunteers_NeglectedTasksCount DEFAULT(0);

                IF COL_LENGTH('Volunteers', 'IsDeleted') IS NULL
                    ALTER TABLE Volunteers ADD IsDeleted bit NOT NULL CONSTRAINT DF_Volunteers_IsDeleted DEFAULT(0);

                IF COL_LENGTH('Volunteers', 'DeletedAt') IS NULL
                    ALTER TABLE Volunteers ADD DeletedAt datetime2 NULL;

                IF EXISTS (
                    SELECT 1
                    FROM sys.columns c
                    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID('Volunteers')
                      AND c.name = 'Status'
                      AND t.name IN ('int', 'tinyint', 'smallint')
                )
                BEGIN
                    ALTER TABLE Volunteers ADD StatusText nvarchar(20) NOT NULL CONSTRAINT DF_Volunteers_StatusText DEFAULT(N'Active');

                    EXEC(N'
                        UPDATE Volunteers SET StatusText = CASE Status
                            WHEN 1 THEN N''Pending''
                            WHEN 2 THEN N''Active''
                            WHEN 3 THEN N''Inactive''
                            WHEN 4 THEN N''Suspended''
                            ELSE N''Active''
                        END;
                    ');

                    ALTER TABLE Volunteers DROP COLUMN Status;
                    EXEC sp_rename 'Volunteers.StatusText', 'Status', 'COLUMN';
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: safety-net migration.
        }
    }
}
