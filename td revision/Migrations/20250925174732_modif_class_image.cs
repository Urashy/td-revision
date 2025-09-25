using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace td_revision.Migrations
{
    /// <inheritdoc />
    public partial class modif_class_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nomphoto",
                table: "produit");

            migrationBuilder.DropColumn(
                name: "urlphoto",
                table: "produit");

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    idimage = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false),
                    urlphoto = table.Column<string>(type: "text", nullable: true),
                    idproduit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image", x => x.idimage);
                    table.ForeignKey(
                        name: "FK_image_produit_idproduit",
                        column: x => x.idproduit,
                        principalTable: "produit",
                        principalColumn: "idproduit",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_image_idproduit",
                table: "image",
                column: "idproduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.AddColumn<string>(
                name: "nomphoto",
                table: "produit",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "urlphoto",
                table: "produit",
                type: "text",
                nullable: true);
        }
    }
}
