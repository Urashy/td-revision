using Microsoft.AspNetCore.Mvc;
using td_revision.Models;
using td_revision.Models.Repository;
using td_revision.DTO.Produit;

namespace td_revision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IDataRepository<Produit> _produitRepository;
        private readonly IDataRepository<Marque> _marqueRepository;
        private readonly IDataRepository<TypeProduit> _typeProduitRepository;
        private readonly IDataRepository<Image> _imageRepository;

        public SeedController(
            IDataRepository<Produit> produitRepository,
            IDataRepository<Marque> marqueRepository,
            IDataRepository<TypeProduit> typeProduitRepository,
            IDataRepository<Image> imageRepository)
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
            try
            {
                // Vérifier si des données existent déjà
                var existingProducts = await _produitRepository.GetAllAsync();
                if (existingProducts.Value != null && existingProducts.Value.Any())
                {
                    return BadRequest("La base de données contient déjà des produits. Utilisez ResetAndSeed pour tout réinitialiser.");
                }

                await SeedData();
                return Ok(new { message = "Données de démonstration créées avec succès", count = 15 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'initialisation : {ex.Message}");
            }
        }

        [HttpPost]
        [ActionName("ResetAndSeed")]
        public async Task<ActionResult> ResetAndSeed()
        {
            try
            {
                // Supprimer toutes les données existantes
                await DeleteAllData();

                // Créer les nouvelles données
                await SeedData();

                return Ok(new { message = "Base de données réinitialisée et données de démonstration créées", count = 15 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la réinitialisation : {ex.Message}");
            }
        }

        private async Task DeleteAllData()
        {
            // Supprimer dans l'ordre inverse des dépendances
            var allImages = await _imageRepository.GetAllAsync();
            if (allImages.Value != null)
            {
                foreach (var image in allImages.Value)
                {
                    await _imageRepository.DeleteAsync(image);
                }
            }

            var allProduits = await _produitRepository.GetAllAsync();
            if (allProduits.Value != null)
            {
                foreach (var produit in allProduits.Value)
                {
                    await _produitRepository.DeleteAsync(produit);
                }
            }

            var allMarques = await _marqueRepository.GetAllAsync();
            if (allMarques.Value != null)
            {
                foreach (var marque in allMarques.Value)
                {
                    await _marqueRepository.DeleteAsync(marque);
                }
            }

            var allTypes = await _typeProduitRepository.GetAllAsync();
            if (allTypes.Value != null)
            {
                foreach (var type in allTypes.Value)
                {
                    await _typeProduitRepository.DeleteAsync(type);
                }
            }
        }

        private async Task SeedData()
        {
            // 1. Créer les marques
            var marques = new[]
            {
                new Marque { Nom = "Apple" },
                new Marque { Nom = "Samsung" },
                new Marque { Nom = "Sony" },
                new Marque { Nom = "Dell" },
                new Marque { Nom = "HP" }
            };

            foreach (var marque in marques)
            {
                await _marqueRepository.AddAsync(marque);
            }

            // 2. Créer les types
            var types = new[]
            {
                new TypeProduit { Nom = "Smartphone" },
                new TypeProduit { Nom = "Ordinateur portable" },
                new TypeProduit { Nom = "Tablette" },
                new TypeProduit { Nom = "Écouteurs" },
                new TypeProduit { Nom = "Télévision" }
            };

            foreach (var type in types)
            {
                await _typeProduitRepository.AddAsync(type);
            }

            // Récupérer les IDs créés
            var marquesCreated = await _marqueRepository.GetAllAsync();
            var typesCreated = await _typeProduitRepository.GetAllAsync();

            // 3. Créer les produits
            var produits = new[]
            {
                // Apple
                new Produit
                {
                    Nom = "iPhone 15 Pro",
                    Description = "Smartphone haut de gamme avec processeur A17 Pro",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Apple").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Smartphone").IdTypeProduit,
                    Stock = 25,
                    StockMini = 10,
                    StockMaxi = 50
                },
                new Produit
                {
                    Nom = "MacBook Pro 16\"",
                    Description = "Ordinateur portable professionnel avec puce M3 Max",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Apple").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Ordinateur portable").IdTypeProduit,
                    Stock = 12,
                    StockMini = 8,
                    StockMaxi = 30
                },
                new Produit
                {
                    Nom = "iPad Air",
                    Description = "Tablette légère et performante pour le travail et les loisirs",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Apple").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Tablette").IdTypeProduit,
                    Stock = 5,
                    StockMini = 15,
                    StockMaxi = 40
                },
                new Produit
                {
                    Nom = "AirPods Pro",
                    Description = "Écouteurs sans fil avec réduction de bruit active",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Apple").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Écouteurs").IdTypeProduit,
                    Stock = 45,
                    StockMini = 20,
                    StockMaxi = 60
                },
                
                // Samsung
                new Produit
                {
                    Nom = "Galaxy S24 Ultra",
                    Description = "Smartphone premium avec stylet S Pen intégré",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Samsung").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Smartphone").IdTypeProduit,
                    Stock = 18,
                    StockMini = 12,
                    StockMaxi = 45
                },
                new Produit
                {
                    Nom = "Galaxy Book4 Pro",
                    Description = "Ordinateur portable ultra-fin et léger",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Samsung").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Ordinateur portable").IdTypeProduit,
                    Stock = 8,
                    StockMini = 10,
                    StockMaxi = 25
                },
                new Produit
                {
                    Nom = "Galaxy Tab S9",
                    Description = "Tablette Android haut de gamme avec écran AMOLED",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Samsung").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Tablette").IdTypeProduit,
                    Stock = 22,
                    StockMini = 15,
                    StockMaxi = 35
                },
                new Produit
                {
                    Nom = "Neo QLED 65\"",
                    Description = "Télévision 4K avec technologie Quantum HDR",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Samsung").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Télévision").IdTypeProduit,
                    Stock = 7,
                    StockMini = 5,
                    StockMaxi = 15
                },
                
                // Sony
                new Produit
                {
                    Nom = "Xperia 1 VI",
                    Description = "Smartphone pour les créateurs avec écran 4K",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Sony").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Smartphone").IdTypeProduit,
                    Stock = 3,
                    StockMini = 8,
                    StockMaxi = 20
                },
                new Produit
                {
                    Nom = "WH-1000XM5",
                    Description = "Casque audio avec meilleure réduction de bruit du marché",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Sony").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Écouteurs").IdTypeProduit,
                    Stock = 32,
                    StockMini = 15,
                    StockMaxi = 50
                },
                new Produit
                {
                    Nom = "Bravia XR 55\"",
                    Description = "Télévision OLED avec processeur cognitif XR",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Sony").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Télévision").IdTypeProduit,
                    Stock = 10,
                    StockMini = 6,
                    StockMaxi = 18
                },
                
                // Dell
                new Produit
                {
                    Nom = "XPS 15",
                    Description = "Ordinateur portable pour créatifs avec écran InfinityEdge",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "Dell").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Ordinateur portable").IdTypeProduit,
                    Stock = 14,
                    StockMini = 10,
                    StockMaxi = 28
                },
                
                // HP
                new Produit
                {
                    Nom = "Spectre x360",
                    Description = "PC convertible 2-en-1 haut de gamme",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "HP").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Ordinateur portable").IdTypeProduit,
                    Stock = 16,
                    StockMini = 12,
                    StockMaxi = 32
                },
                new Produit
                {
                    Nom = "Elite x2",
                    Description = "Tablette professionnelle détachable avec Windows",
                    IdMarque = marquesCreated.Value?.First(m => m.Nom == "HP").IdMarque,
                    IdTypeProduit = typesCreated.Value?.First(t => t.Nom == "Tablette").IdTypeProduit,
                    Stock = 11,
                    StockMini = 10,
                    StockMaxi = 25
                }
            };

            foreach (var produit in produits)
            {
                await _produitRepository.AddAsync(produit);
            }

            // 4. Ajouter quelques images pour certains produits
            var produitsCreated = await _produitRepository.GetAllAsync();

            var images = new[]
            {
                new Image
                {
                    Nom = "iPhone 15 Pro - Avant",
                    Url = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=300",
                    IdProduit = produitsCreated.Value?.First(p => p.Nom == "iPhone 15 Pro").IdProduit ?? 0
                },
                new Image
                {
                    Nom = "MacBook Pro - Vue principale",
                    Url = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=300",
                    IdProduit = produitsCreated.Value?.First(p => p.Nom == "MacBook Pro 16\"").IdProduit ?? 0
                },
                new Image
                {
                    Nom = "Galaxy S24 - Design",
                    Url = "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=300",
                    IdProduit = produitsCreated.Value?.First(p => p.Nom == "Galaxy S24 Ultra").IdProduit ?? 0
                },new Image
                {
                    Nom = "Galaxy Book4 Pro",
                    Url = "https://encrypted-tbn3.gstatic.com/shopping?q=tbn:ANd9GcQ67a8SAzLpqKHqJYhwBaShGJKTHLS12xVxE0w2skwKVjkhQPB-O1uMcqPRXgpnxMN4stIH6kGB-rIjLOSraQ9nuDGBMLbKnS7d0gSuey3hDDoCUQqqK7C8K4SEgtiFwKdK5NEQTnYvDyM&usqp=CAc",
                    IdProduit = produitsCreated.Value?.First(p => p.Nom == "Galaxy Book4 Pro").IdProduit ?? 0
                },new Image
                {
                    Nom = "iPad Air",
                    Url = "https://encrypted-tbn0.gstatic.com/shopping?q=tbn:ANd9GcRwp5zYrry6NTaWUnLoQSoqppXdJXRJu_ogu0xFBfoc98_r5vmqt1p4asaAmFFKjM6gLqSz79CssEDlrA1gLSDEJDVTd2WsiJ0dUYtL62WQ3J_V7ZeCpDEvI8b5UuAk&usqp=CAc",
                    IdProduit = produitsCreated.Value?.First(p => p.Nom == "iPad Air").IdProduit ?? 0
                }


            };

            foreach (var image in images)
            {
                if (image.IdProduit > 0)
                {
                    await _imageRepository.AddAsync(image);
                }
            }
        }

        [HttpGet]
        [ActionName("GetStats")]
        public async Task<ActionResult> GetStats()
        {
            try
            {
                var produits = await _produitRepository.GetAllAsync();
                var marques = await _marqueRepository.GetAllAsync();
                var types = await _typeProduitRepository.GetAllAsync();
                var images = await _imageRepository.GetAllAsync();

                return Ok(new
                {
                    produits = produits.Value?.Count() ?? 0,
                    marques = marques.Value?.Count() ?? 0,
                    types = types.Value?.Count() ?? 0,
                    images = images.Value?.Count() ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur : {ex.Message}");
            }
        }
    }
}