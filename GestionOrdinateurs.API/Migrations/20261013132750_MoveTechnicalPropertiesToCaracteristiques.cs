using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveTechnicalPropertiesToCaracteristiques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ram",
                table: "Ordinateur");

            migrationBuilder.RenameColumn(
                name: "Processeur",
                table: "Ordinateur",
                newName: "Caracteristiques_Processeur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Processeur",
                table: "Ordinateur",
                newName: "Processeur");

            migrationBuilder.AddColumn<string>(
                name: "Ram",
                table: "Ordinateur",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
