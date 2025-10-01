namespace td_revision.DTO.Produit
{
    public class ProduitPostDTO
    {
        public string? Nom { get; set; }
        public string? Type { get; set; }
        public string? Marque { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public int? StockMini { get; set; }
        public int? StockMaxi { get; set; }
    }
}
