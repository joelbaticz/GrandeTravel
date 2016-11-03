using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GrandeTravel.Migrations
{
    public partial class addingBookingDatabaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblBooking",
                columns: table => new
                {
                    BookingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(nullable: false),
                    DateFor = table.Column<DateTime>(nullable: false),
                    DateMade = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    NumberOfPeople = table.Column<int>(nullable: false),
                    PackageId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    SpecialRequirements = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBooking", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_tblBooking_tblCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBooking_CustomerId",
                table: "tblBooking",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBooking");
        }
    }
}
