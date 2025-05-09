using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTorz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTravellerRelationToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Traveller_Bookings_BookingId",
                table: "Traveller");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Traveller",
                table: "Traveller");

            migrationBuilder.DropColumn(
                name: "TravellerId",
                table: "Traveller");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Traveller");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Traveller");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Traveller");

            migrationBuilder.RenameTable(
                name: "Traveller",
                newName: "Travellers");

            migrationBuilder.RenameIndex(
                name: "IX_Traveller_BookingId",
                table: "Travellers",
                newName: "IX_Travellers_BookingId");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Travellers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Travellers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Travellers",
                table: "Travellers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Travellers_Bookings_BookingId",
                table: "Travellers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Travellers_Bookings_BookingId",
                table: "Travellers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Travellers",
                table: "Travellers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Travellers");

            migrationBuilder.RenameTable(
                name: "Travellers",
                newName: "Traveller");

            migrationBuilder.RenameIndex(
                name: "IX_Travellers_BookingId",
                table: "Traveller",
                newName: "IX_Traveller_BookingId");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Traveller",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "TravellerId",
                table: "Traveller",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "Traveller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Traveller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Traveller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Traveller",
                table: "Traveller",
                column: "TravellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Traveller_Bookings_BookingId",
                table: "Traveller",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }
    }
}
