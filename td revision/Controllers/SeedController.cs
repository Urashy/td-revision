using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly INamedRepository<Produit> _produitRepository;
        private readonly INamedRepository<Marque> _marqueRepository;
        private readonly INamedRepository<TypeProduit> _typeProduitRepository;
        private readonly IRepository<Image> _imageRepository;

        public SeedController(
            INamedRepository<Produit> produitRepository,
            INamedRepository<Marque> marqueRepository,
            INamedRepository<TypeProduit> typeProduitRepository,
            IRepository<Image> imageRepository)
        {
            _produitRepository = produitRepository;
            _marqueRepository = marqueRepository;
            _typeProduitRepository = typeProduitRepository;
            _imageRepository = imageRepository;
        }

        [HttpPost]
        [ActionName("InitializeDemo")]
        public async Task<ActionResult> InitializeDemo()
        {
            var existingProducts = await _produitRepository.GetAllAsync();
            if (existingProducts != null && existingProducts.Any())
                return BadRequest("La base contient déjà des produits. Utilisez ResetAndSeed pour réinitialiser.");

            await SeedData();
            return Ok(new { message = "Données de démonstration créées" });
        }

        [HttpPost]
        [ActionName("ResetAndSeed")]
        public async Task<ActionResult> ResetAndSeed()
        {
            await DeleteAllData();
            await SeedData();
            return Ok(new { message = "Base réinitialisée + données créées" });
        }

        private async Task DeleteAllData()
        {
            foreach (var img in await _imageRepository.GetAllAsync() ?? [])
                await _imageRepository.DeleteAsync(img);

            foreach (var p in await _produitRepository.GetAllAsync() ?? [])
                await _produitRepository.DeleteAsync(p);

            foreach (var m in await _marqueRepository.GetAllAsync() ?? [])
                await _marqueRepository.DeleteAsync(m);

            foreach (var t in await _typeProduitRepository.GetAllAsync() ?? [])
                await _typeProduitRepository.DeleteAsync(t);
        }

        private async Task SeedData()
        {
            // Marques
            var marques = new[]
            {
        "Apple", "Samsung", "Sony", "Dell", "HP", "MarqueTest" // E2E
    }.Select(n => new Marque { Nom = n }).ToList();
            foreach (var m in marques) await _marqueRepository.AddAsync(m);

            // Types
            var types = new[]
            {
        "Smartphone", "Ordinateur portable", "Tablette", "Écouteurs", "Télévision", "TypeTest" // E2E
    }.Select(n => new TypeProduit { Nom = n }).ToList();
            foreach (var t in types) await _typeProduitRepository.AddAsync(t);

            // Recharger depuis DB (pour les IDs)
            var marquesDb = await _marqueRepository.GetAllAsync();
            var typesDb = await _typeProduitRepository.GetAllAsync();

            // Produits démo
            var produits = new[]
            {
        new Produit { Nom="iPhone 15 Pro", Description="Smartphone haut de gamme",
            IdMarque=marquesDb.First(m=>m.Nom=="Apple").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Smartphone").IdTypeProduit,
            Stock=25, StockMini=10, StockMaxi=50 },

        new Produit { Nom="iPad Air", Description="Tablette performante",
            IdMarque=marquesDb.First(m=>m.Nom=="Apple").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Tablette").IdTypeProduit,
            Stock=5, StockMini=15, StockMaxi=40 }, // 🔹 en réappro

        new Produit { Nom="Galaxy S24 Ultra", Description="Smartphone premium",
            IdMarque=marquesDb.First(m=>m.Nom=="Samsung").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Smartphone").IdTypeProduit,
            Stock=18, StockMini=12, StockMaxi=45 },

        new Produit { Nom="XPS 15", Description="Ordinateur portable Dell",
            IdMarque=marquesDb.First(m=>m.Nom=="Dell").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Ordinateur portable").IdTypeProduit,
            Stock=14, StockMini=10, StockMaxi=28 },

        // 🔹 Nouveau produit supplémentaire pour atteindre 7
        new Produit { Nom="Bravia OLED", Description="Télévision Sony",
            IdMarque=marquesDb.First(m=>m.Nom=="Sony").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Télévision").IdTypeProduit,
            Stock=8, StockMini=3, StockMaxi=15 }
    };
            foreach (var p in produits) await _produitRepository.AddAsync(p);

            // Produits spéciaux E2E
            var marqueTest = marquesDb.First(m => m.Nom == "MarqueTest");
            var typeTest = typesDb.First(t => t.Nom == "TypeTest");

            await _produitRepository.AddAsync(new Produit
            {
                Nom = "Produit test 1",
                Description = "Produit utilisé pour les tests",
                IdMarque = marqueTest.IdMarque,
                IdTypeProduit = typeTest.IdTypeProduit,
                Stock = 5,
                StockMini = 3,
                StockMaxi = 10
            });

            await _produitRepository.AddAsync(new Produit
            {
                Nom = "Nouveau Produit test E2E",
                Description = "Deuxième produit de test",
                IdMarque = marqueTest.IdMarque,
                IdTypeProduit = typeTest.IdTypeProduit,
                Stock = 12,
                StockMini = 5,
                StockMaxi = 20
            });
        }


        [HttpGet]
        [ActionName("GetStats")]
        public async Task<ActionResult> GetStats()
        {
            var produits = await _produitRepository.GetAllAsync();
            var marques = await _marqueRepository.GetAllAsync();
            var types = await _typeProduitRepository.GetAllAsync();
            var images = await _imageRepository.GetAllAsync();

            return Ok(new
            {
                produits = produits?.Count() ?? 0,
                marques = marques?.Count() ?? 0,
                types = types?.Count() ?? 0,
                images = images?.Count() ?? 0
            });
        }
    }
}
