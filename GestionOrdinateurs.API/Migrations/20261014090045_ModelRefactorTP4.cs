using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class ModelRefactorTP4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ordinateur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Marque = table.Column<string>(type: "TEXT", nullable: false),
                    Processeur = table.Column<string>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    Prix = table.Column<decimal>(type: "TEXT", nullable: false),
                    etat = table.Column<int>(type: "INTEGER", nullable: false),
                    Caracteristiques_RamGo = table.Column<int>(type: "INTEGER", nullable: false),
                    Caracteristiques_TypeOrdi = table.Column<int>(type: "INTEGER", nullable: false),
                    Caracteristiques_StockageGo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordinateur", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reparations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrdinateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeReparation = table.Column<string>(type: "TEXT", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reparations_Ordinateur_OrdinateurId",
                        column: x => x.OrdinateurId,
                        principalTable: "Ordinateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Ordinateur",
                columns: new[] { "Id", "Caracteristiques_RamGo", "Caracteristiques_StockageGo", "Caracteristiques_TypeOrdi", "Marque", "Nom", "Prix", "Processeur", "Stock", "etat" },
                values: new object[,]
                {
                    { 1, 8, 512, 1, "HP", "PC1", 800.0m, "AMD Ryzen 5 5600H", 5, 0 },
                    { 2, 16, 512, 0, "Dell", "PC2", 800.0m, "AMD Ryzen 7", 5, 0 },
                    { 3, 32, 1024, 1, "Asus", "PC3", 800.0m, "Intel 5", 5, 0 },
                    { 4, 64, 2048, 0, "MSI", "PC4", 800.0m, "Intel 7", 5, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reparations_OrdinateurId",
                table: "Reparations",
                column: "OrdinateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reparations");

            migrationBuilder.DropTable(
                name: "Ordinateur");
        }
    }
}
