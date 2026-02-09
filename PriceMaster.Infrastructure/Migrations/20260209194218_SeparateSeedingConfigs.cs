using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeparateSeedingConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 1000,
                column: "CategoryId",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 1000,
                column: "CategoryId",
                value: 3);
        }
    }
}
