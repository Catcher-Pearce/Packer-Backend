using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSleepingBag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_gear_category",
                schema: "public",
                table: "gear");

            migrationBuilder.Sql("""
                UPDATE public.gear SET "Category" = 'Miscellaneous'
                WHERE "Category" = 'SleepSystem';
                """);

            migrationBuilder.CreateTable(
                name: "sleeping_bag",
                schema: "public",
                columns: table => new
                {
                    GearId = table.Column<Guid>(type: "uuid", nullable: false),
                    lower_limit_temp_f = table.Column<decimal>(type: "numeric", nullable: true),
                    comfort_temp_f = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sleeping_bag", x => x.GearId);
                    table.CheckConstraint("CK_sleeping_bag_temperature_order", "lower_limit_temp_f <= comfort_temp_f");
                    table.ForeignKey(
                        name: "FK_sleeping_bag_gear_GearId",
                        column: x => x.GearId,
                        principalSchema: "public",
                        principalTable: "gear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_gear_category",
                schema: "public",
                table: "gear",
                sql: "\"Category\" IN ('Backpack', 'Shelter', 'Clothing', 'Footwear', 'Kitchen', 'Hydration', 'Navigation', 'Electronics', 'Safety', 'Health', 'Hygiene', 'Miscellaneous', 'SleepingPad', 'SleepingBag')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sleeping_bag",
                schema: "public");

            migrationBuilder.Sql("""
                UPDATE public.gear SET "Category" = 'Miscellaneous'
                WHERE "Category" IN ('SleepingPad', 'SleepingBag');
                """);

            migrationBuilder.DropCheckConstraint(
                name: "CK_gear_category",
                schema: "public",
                table: "gear");

            migrationBuilder.AddCheckConstraint(
                name: "CK_gear_category",
                schema: "public",
                table: "gear",
                sql: "\"Category\" IN ('Backpack', 'Shelter', 'SleepSystem', 'Clothing', 'Footwear', 'Kitchen', 'Hydration', 'Navigation', 'Electronics', 'Safety', 'Health', 'Hygiene', 'Miscellaneous')");
        }
    }
}
