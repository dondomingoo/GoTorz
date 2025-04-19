using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTorz.Api.Migrations
{
    /// <inheritdoc />
    public partial class BookingFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "TravelPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TravelPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravelPackageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_TravelPackages_TravelPackageId",
                        column: x => x.TravelPackageId,
                        principalTable: "TravelPackages",
                        principalColumn: "TravelPackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Traveller",
                columns: table => new
                {
                    TravellerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traveller", x => x.TravellerId);
                    table.ForeignKey(
                        name: "FK_Traveller_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelPackageId",
                table: "Bookings",
                column: "TravelPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Traveller_BookingId",
                table: "Traveller",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Traveller");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TravelPackages");
        }
    }
}
