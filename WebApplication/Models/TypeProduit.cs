namespace WebApplication.Models
{
    public class TypeProduit : IEntity
    {
        public int IdTypeProduit { get; set; }
        public string Nom { get; set; }
        public int? NbProduits { get; set; } = 0;

        public int Id
        {
            get => IdTypeProduit;
            set => IdTypeProduit = value;
        }
    }
}
