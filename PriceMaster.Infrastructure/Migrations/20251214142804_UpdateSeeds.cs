using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 50,
                column: "ComponentName",
                value: "Copper coin (n3)");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 51,
                column: "ComponentName",
                value: "Copper coin (n2)");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 52,
                column: "ComponentName",
                value: "Copper coin (n1)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "SeriesId",
                keyValue: 2,
                column: "SeriesName",
                value: "UNR 1917-1921. The Ukrainian Liberation struggle.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 50,
                column: "ComponentName",
                value: "Cooper coin (n3)");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 51,
                column: "ComponentName",
                value: "Cooper coin (n2)");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "ComponentId",
                keyValue: 52,
                column: "ComponentName",
                value: "Cooper coin (n1)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "SeriesId",
                keyValue: 2,
                column: "SeriesName",
                value: "UNR 1917-1921. The Ukrainian Liberation truggle.");
        }
    }
}
