using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROJEKTDB.Migrations
{
    /// <inheritdoc />
    public partial class InitGaleria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GALERI",
                columns: table => new
                {
                    GAL_ID = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    GAL_MNG = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    GAL_TEL = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    GAL_ZON = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GALERI", x => x.GAL_ID);
                });

            migrationBuilder.CreateTable(
                name: "PERSON",
                columns: table => new
                {
                    PER_ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PER_EM = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    PER_MB = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    PER_DAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PER_ZON = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    PER_TEL = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERSON", x => x.PER_ID);
                });

            migrationBuilder.CreateTable(
                name: "FATURE",
                columns: table => new
                {
                    FAT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PER_ID = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    FAT_DAT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FATURE", x => x.FAT_ID);
                    table.ForeignKey(
                        name: "FK_FATURE_PERSON_PER_ID",
                        column: x => x.PER_ID,
                        principalTable: "PERSON",
                        principalColumn: "PER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PIKTURE",
                columns: table => new
                {
                    PIK_ID = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    PIK_TIT = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    PIK_CMIM = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PER_ID = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    GAL_ID = table.Column<string>(type: "nvarchar(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PIKTURE", x => x.PIK_ID);
                    table.ForeignKey(
                        name: "FK_PIKTURE_GALERI_GAL_ID",
                        column: x => x.GAL_ID,
                        principalTable: "GALERI",
                        principalColumn: "GAL_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PIKTURE_PERSON_PER_ID",
                        column: x => x.PER_ID,
                        principalTable: "PERSON",
                        principalColumn: "PER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RRESHT",
                columns: table => new
                {
                    FAT_ID = table.Column<int>(type: "int", nullable: false),
                    RRE_ID = table.Column<byte>(type: "tinyint", nullable: false),
                    PIK_ID = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    RRE_SASI = table.Column<byte>(type: "tinyint", nullable: false),
                    RRE_CMIM = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FatureFatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RRESHT", x => new { x.FAT_ID, x.RRE_ID });
                    table.ForeignKey(
                        name: "FK_RRESHT_FATURE_FAT_ID",
                        column: x => x.FAT_ID,
                        principalTable: "FATURE",
                        principalColumn: "FAT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RRESHT_FATURE_FatureFatId",
                        column: x => x.FatureFatId,
                        principalTable: "FATURE",
                        principalColumn: "FAT_ID");
                    table.ForeignKey(
                        name: "FK_RRESHT_PIKTURE_PIK_ID",
                        column: x => x.PIK_ID,
                        principalTable: "PIKTURE",
                        principalColumn: "PIK_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FATURE_PER_ID",
                table: "FATURE",
                column: "PER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PIKTURE_GAL_ID",
                table: "PIKTURE",
                column: "GAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PIKTURE_PER_ID",
                table: "PIKTURE",
                column: "PER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RRESHT_FatureFatId",
                table: "RRESHT",
                column: "FatureFatId");

            migrationBuilder.CreateIndex(
                name: "IX_RRESHT_PIK_ID",
                table: "RRESHT",
                column: "PIK_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RRESHT");

            migrationBuilder.DropTable(
                name: "FATURE");

            migrationBuilder.DropTable(
                name: "PIKTURE");

            migrationBuilder.DropTable(
                name: "GALERI");

            migrationBuilder.DropTable(
                name: "PERSON");
        }
    }
}
