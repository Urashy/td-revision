using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class ProduitSimple : IEntity
    {
        [JsonPropertyName("idProduit")]
        public int IdProduit { get; set; }

        [JsonPropertyName("nom")]
        public string? Nom { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("marque")]
        public string? Marque { get; set; }
        public bool? EnReappro { get; set; } 


        public int Id
        {
            get => IdProduit;
            set => IdProduit = value;
        }
    }
}