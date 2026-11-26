using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTripFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trip_food",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeightOunces = table.Column<decimal>(type: "numeric", nullable: false),
                    Calories = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_food", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trip_food_trip_TripId",
                        column: x => x.TripId,
                        principalSchema: "public",
                        principalTable: "trip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_food_TripId",
                schema: "public",
                table: "trip_food",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_food",
                schema: "public");
        }
    }
}
