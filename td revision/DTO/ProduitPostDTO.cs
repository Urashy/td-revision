using System.ComponentModel.DataAnnotations;

namespace td_revision.DTO
{
    public class ProduitPostDTO
    {
        [Required(ErrorMessage = "Le nom du produit est obligatoire")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "Le type de produit est obligatoire")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "La marque est obligatoire")]
        public string? Marque { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Le stock initial est obligatoire")]
        public int? Stock { get; set; }

        [Required(ErrorMessage = "Le stock minimum est obligatoire")]
        public int? StockMini { get; set; }

        [Required(ErrorMessage = "Le stock maximum est obligatoire")]
        public int? StockMaxi { get; set; }
    }
}