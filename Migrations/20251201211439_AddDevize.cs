using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevizWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDevize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCreare",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Devize");

            migrationBuilder.RenameColumn(
                name: "Descriere",
                table: "Devize",
                newName: "Telefon");

            migrationBuilder.RenameColumn(
                name: "Client",
                table: "Devize",
                newName: "SerieMotor");

            migrationBuilder.AddColumn<string>(
                name: "Adresa",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CUI",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Constatare",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Firma",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KM",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LucrariConvenite",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Masina",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NrDeviz",
                table: "Devize",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NrInmat",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PieseAduseClient",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SerieCaroserie",
                table: "Devize",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MyEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyEntities");

            migrationBuilder.DropColumn(
                name: "Adresa",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "CUI",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "Constatare",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "Firma",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "KM",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "LucrariConvenite",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "Masina",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "NrDeviz",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "NrInmat",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "PieseAduseClient",
                table: "Devize");

            migrationBuilder.DropColumn(
                name: "SerieCaroserie",
                table: "Devize");

            migrationBuilder.RenameColumn(
                name: "Telefon",
                table: "Devize",
                newName: "Descriere");

            migrationBuilder.RenameColumn(
                name: "SerieMotor",
                table: "Devize",
                newName: "Client");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCreare",
                table: "Devize",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Devize",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
