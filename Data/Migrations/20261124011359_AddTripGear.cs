using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTripGear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConsumable",
                schema: "public",
                table: "gear",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "trip_gear",
                schema: "public",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "integer", nullable: false),
                    GearId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    IsPacked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_gear", x => new { x.TripId, x.GearId });
                    table.CheckConstraint("CK_trip_gear_quantity", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_trip_gear_gear_GearId",
                        column: x => x.GearId,
                        principalSchema: "public",
                        principalTable: "gear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_gear_trip_TripId",
                        column: x => x.TripId,
                        principalSchema: "public",
                        principalTable: "trip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_gear_GearId",
                schema: "public",
                table: "trip_gear",
                column: "GearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_gear",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "IsConsumable",
                schema: "public",
                table: "gear");
        }
    }
}
