using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEntityIdsToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FKs that depend on int PKs / int FK columns.
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyMembers_Beneficiaries_BeneficiaryId",
                table: "FamilyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Beneficiaries_Addresses_AddressId",
                table: "Beneficiaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Cities_CityId",
                table: "Areas");

            // --- FamilyMembers ---
            migrationBuilder.DropPrimaryKey(name: "PK_FamilyMembers", table: "FamilyMembers");
            migrationBuilder.DropIndex(name: "IX_FamilyMembers_BeneficiaryId", table: "FamilyMembers");
            migrationBuilder.DropColumn(name: "Id", table: "FamilyMembers");
            migrationBuilder.DropColumn(name: "BeneficiaryId", table: "FamilyMembers");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FamilyMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddColumn<Guid>(
                name: "BeneficiaryId",
                table: "FamilyMembers",
                type: "uniqueidentifier",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_FamilyMembers", table: "FamilyMembers", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_FamilyMembers_BeneficiaryId",
                table: "FamilyMembers",
                column: "BeneficiaryId");

            // --- Beneficiaries ---
            migrationBuilder.DropPrimaryKey(name: "PK_Beneficiaries", table: "Beneficiaries");
            migrationBuilder.DropIndex(name: "IX_Beneficiaries_AddressId", table: "Beneficiaries");
            migrationBuilder.DropColumn(name: "Id", table: "Beneficiaries");
            migrationBuilder.DropColumn(name: "AddressId", table: "Beneficiaries");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Beneficiaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Beneficiaries",
                type: "uniqueidentifier",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Beneficiaries", table: "Beneficiaries", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_AddressId",
                table: "Beneficiaries",
                column: "AddressId");

            // --- Addresses ---
            migrationBuilder.DropPrimaryKey(name: "PK_Addresses", table: "Addresses");
            migrationBuilder.DropIndex(name: "IX_Addresses_AreaId", table: "Addresses");
            migrationBuilder.DropColumn(name: "Id", table: "Addresses");
            migrationBuilder.DropColumn(name: "AreaId", table: "Addresses");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddColumn<Guid>(
                name: "AreaId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Addresses", table: "Addresses", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AreaId",
                table: "Addresses",
                column: "AreaId");

            // --- Areas ---
            migrationBuilder.DropPrimaryKey(name: "PK_Areas", table: "Areas");
            migrationBuilder.DropIndex(name: "IX_Areas_CityId_Name", table: "Areas");
            migrationBuilder.DropColumn(name: "Id", table: "Areas");
            migrationBuilder.DropColumn(name: "CityId", table: "Areas");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Areas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "Areas",
                type: "uniqueidentifier",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Areas", table: "Areas", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Areas_CityId_Name",
                table: "Areas",
                columns: new[] { "CityId", "Name" });

            // --- Cities ---
            migrationBuilder.DropPrimaryKey(name: "PK_Cities", table: "Cities");
            migrationBuilder.DropColumn(name: "Id", table: "Cities");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddPrimaryKey(name: "PK_Cities", table: "Cities", column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Cities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // --- Donors ---
            migrationBuilder.DropPrimaryKey(name: "PK_Donors", table: "Donors");
            migrationBuilder.DropColumn(name: "Id", table: "Donors");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Donors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddPrimaryKey(name: "PK_Donors", table: "Donors", column: "Id");

            // --- Volunteers ---
            migrationBuilder.DropPrimaryKey(name: "PK_Volunteers", table: "Volunteers");
            migrationBuilder.DropColumn(name: "Id", table: "Volunteers");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Volunteers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");
            migrationBuilder.AddPrimaryKey(name: "PK_Volunteers", table: "Volunteers", column: "Id");

            // Already-Guid PKs: add SQL Server default only.
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Organizations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Restore FKs with Guid types.
            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Cities_CityId",
                table: "Areas",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Addresses_AddressId",
                table: "Beneficiaries",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyMembers_Beneficiaries_BeneficiaryId",
                table: "FamilyMembers",
                column: "BeneficiaryId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyMembers_Beneficiaries_BeneficiaryId",
                table: "FamilyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Beneficiaries_Addresses_AddressId",
                table: "Beneficiaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Cities_CityId",
                table: "Areas");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Organizations",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            // Volunteers
            migrationBuilder.DropPrimaryKey(name: "PK_Volunteers", table: "Volunteers");
            migrationBuilder.DropColumn(name: "Id", table: "Volunteers");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Volunteers",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey(name: "PK_Volunteers", table: "Volunteers", column: "Id");

            // Donors
            migrationBuilder.DropPrimaryKey(name: "PK_Donors", table: "Donors");
            migrationBuilder.DropColumn(name: "Id", table: "Donors");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Donors",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey(name: "PK_Donors", table: "Donors", column: "Id");

            // Cities
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.DropPrimaryKey(name: "PK_Cities", table: "Cities");
            migrationBuilder.DropColumn(name: "Id", table: "Cities");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Cities",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey(name: "PK_Cities", table: "Cities", column: "Id");

            // Areas
            migrationBuilder.DropPrimaryKey(name: "PK_Areas", table: "Areas");
            migrationBuilder.DropIndex(name: "IX_Areas_CityId_Name", table: "Areas");
            migrationBuilder.DropColumn(name: "Id", table: "Areas");
            migrationBuilder.DropColumn(name: "CityId", table: "Areas");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Areas",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Areas",
                type: "int",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Areas", table: "Areas", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Areas_CityId_Name",
                table: "Areas",
                columns: new[] { "CityId", "Name" });

            // Addresses
            migrationBuilder.DropPrimaryKey(name: "PK_Addresses", table: "Addresses");
            migrationBuilder.DropIndex(name: "IX_Addresses_AreaId", table: "Addresses");
            migrationBuilder.DropColumn(name: "Id", table: "Addresses");
            migrationBuilder.DropColumn(name: "AreaId", table: "Addresses");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Addresses",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Addresses",
                type: "int",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Addresses", table: "Addresses", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AreaId",
                table: "Addresses",
                column: "AreaId");

            // Beneficiaries
            migrationBuilder.DropPrimaryKey(name: "PK_Beneficiaries", table: "Beneficiaries");
            migrationBuilder.DropIndex(name: "IX_Beneficiaries_AddressId", table: "Beneficiaries");
            migrationBuilder.DropColumn(name: "Id", table: "Beneficiaries");
            migrationBuilder.DropColumn(name: "AddressId", table: "Beneficiaries");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Beneficiaries",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Beneficiaries",
                type: "int",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_Beneficiaries", table: "Beneficiaries", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_AddressId",
                table: "Beneficiaries",
                column: "AddressId");

            // FamilyMembers
            migrationBuilder.DropPrimaryKey(name: "PK_FamilyMembers", table: "FamilyMembers");
            migrationBuilder.DropIndex(name: "IX_FamilyMembers_BeneficiaryId", table: "FamilyMembers");
            migrationBuilder.DropColumn(name: "Id", table: "FamilyMembers");
            migrationBuilder.DropColumn(name: "BeneficiaryId", table: "FamilyMembers");
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FamilyMembers",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryId",
                table: "FamilyMembers",
                type: "int",
                nullable: false);
            migrationBuilder.AddPrimaryKey(name: "PK_FamilyMembers", table: "FamilyMembers", column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_FamilyMembers_BeneficiaryId",
                table: "FamilyMembers",
                column: "BeneficiaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Cities_CityId",
                table: "Areas",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Addresses_AddressId",
                table: "Beneficiaries",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyMembers_Beneficiaries_BeneficiaryId",
                table: "FamilyMembers",
                column: "BeneficiaryId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
