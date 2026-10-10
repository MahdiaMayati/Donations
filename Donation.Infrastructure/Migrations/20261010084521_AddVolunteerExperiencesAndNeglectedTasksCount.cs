using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerExperiencesAndNeglectedTasksCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Experiences",
                table: "Volunteers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeglectedTasksCount",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experiences",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "NeglectedTasksCount",
                table: "Volunteers");
        }
    }
}
