using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesName = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.SeriesId);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitName = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.UnitId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeWidth = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    SizeHeight = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    RecommendedPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComponentName = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_Components_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Components_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionHistories",
                columns: table => new
                {
                    ProductionHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RecommendedPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    WorkCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionHistories", x => x.ProductionHistoryId);
                    table.ForeignKey(
                        name: "FK_ProductionHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BomItems",
                columns: table => new
                {
                    BomItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BomItems", x => x.BomItemId);
                    table.ForeignKey(
                        name: "FK_BomItems_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BomItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Artifact" },
                    { 2, "BaseMaterial" },
                    { 3, "AssemblyWork" }
                });

            migrationBuilder.InsertData(
                table: "Series",
                columns: new[] { "SeriesId", "SeriesName" },
                values: new object[,]
                {
                    { 1, "Cossacks. Birth Of Liberty." },
                    { 2, "UNR 1917-1921. The Ukrainian Liberation struggle." },
                    { 3, "Kyivan Rus. The Golden Legacy." }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "UnitId", "UnitName" },
                values: new object[,]
                {
                    { 1, "piece" },
                    { 2, "sq.m" }
                });

            migrationBuilder.InsertData(
                table: "Components",
                columns: new[] { "ComponentId", "CategoryId", "ComponentName", "PricePerUnit", "UnitId" },
                values: new object[,]
                {
                    { 1, 1, "Button", 5m, 1 },
                    { 2, 1, "Ring", 5m, 1 },
                    { 3, 1, "Nail", 5m, 1 },
                    { 4, 1, "Harness Component (simple)", 10m, 1 },
                    { 5, 1, "Harness Component (stars)", 20m, 1 },
                    { 6, 1, "Handle", 50m, 1 },
                    { 50, 1, "Copper coin (n3)", 5m, 1 },
                    { 51, 1, "Copper coin (n2)", 10m, 1 },
                    { 52, 1, "Copper coin (n1)", 20m, 1 },
                    { 53, 1, "Silver coin (copy)", 72m, 1 },
                    { 90, 1, "Spherical ball", 5m, 1 },
                    { 110, 1, "Inkwell", 30m, 1 },
                    { 111, 1, "Tobacco pipe", 25m, 1 },
                    { 120, 1, "Flint", 80m, 1 },
                    { 500, 2, "Baguette 60*30", 661m, 1 },
                    { 900, 2, "Printout 110", 71m, 1 },
                    { 901, 2, "Printout 150", 71m, 1 },
                    { 950, 2, "Hardware", 25m, 1 },
                    { 961, 2, "Textile, per sq.m.", 150m, 2 },
                    { 962, 2, "Polyurethane, per sq.m.", 85m, 2 },
                    { 1000, 3, "Work", 800m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ComponentId",
                table: "BomItems",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ProductId",
                table: "BomItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Components_CategoryId",
                table: "Components",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentName",
                table: "Components",
                column: "ComponentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Components_UnitId",
                table: "Components",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionHistories_ProductId",
                table: "ProductionHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SeriesId",
                table: "Products",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_SeriesName",
                table: "Series",
                column: "SeriesName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitName",
                table: "Units",
                column: "UnitName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BomItems");

            migrationBuilder.DropTable(
                name: "ProductionHistories");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Series");
        }
    }
}
