using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace td_revision.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesISO11179 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_e_marque_mrq",
                columns: table => new
                {
                    idmarque = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_marque_mrq", x => x.idmarque);
                });

            migrationBuilder.CreateTable(
                name: "t_e_typeproduit_typ",
                columns: table => new
                {
                    idtypeproduit = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_typeproduit_typ", x => x.idtypeproduit);
                });

            migrationBuilder.CreateTable(
                name: "t_e_produit_prd",
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
                    table.PrimaryKey("PK_t_e_produit_prd", x => x.idproduit);
                    table.ForeignKey(
                        name: "FK_t_e_produit_prd_t_e_marque_mrq",
                        column: x => x.idmarque,
                        principalTable: "t_e_marque_mrq",
                        principalColumn: "idmarque");
                    table.ForeignKey(
                        name: "FK_t_e_produit_prd_t_e_typeproduit_typ",
                        column: x => x.idtypeproduit,
                        principalTable: "t_e_typeproduit_typ",
                        principalColumn: "idtypeproduit");
                });

            migrationBuilder.CreateTable(
                name: "t_e_image_img",
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
                    table.PrimaryKey("PK_t_e_image_img", x => x.idimage);
                    table.ForeignKey(
                        name: "FK_t_e_image_img_t_e_produit_prd",
                        column: x => x.idproduit,
                        principalTable: "t_e_produit_prd",
                        principalColumn: "idproduit");
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_image_img_idproduit",
                table: "t_e_image_img",
                column: "idproduit");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_prd_idmarque",
                table: "t_e_produit_prd",
                column: "idmarque");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_prd_idtypeproduit",
                table: "t_e_produit_prd",
                column: "idtypeproduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_image_img");

            migrationBuilder.DropTable(
                name: "t_e_produit_prd");

            migrationBuilder.DropTable(
                name: "t_e_marque_mrq");

            migrationBuilder.DropTable(
                name: "t_e_typeproduit_typ");
        }
    }
}
