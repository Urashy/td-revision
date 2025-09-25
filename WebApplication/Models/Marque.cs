namespace WebApplication.Models
{
    public class Marque : IEntity
    {
        public int IdMarque { get; set; }
        public string? Nom { get; set; }
        public int NbProduits { get; set; }

        public int Id
        {
            get => IdMarque;
            set => IdMarque = value;
        }
    }
}
