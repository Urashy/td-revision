using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class TypeProduit : IEntity
    {
        [JsonPropertyName("idTypeProduit")]
        public int IdTypeProduit { get; set; }

        [JsonPropertyName("nom")]
        public string Nom { get; set; } = string.Empty;

        [JsonPropertyName("nbProduits")]
        public int NbProduits { get; set; } = 0;

        public int Id
        {
            get => IdTypeProduit;
            set => IdTypeProduit = value;
        }
    }
}