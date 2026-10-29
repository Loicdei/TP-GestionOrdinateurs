using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class FixModelAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Vendu",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Ordinateur",
                keyColumn: "Id",
                keyValue: 1,
                column: "Vendu",
                value: false);

            migrationBuilder.UpdateData(
                table: "Ordinateur",
                keyColumn: "Id",
                keyValue: 2,
                column: "Vendu",
                value: false);

            migrationBuilder.UpdateData(
                table: "Ordinateur",
                keyColumn: "Id",
                keyValue: 3,
                column: "Vendu",
                value: false);

            migrationBuilder.UpdateData(
                table: "Ordinateur",
                keyColumn: "Id",
                keyValue: 4,
                column: "Vendu",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vendu",
                table: "Ordinateur");
        }
    }
}
