using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTripName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "trip",
                type: "text",
                nullable: false,
                defaultValue: "Unnamed trip");

            migrationBuilder.Sql("""
                ALTER TABLE public.trip ALTER COLUMN "Name" DROP DEFAULT;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "trip");
        }
    }
}
