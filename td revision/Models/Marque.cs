using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace td_revision.Models
{
    [Table("marque")]
    public class Marque : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idmarque")]
        public int IdMarque { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [InverseProperty(nameof(Produit.MarqueProduitNavigation))]
        public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
        public int Id => IdMarque;
    }
}

