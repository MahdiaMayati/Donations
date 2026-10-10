using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FamilyMemberClothingSizeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClothingSizeTmp",
                table: "FamilyMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE FamilyMembers
                SET ClothingSizeTmp = CASE WHEN ClothingSize = 1 THEN N'M' ELSE N'S' END;
                """);

            migrationBuilder.DropColumn(
                name: "ClothingSize",
                table: "FamilyMembers");

            migrationBuilder.RenameColumn(
                name: "ClothingSizeTmp",
                table: "FamilyMembers",
                newName: "ClothingSize");

            migrationBuilder.AlterColumn<string>(
                name: "ClothingSize",
                table: "FamilyMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ClothingSizeTmp",
                table: "FamilyMembers",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE FamilyMembers
                SET ClothingSizeTmp = CASE
                    WHEN ClothingSize IN (N'M', N'L', N'XL', N'XXL', N'XXXL') THEN 1
                    ELSE 0
                END;
                """);

            migrationBuilder.DropColumn(
                name: "ClothingSize",
                table: "FamilyMembers");

            migrationBuilder.RenameColumn(
                name: "ClothingSizeTmp",
                table: "FamilyMembers",
                newName: "ClothingSize");

            migrationBuilder.AlterColumn<bool>(
                name: "ClothingSize",
                table: "FamilyMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
