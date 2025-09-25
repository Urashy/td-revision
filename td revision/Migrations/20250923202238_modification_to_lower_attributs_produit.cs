using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace td_revision.Migrations
{
    /// <inheritdoc />
    public partial class modification_to_lower_attributs_produit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stockReel",
                table: "produit",
                newName: "stockreel");

            migrationBuilder.RenameColumn(
                name: "stockMini",
                table: "produit",
                newName: "stockmini");

            migrationBuilder.RenameColumn(
                name: "nomPhoto",
                table: "produit",
                newName: "nomphoto");

            migrationBuilder.RenameColumn(
                name: "UrlPhoto",
                table: "produit",
                newName: "urlphoto");

            migrationBuilder.RenameColumn(
                name: "StockMaxi",
                table: "produit",
                newName: "stockmaxi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "urlphoto",
                table: "produit",
                newName: "UrlPhoto");

            migrationBuilder.RenameColumn(
                name: "stockreel",
                table: "produit",
                newName: "stockReel");

            migrationBuilder.RenameColumn(
                name: "stockmini",
                table: "produit",
                newName: "stockMini");

            migrationBuilder.RenameColumn(
                name: "stockmaxi",
                table: "produit",
                newName: "StockMaxi");

            migrationBuilder.RenameColumn(
                name: "nomphoto",
                table: "produit",
                newName: "nomPhoto");
        }
    }
}
