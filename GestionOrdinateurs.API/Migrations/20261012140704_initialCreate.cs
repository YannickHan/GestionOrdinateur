using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs");

            migrationBuilder.RenameTable(
                name: "Ordinateurs",
                newName: "Ordinateur");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur");

            migrationBuilder.RenameTable(
                name: "Ordinateur",
                newName: "Ordinateurs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs",
                column: "Id");
        }
    }
}
