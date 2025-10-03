using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using td_revision.Models.Repository;

namespace td_revision.Models
{
    [Table("t_e_typeproduit_typ")]
    public class TypeProduit : IEntity, INamedEntity
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