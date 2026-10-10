using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandVolunteerProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers");

            // Status: int enum -> nvarchar (map known values, default Active).
            migrationBuilder.AddColumn<string>(
                name: "StatusText",
                table: "Volunteers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.Sql("""
                UPDATE Volunteers SET StatusText = CASE Status
                    WHEN 1 THEN 'Pending'
                    WHEN 2 THEN 'Active'
                    WHEN 3 THEN 'Inactive'
                    WHEN 4 THEN 'Suspended'
                    ELSE 'Active'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Volunteers");

            migrationBuilder.RenameColumn(
                name: "StatusText",
                table: "Volunteers",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Volunteers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<string>(
                name: "Days",
                table: "Volunteers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Volunteers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hobbies",
                table: "Volunteers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoursCount",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Volunteers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Volunteers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Volunteers",
                type: "uniqueidentifier",
                nullable: true);

            // Ensure at least one organization exists, then backfill volunteers.
            migrationBuilder.Sql("""
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
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Volunteers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_OrganizationId",
                table: "Volunteers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers",
                column: "UserId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Organizations_OrganizationId",
                table: "Volunteers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Organizations_OrganizationId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_Volunteers_OrganizationId",
                table: "Volunteers");

            migrationBuilder.DropIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Days",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Hobbies",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "HoursCount",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Volunteers");

            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.Sql("""
                UPDATE Volunteers SET StatusInt = CASE Status
                    WHEN 'Pending' THEN 1
                    WHEN 'Active' THEN 2
                    WHEN 'Inactive' THEN 3
                    WHEN 'Suspended' THEN 4
                    ELSE 2
                END;
                """);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Volunteers");

            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Volunteers",
                newName: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_UserId",
                table: "Volunteers",
                column: "UserId",
                unique: true);
        }
    }
}
