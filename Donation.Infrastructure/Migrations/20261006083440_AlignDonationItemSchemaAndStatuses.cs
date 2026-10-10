using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignDonationItemSchemaAndStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_Barcode",
                table: "Items");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE i
SET i.OrganizationId = dr.OrganizationId
FROM [Items] i
INNER JOIN [DonationRequests] dr ON dr.Id = i.DonationRequestId
WHERE i.OrganizationId IS NULL;

UPDATE [DonationRequests]
SET [Status] = CASE [Status]
    WHEN N'Reviewed' THEN N'Scheduled'
    WHEN N'Accepted' THEN N'Scheduled'
    WHEN N'Completed' THEN N'Sorted'
    ELSE [Status]
END;

UPDATE [Items]
SET [TargetGender] = CASE
    WHEN [TargetGender] IN (N'Men', N'Women', N'Kids', N'Unisex') THEN [TargetGender]
    WHEN LOWER([TargetGender]) IN (N'male', N'man') THEN N'Men'
    WHEN LOWER([TargetGender]) IN (N'female', N'woman') THEN N'Women'
    ELSE N'Unisex'
END,
[AgeGroup] = CASE
    WHEN [AgeGroup] IN (N'Infant', N'Child', N'Teen', N'Adult') THEN [AgeGroup]
    ELSE N'Adult'
END,
[Season] = CASE
    WHEN [Season] IN (N'Winter', N'Summer', N'AllSeasons') THEN [Season]
    ELSE N'AllSeasons'
END,
[Condition] = CASE
    WHEN [Condition] IN (N'New', N'Used', N'NeedsRepair', N'Unfit') THEN [Condition]
    ELSE N'Used'
END,
[SortingStatus] = CASE
    WHEN [SortingStatus] IN (N'PendingReview', N'SuitableForDistribution', N'NeedsRepairOrWash', N'Unfit') THEN [SortingStatus]
    ELSE N'PendingReview'
END,
[AvailabilityStatus] = CASE
    WHEN [AvailabilityStatus] IN (N'PendingReview', N'Available', N'Reserved', N'Distributed', N'Unfit') THEN [AvailabilityStatus]
    ELSE N'PendingReview'
END;
");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Barcode",
                table: "Items",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Items_OrganizationId",
                table: "Items",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Organizations_OrganizationId",
                table: "Items",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Organizations_OrganizationId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Barcode",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_OrganizationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Items");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Barcode",
                table: "Items",
                column: "Barcode");
        }
    }
}
