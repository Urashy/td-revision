using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Image : IEntity
    {
        public int IdImage { get; set; }
        public string NomImage { get; set; }
        public string? UrlPhoto { get; set; }
        public int IdProduit { get; set; }

        public int Id
        {
            get => IdImage;
            set => IdImage = value;
        }
    }
}
