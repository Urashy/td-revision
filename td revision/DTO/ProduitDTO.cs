using td_revision.Controllers;

namespace td_revision.DTO
{
    public class ProduitDTO
    {
        public int IdProduit { get; set; }
        public string? Nom { get; set; }

        public string? Type { get; set; }
        public string? Marque { get; set; }
    }
}
