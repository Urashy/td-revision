using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class Marque : IEntity
    {
        [JsonPropertyName("idMarque")]
        public int IdMarque { get; set; }

        [JsonPropertyName("nom")]
        public string? Nom { get; set; }

        [JsonPropertyName("nbProduits")]
        public int NbProduits { get; set; }

        public int Id
        {
            get => IdMarque;
            set => IdMarque = value;
        }
    }
}