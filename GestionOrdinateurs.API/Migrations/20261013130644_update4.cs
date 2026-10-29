using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdinateurId1",
                table: "Reparation",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reparation_OrdinateurId1",
                table: "Reparation",
                column: "OrdinateurId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reparation_Ordinateur_OrdinateurId1",
                table: "Reparation",
                column: "OrdinateurId1",
                principalTable: "Ordinateur",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reparation_Ordinateur_OrdinateurId1",
                table: "Reparation");

            migrationBuilder.DropIndex(
                name: "IX_Reparation_OrdinateurId1",
                table: "Reparation");

            migrationBuilder.DropColumn(
                name: "OrdinateurId1",
                table: "Reparation");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateur");
        }
    }
}
