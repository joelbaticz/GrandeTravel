using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrandeTravel.Migrations
{
    public partial class updateBookingDatabaseMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "tblBooking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageName",
                table: "tblBooking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "tblBooking");

            migrationBuilder.DropColumn(
                name: "PackageName",
                table: "tblBooking");
        }
    }
}
