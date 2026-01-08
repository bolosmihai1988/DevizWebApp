using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevizWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenderInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevizItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DevizId = table.Column<int>(type: "integer", nullable: false),
                    Tip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Denumire = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UM = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cantitate = table.Column<decimal>(type: "numeric", nullable: false),
                    PretUnitar = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalLinie = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevizItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevizItems_Devize_DevizId",
                        column: x => x.DevizId,
                        principalTable: "Devize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facturi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NrFactura = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClientNume = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ClientCUI = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClientAdresa = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TotalPiese = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalManopera = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGeneral = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacturaDevize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    DevizId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDevize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaDevize_Devize_DevizId",
                        column: x => x.DevizId,
                        principalTable: "Devize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacturaDevize_Facturi_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    Denumire = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UM = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cantitate = table.Column<decimal>(type: "numeric", nullable: false),
                    PretUnitar = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalLinie = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaItems_Facturi_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevizItems_DevizId",
                table: "DevizItems",
                column: "DevizId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDevize_DevizId",
                table: "FacturaDevize",
                column: "DevizId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDevize_FacturaId",
                table: "FacturaDevize",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaItems_FacturaId",
                table: "FacturaItems",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturi_NrFactura",
                table: "Facturi",
                column: "NrFactura",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevizItems");

            migrationBuilder.DropTable(
                name: "FacturaDevize");

            migrationBuilder.DropTable(
                name: "FacturaItems");

            migrationBuilder.DropTable(
                name: "Facturi");
        }
    }
}
