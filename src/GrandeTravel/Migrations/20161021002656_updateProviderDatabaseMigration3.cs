using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrandeTravel.Migrations
{
    public partial class updateProviderDatabaseMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderUserId",
                table: "tblProvider");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tblProvider",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tblProvider");

            migrationBuilder.AddColumn<int>(
                name: "ProviderUserId",
                table: "tblProvider",
                nullable: false,
                defaultValue: 0);
        }
    }
}
