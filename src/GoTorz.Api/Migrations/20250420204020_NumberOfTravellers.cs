using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTorz.Api.Migrations
{
    /// <inheritdoc />
    public partial class NumberOfTravellers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfTravellers",
                table: "TravelPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTravellers",
                table: "TravelPackages");
        }
    }
}
