using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrandeTravel.Migrations
{
    public partial class updateProviderDatabaseMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "tblProvider");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "tblProvider");

            migrationBuilder.AddColumn<int>(
                name: "ProviderUserId",
                table: "tblProvider",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderUserId",
                table: "tblProvider");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "tblProvider",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "tblProvider",
                nullable: true);
        }
    }
}
