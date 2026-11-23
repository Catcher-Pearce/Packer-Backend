using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packer.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.users;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The legacy table was outside the EF model and may never have existed.
            // Rolling back cannot restore its deleted rows.
        }
    }
}
