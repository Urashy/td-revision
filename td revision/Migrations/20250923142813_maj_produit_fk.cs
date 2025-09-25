using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace td_revision.Migrations
{
    /// <inheritdoc />
    public partial class maj_produit_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_produits_type_produit",
                table: "produit");

            migrationBuilder.CreateIndex(
                name: "IX_produit_idtypeproduit",
                table: "produit",
                column: "idtypeproduit");

            migrationBuilder.AddForeignKey(
                name: "FK_produits_type_produit",
                table: "produit",
                column: "idtypeproduit",
                principalTable: "typeproduit",
                principalColumn: "idtypeproduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_produits_type_produit",
                table: "produit");

            migrationBuilder.DropIndex(
                name: "IX_produit_idtypeproduit",
                table: "produit");

            migrationBuilder.AddForeignKey(
                name: "FK_produits_type_produit",
                table: "produit",
                column: "idmarque",
                principalTable: "typeproduit",
                principalColumn: "idtypeproduit");
        }
    }
}
