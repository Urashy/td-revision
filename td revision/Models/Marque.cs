using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using td_revision.Models.Repository;

namespace td_revision.Models
{
    [Table("t_e_marque_mrq")]
    public class Marque : IEntity, INamedEntity
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