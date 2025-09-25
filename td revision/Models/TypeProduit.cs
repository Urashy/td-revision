using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace td_revision.Models
{
    [Table("typeproduit")]
    public class TypeProduit : IEntity
    {
        [Key]
        [Column("idtypeproduit")]
        public int IdTypeProduit { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [InverseProperty(nameof(Produit.TypeProduitNavigation))]
        public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();

        public int Id => IdTypeProduit;

    }
}
