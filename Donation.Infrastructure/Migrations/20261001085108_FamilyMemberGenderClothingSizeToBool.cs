using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FamilyMemberGenderClothingSizeToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Normalize enum ints before converting to bit (true=Male/has size flag).
            migrationBuilder.Sql("""
                UPDATE FamilyMembers SET Gender = CASE WHEN Gender = 1 THEN 1 ELSE 0 END;
                UPDATE FamilyMembers SET ClothingSize = CASE WHEN ClothingSize <> 0 THEN 1 ELSE 0 END;
                """);

            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "FamilyMembers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "ClothingSize",
                table: "FamilyMembers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "FamilyMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ClothingSize",
                table: "FamilyMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            // Restore previous enum-ish values: Male=1 / Female=2; ClothingSize true->M(3).
            migrationBuilder.Sql("""
                UPDATE FamilyMembers SET Gender = CASE WHEN Gender = 1 THEN 1 ELSE 2 END;
                UPDATE FamilyMembers SET ClothingSize = CASE WHEN ClothingSize = 1 THEN 3 ELSE 2 END;
                """);
        }
    }
}
