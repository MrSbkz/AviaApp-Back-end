using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class BookingAddCabinClassId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CabinClassId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CabinClassId",
                table: "Bookings",
                column: "CabinClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_CabinClasses_CabinClassId",
                table: "Bookings",
                column: "CabinClassId",
                principalTable: "CabinClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_CabinClasses_CabinClassId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CabinClassId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CabinClassId",
                table: "Bookings");
        }
    }
}
