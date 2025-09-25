using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Produit : IEntity
    {
        public int IdProduit { get; set; }
        public string? Nom { get; set; }
        public string? Type { get; set; }
        public string? Marque { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public bool EnReappro { get; set; }

        public int Id
        {
            get => IdProduit;
            set => IdProduit = value;
        }
    }
}
