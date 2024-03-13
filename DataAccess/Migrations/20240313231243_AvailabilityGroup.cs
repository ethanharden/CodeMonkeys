using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AvailabilityGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_RecurringTypes_RecurringTypeId",
                table: "Availabilities");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_RecurringTypeId",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "Recurring",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "RecurringEndDate",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "RecurringTypeId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "Attatchment",
                table: "Bookings",
                newName: "Attachment");

            migrationBuilder.AddColumn<int>(
                name: "AvailabilityId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AvailabilityGroupId",
                table: "Availabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AvailabilityGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecurringTypeId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecurringEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailabilityGroups_RecurringTypes_RecurringTypeId",
                        column: x => x.RecurringTypeId,
                        principalTable: "RecurringTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_AvailabilityGroupId",
                table: "Availabilities",
                column: "AvailabilityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityGroups_RecurringTypeId",
                table: "AvailabilityGroups",
                column: "RecurringTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_AvailabilityGroups_AvailabilityGroupId",
                table: "Availabilities",
                column: "AvailabilityGroupId",
                principalTable: "AvailabilityGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_AvailabilityGroups_AvailabilityGroupId",
                table: "Availabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "AvailabilityGroups");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_AvailabilityGroupId",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AvailabilityGroupId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "Bookings",
                newName: "Attatchment");

            migrationBuilder.AddColumn<bool>(
                name: "Recurring",
                table: "Availabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurringEndDate",
                table: "Availabilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurringTypeId",
                table: "Availabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_RecurringTypeId",
                table: "Availabilities",
                column: "RecurringTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_RecurringTypes_RecurringTypeId",
                table: "Availabilities",
                column: "RecurringTypeId",
                principalTable: "RecurringTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
