using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace td_revision.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "marque",
                columns: table => new
                {
                    idmarque = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marque", x => x.idmarque);
                });

            migrationBuilder.CreateTable(
                name: "typeproduit",
                columns: table => new
                {
                    idtypeproduit = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeproduit", x => x.idtypeproduit);
                });

            migrationBuilder.CreateTable(
                name: "produit",
                columns: table => new
                {
                    idproduit = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    idmarque = table.Column<int>(type: "integer", nullable: true),
                    idtypeproduit = table.Column<int>(type: "integer", nullable: true),
                    stockreel = table.Column<int>(type: "integer", nullable: true),
                    stockmini = table.Column<int>(type: "integer", nullable: true),
                    stockmaxi = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_produit", x => x.idproduit);
                    table.ForeignKey(
                        name: "FK_produits_marque",
                        column: x => x.idmarque,
                        principalTable: "marque",
                        principalColumn: "idmarque");
                    table.ForeignKey(
                        name: "FK_produits_type_produit",
                        column: x => x.idtypeproduit,
                        principalTable: "typeproduit",
                        principalColumn: "idtypeproduit");
                });

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
                        name: "FK_images_produit",
                        column: x => x.idproduit,
                        principalTable: "produit",
                        principalColumn: "idproduit");
                });

            migrationBuilder.CreateIndex(
                name: "IX_image_idproduit",
                table: "image",
                column: "idproduit");

            migrationBuilder.CreateIndex(
                name: "IX_produit_idmarque",
                table: "produit",
                column: "idmarque");

            migrationBuilder.CreateIndex(
                name: "IX_produit_idtypeproduit",
                table: "produit",
                column: "idtypeproduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "produit");

            migrationBuilder.DropTable(
                name: "marque");

            migrationBuilder.DropTable(
                name: "typeproduit");
        }
    }
}
