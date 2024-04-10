using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingCategoryAndLocationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentString",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "AddressCity",
                table: "Locations");

            migrationBuilder.AddColumn<int>(
                name: "DeparmentId",
                table: "ProviderProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "userFullName",
                table: "ProviderProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "workingEndHours",
                table: "ProviderProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "workingStartHours",
                table: "ProviderProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationType",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderFullName",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderProfile = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressCity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "LocationTypes");

            migrationBuilder.DropColumn(
                name: "DeparmentId",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "userFullName",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "workingEndHours",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "workingStartHours",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "LocationType",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "ProviderFullName",
                table: "Availabilities");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentString",
                table: "ProviderProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressCity",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
