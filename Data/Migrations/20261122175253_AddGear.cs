using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "gear",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WeightOunces = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gear", x => x.Id);
                    table.CheckConstraint("CK_gear_category", "\"Category\" IN ('Backpack', 'Shelter', 'SleepSystem', 'Clothing', 'Footwear', 'Kitchen', 'Hydration', 'Navigation', 'Electronics', 'Safety', 'Health', 'Hygiene', 'Miscellaneous')");
                    table.CheckConstraint("CK_gear_weight_ounces", "\"WeightOunces\" >= 0");
                    table.ForeignKey(
                        name: "FK_gear_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gear_UserId",
                schema: "public",
                table: "gear",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gear",
                schema: "public");
        }
    }
}
