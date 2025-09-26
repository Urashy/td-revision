using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class Produit : IEntity
    {
        [JsonPropertyName("idProduit")]
        public int IdProduit { get; set; }

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
        public int? Stock { get; set; }

        [JsonPropertyName("enReappro")]
        public bool EnReappro { get; set; }

        public int Id
        {
            get => IdProduit;
            set => IdProduit = value;
        }
    }
}