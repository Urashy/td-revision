using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.Models.Repository;
using static System.Net.WebRequestMethods;

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

        new Produit { Nom="Bravia OLED", Description="Télévision Sony",
            IdMarque=marquesDb.First(m=>m.Nom=="Sony").IdMarque,
            IdTypeProduit=typesDb.First(t=>t.Nom=="Télévision").IdTypeProduit,
            Stock=8, StockMini=3, StockMaxi=15 }
    };
            foreach (var p in produits) await _produitRepository.AddAsync(p);

            // Produits spéciaux E2E
            var marqueTest = marquesDb.First(m => m.Nom == "MarqueTest");
            var typeTest = typesDb.First(t => t.Nom == "TypeTest");

            var produitTest1 = new Produit
            {
                Nom = "Produit test 1",
                Description = "Produit utilisé pour les tests",
                IdMarque = marqueTest.IdMarque,
                IdTypeProduit = typeTest.IdTypeProduit,
                Stock = 5,
                StockMini = 3,
                StockMaxi = 10
            };
            await _produitRepository.AddAsync(produitTest1);

            var produitTest2 = new Produit
            {
                Nom = "Nouveau Produit test E2E",
                Description = "Deuxième produit de test",
                IdMarque = marqueTest.IdMarque,
                IdTypeProduit = typeTest.IdTypeProduit,
                Stock = 12,
                StockMini = 5,
                StockMaxi = 20
            };
            await _produitRepository.AddAsync(produitTest2);

            // 🔹 Recharger les produits pour avoir leurs IDs
            var produitsDb = await _produitRepository.GetAllAsync();

            // Images associées
            var images = new[]
            {
        new Image { Nom="iPhone15", Url="https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcQRjj5FqXNtQZEP2oL70ybOdohein3ddcNB1M_g9Msc_TI1NV9gNC6YxmrpgU9Ba7ONPd7BR4pmdFBDG9lQcEpi1szxvf8dFXGlhrmchDRQ4noQZOiOLvB_8ajzLr4V&usqp=CAc",
            IdProduit = produitsDb.First(p => p.Nom=="iPhone 15 Pro").IdProduit },
        new Image { Nom="iPhone15.1", Url="https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/IPhone_15_Vector.svg/langfr-250px-IPhone_15_Vector.svg.png",
            IdProduit = produitsDb.First(p => p.Nom=="iPhone 15 Pro").IdProduit },

        new Image { Nom="iPadAir", Url="https://encrypted-tbn0.gstatic.com/shopping?q=tbn:ANd9GcRwp5zYrry6NTaWUnLoQSoqppXdJXRJu_ogu0xFBfoc98_r5vmqt1p4asaAmFFKjM6gLqSz79CssEDlrA1gLSDEJDVTd2WsiJ0dUYtL62WQ3J_V7ZeCpDEvI8b5UuAk&usqp=CAc",
            IdProduit = produitsDb.First(p => p.Nom=="iPad Air").IdProduit },

        new Image { Nom="GalaxyS24", Url="https://encrypted-tbn3.gstatic.com/shopping?q=tbn:ANd9GcTqhqJkf56uCKsQHDmcyl8M6JkEqOoSPbjEs0XJ-TSiqBc3wZ8LDXy5vZIJq1uPSyVNiOdwk_VXbt4Hmcj7NAxlPJoLxk3KzCXEpe_wgdjEEG9DxiBtlEwmdLPtAeZf_MN6ZrrOWNQ&usqp=CAc",
            IdProduit = produitsDb.First(p => p.Nom=="Galaxy S24 Ultra").IdProduit },

        new Image { Nom="XPS15", Url="https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/xps-notebooks/xps-15-9530/media-gallery/touch-black/notebook-xps-15-9530-t-black-gallery-1.psd?fmt=png-alpha&pscan=auto&scl=1&hei=402&wid=654&qlt=100,1&resMode=sharp2&size=654,402&chrss=full",
            IdProduit = produitsDb.First(p => p.Nom=="XPS 15").IdProduit },

        new Image { Nom="Bravia", Url="https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcSVODlfFxBeaQKh2M12ES8-KOpmN8mgBy25jEPOPfwflj45SgxwR7gSm5hT-l_dIiYpz3zu4cM_WSSpI_4-dmQXbSmripH1NDe742VpL_2_OE9WWMpTtJX1v0ZMN2zrmgJcBJWb5Q-OCQ&usqp=CAc",
            IdProduit = produitsDb.First(p => p.Nom=="Bravia OLED").IdProduit },

        new Image { Nom="ProduitTest1", Url="https://latavernedutesteur.fr/wp-content/uploads/2017/11/testss.png",
            IdProduit = produitsDb.First(p => p.Nom=="Produit test 1").IdProduit },

        new Image { Nom="ProduitTest2", Url="https://latavernedutesteur.fr/wp-content/uploads/2017/11/testss.png",
            IdProduit = produitsDb.First(p => p.Nom=="Nouveau Produit test E2E").IdProduit }
    };
            foreach (var img in images) await _imageRepository.AddAsync(img);
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
