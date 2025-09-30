using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class ProduitPost
    {
        [JsonPropertyName("nom")]
        [Required(ErrorMessage = "Le nom du produit est obligatoire")]
        public string? Nom { get; set; }

        [JsonPropertyName("type")]
        [Required(ErrorMessage = "Le type de produit est obligatoire")]
        public string? Type { get; set; }

        [JsonPropertyName("marque")]
        [Required(ErrorMessage = "La marque est obligatoire")]
        public string? Marque { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("stock")]
        [Required(ErrorMessage = "Le stock initial est obligatoire")]
        public int? Stock { get; set; }

        [JsonPropertyName("stockMini")]
        [Required(ErrorMessage = "Le stock minimum est obligatoire")]
        public int? StockMini { get; set; }

        [JsonPropertyName("stockMaxi")]
        [Required(ErrorMessage = "Le stock maximum est obligatoire")]
        public int? StockMaxi { get; set; }
    }
}