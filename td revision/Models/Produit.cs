using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace td_revision.Models
{
    [Table("produit")]
    public class Produit : IEntity
    {
        [Key]
        [Column("idproduit")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProduit { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("idmarque")]
        public int? IdMarque { get; set; }

        [Column("idtypeproduit")]
        public int? IdTypeProduit { get; set; }

        [Column("stockreel")]
        public int? Stock { get; set; }

        [Column("stockmini")]
        public int? StockMini { get; set; }

        [Column("stockmaxi")]
        public int? StockMaxi { get; set; }

        [ForeignKey("IdMarque")]
        [InverseProperty(nameof(Marque.Produits))]
        public virtual Marque? MarqueProduitNavigation { get; set; } = null!;

        [ForeignKey("IdTypeProduit")]
        [InverseProperty(nameof(TypeProduit.Produits))]
        public virtual TypeProduit? TypeProduitNavigation { get; set; } = null!;

        [InverseProperty(nameof(Image.ProduitNavigation))]
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        public int Id => IdProduit;
    }
}