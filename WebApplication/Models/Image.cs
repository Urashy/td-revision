using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class Image : IEntity
    {
        [JsonPropertyName("idImage")]
        public int IdImage { get; set; }

        [JsonPropertyName("nom")]
        public string? NomImage { get; set; }

        [JsonPropertyName("url")]
        public string? UrlPhoto { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("idProduit")]
        public int IdProduit { get; set; }

        public int Id
        {
            get => IdImage;
            set => IdImage = value;
        }
    }
}