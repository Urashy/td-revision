using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using td_revision.Controllers;
using td_revision.DTO.Produit;
using td_revision.Mapper;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;

namespace td_revisionTests.Controllers
{
    [TestClass()]
    public class ProduitControllerTests
    {
        private ProduitsbdContext _context;
        private ProduitController _controller;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProduitsbdContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProduitsbdContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            var produitRepository = new ProduitManager(_context);
            var marqueRepository = new MarqueManager(_context);
            var typeProduitRepository = new TypeProduitManager(_context);
            var imageRepository = new ImageManager(_context);
            _controller = new ProduitController(_mapper, produitRepository, marqueRepository, typeProduitRepository, imageRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ShouldGetProduitById()
        {
            // Given: Un produit enregistré avec marque et type
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                Description = "Une superbe chaussure",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit,
                Stock = 50,
                StockMini = 10,
                StockMaxi = 100
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            // When: On récupère le produit par son ID
            var result = await _controller.GetById(produit.IdProduit);

            // Then: Le produit est retourné avec un code 200
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDto = okResult.Value as ProduitDetailDTO;
            Assert.IsNotNull(produitDto);
            Assert.AreEqual("Air Max", produitDto.Nom);
            Assert.AreEqual("Nike", produitDto.Marque);
            Assert.AreEqual("Chaussure", produitDto.Type);
        }

        [TestMethod]
        public async Task GetProduitByIdShouldReturnNotFound()
        {
            // Given: Aucun produit en base

            // When: On essaie de récupérer un produit qui n'existe pas
            var result = await _controller.GetById(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetAllProduits()
        {
            // Given: Plusieurs produits enregistrés
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produits = new List<Produit>
            {
                new Produit { Nom = "Air Max", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 50, StockMini = 10 },
                new Produit { Nom = "Jordan", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 30, StockMini = 10 },
                new Produit { Nom = "Cortez", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 5, StockMini = 10 }
            };
            _context.Produits.AddRange(produits);
            await _context.SaveChangesAsync();

            // When: On récupère tous les produits
            var result = await _controller.GetAll();

            // Then: Tous les produits sont retournés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(3, produitDtos.Count());
        }

        [TestMethod]
        public async Task ShouldGetProduitByName()
        {
            // Given: Un produit avec un nom spécifique
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            // When: On recherche le produit par son nom
            var result = await _controller.GetByName("Air Max");

            // Then: Le produit est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDto = okResult.Value as ProduitDetailDTO;
            Assert.IsNotNull(produitDto);
            Assert.AreEqual("Air Max", produitDto.Nom);
        }

        [TestMethod]
        public async Task ShouldAddProduit()
        {
            // Given: Un DTO de produit à ajouter
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produitDto = new ProduitPostDTO
            {
                Nom = "Air Max 90",
                Description = "Nouvelle chaussure",
                Marque = "Nike",
                Type = "Chaussure",
                Stock = 50,
                StockMini = 10,
                StockMaxi = 100
            };

            // When: On ajoute le produit
            var result = await _controller.Add(produitDto);

            // Then: Le produit est créé avec un code 201
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedDto = createdResult.Value as ProduitDetailDTO;
            Assert.IsNotNull(returnedDto);
            Assert.AreEqual("Air Max 90", returnedDto.Nom);

            var produitInDb = await _context.Produits.FindAsync(returnedDto.IdProduit);
            Assert.IsNotNull(produitInDb);
        }

        [TestMethod]
        public async Task AddProduitShouldReturnBadRequestWhenStockMiniGreaterThanStockMaxi()
        {
            // Given: Un DTO avec stock mini > stock maxi
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produitDto = new ProduitPostDTO
            {
                Nom = "Air Max",
                Marque = "Nike",
                Type = "Chaussure",
                StockMini = 100,
                StockMaxi = 50
            };

            // When: On essaie d'ajouter le produit
            var result = await _controller.Add(produitDto);

            // Then: Un code 400 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddProduitShouldReturnBadRequestWhenMarqueNotFound()
        {
            // Given: Un DTO avec une marque inexistante
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produitDto = new ProduitPostDTO
            {
                Nom = "Air Max",
                Marque = "MarqueInexistante",
                Type = "Chaussure"
            };

            // When: On essaie d'ajouter le produit
            var result = await _controller.Add(produitDto);

            // Then: Un code 400 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ShouldUpdateProduit()
        {
            // Given: Un produit existant
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max Original",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit,
                Stock = 50
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var updatedDto = new ProduitDetailDTO
            {
                IdProduit = produit.IdProduit,
                Nom = "Air Max Modifié",
                Marque = "Nike",
                Type = "Chaussure",
                Stock = 75
            };

            // When: On met à jour le produit
            var result = await _controller.Update(produit.IdProduit, updatedDto);

            // Then: Le produit est mis à jour avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var produitInDb = await _context.Produits.FindAsync(produit.IdProduit);
            Assert.IsNotNull(produitInDb);
            Assert.AreEqual("Air Max Modifié", produitInDb.Nom);
        }

        [TestMethod]
        public async Task UpdateProduitShouldReturnNotFoundWhenProduitDoesNotExist()
        {
            // Given: Un DTO pour un produit qui n'existe pas
            var produitDto = new ProduitDetailDTO
            {
                IdProduit = 999,
                Nom = "Produit Inexistant"
            };

            // When: On essaie de mettre à jour le produit
            var result = await _controller.Update(999, produitDto);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldDeleteProduit()
        {
            // Given: Un produit existant
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();
            var produitId = produit.IdProduit;

            // When: On supprime le produit
            var result = await _controller.Delete(produitId);

            // Then: Le produit est supprimé avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var produitInDb = await _context.Produits.FindAsync(produitId);
            Assert.IsNull(produitInDb);
        }

        [TestMethod]
        public async Task ShouldDeleteProduitWithImages()
        {
            // Given: Un produit avec des images
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var images = new List<Image>
            {
                new Image { Nom = "Image 1", IdProduit = produit.IdProduit },
                new Image { Nom = "Image 2", IdProduit = produit.IdProduit }
            };
            _context.Images.AddRange(images);
            await _context.SaveChangesAsync();

            var produitId = produit.IdProduit;
            var imageIds = images.Select(i => i.IdImage).ToList();

            // When: On supprime le produit
            var result = await _controller.Delete(produitId);

            // Then: Le produit et ses images sont supprimés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var produitInDb = await _context.Produits.FindAsync(produitId);
            Assert.IsNull(produitInDb);

            foreach (var imageId in imageIds)
            {
                var imageInDb = await _context.Images.FindAsync(imageId);
                Assert.IsNull(imageInDb);
            }
        }

        [TestMethod]
        public async Task DeleteProduitShouldReturnNotFoundWhenProduitDoesNotExist()
        {
            // Given: Aucun produit en base

            // When: On essaie de supprimer un produit qui n'existe pas
            var result = await _controller.Delete(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetFilteredProduits()
        {
            // Given: Plusieurs produits avec différentes marques et types
            var nike = new Marque { Nom = "Nike" };
            var adidas = new Marque { Nom = "Adidas" };
            var chaussure = new TypeProduit { Nom = "Chaussure" };
            var vetement = new TypeProduit { Nom = "Vêtement" };
            _context.Marques.AddRange(nike, adidas);
            _context.TypeProduits.AddRange(chaussure, vetement);
            await _context.SaveChangesAsync();

            var produits = new List<Produit>
            {
                new Produit { Nom = "Air Max", Description = "Chaussure confortable", IdMarque = nike.IdMarque, IdTypeProduit = chaussure.IdTypeProduit, Stock = 50, StockMini = 10 },
                new Produit { Nom = "Stan Smith", Description = "Classique", IdMarque = adidas.IdMarque, IdTypeProduit = chaussure.IdTypeProduit, Stock = 30, StockMini = 10 },
                new Produit { Nom = "T-Shirt Nike", Description = "Confortable", IdMarque = nike.IdMarque, IdTypeProduit = vetement.IdTypeProduit, Stock = 100, StockMini = 20 }
            };
            _context.Produits.AddRange(produits);
            await _context.SaveChangesAsync();

            // When: On filtre par marque Nike
            var result = await _controller.GetFiltered(null, "Nike", null);

            // Then: Seuls les produits Nike sont retournés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(2, produitDtos.Count());
            Assert.IsTrue(produitDtos.All(p => p.Marque == "Nike"));
        }

        [TestMethod]
        public async Task ShouldGetFilteredProduitsBySearchTerm()
        {
            // Given: Plusieurs produits avec différentes descriptions
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produits = new List<Produit>
            {
                new Produit { Nom = "Air Max", Description = "Confortable", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 50, StockMini = 10 },
                new Produit { Nom = "Jordan", Description = "Basket", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 30, StockMini = 10 },
                new Produit { Nom = "Running", Description = "Pour courir confortablement", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit, Stock = 40, StockMini = 10 }
            };
            _context.Produits.AddRange(produits);
            await _context.SaveChangesAsync();

            // When: On recherche par terme "confortable"
            var result = await _controller.GetFiltered("confortable", null, null);

            // Then: Seuls les produits contenant "confortable" sont retournés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(2, produitDtos.Count());
        }

        [TestMethod]
        public async Task ShouldDetectEnReapproWhenStockBelowMinimum()
        {
            // Given: Un produit avec stock inférieur au minimum
            var marque = new Marque { Nom = "Nike" };
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.Marques.Add(marque);
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit,
                Stock = 5,
                StockMini = 10,
                StockMaxi = 100
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            // When: On récupère tous les produits
            var result = await _controller.GetAll();

            // Then: Le produit est marqué comme en réapprovisionnement
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            var produitDto = produitDtos.First();
            Assert.IsTrue(produitDto.EnReappro == true);
        }
    }
}