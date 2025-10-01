using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace td_revision.Models
{


    [Table("image")]
    public class Image : IEntity
    {
        [Key]
        [Column("idimage")]
        public int IdImage { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("urlphoto")]
        public string? Url { get; set; }

        [Column("idproduit")]
        public int IdProduit { get; set; }

        [ForeignKey(nameof(IdProduit))]
        [InverseProperty(nameof(Produit.Images))]
        public virtual Produit ProduitNavigation { get; set; } = null!;

        public int Id => IdImage;
    }
}