using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using td_revision.Controllers;
using td_revision.DTO;
using td_revision.Mapper;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;

namespace td_revisionTests.Controllers.Tests
{
    [TestClass()]
    public class MarqueControllerTests
    {
        private ProduitsbdContext _context;
        private MarqueController _controller;
        private IMapper _mapper;
        private IDataRepository<Produit> _produitRepository;
        private IDataRepository<Image> _imageRepository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProduitsbdContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProduitsbdContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            var marqueRepository = new MarqueManager(_context);
            _produitRepository = new ProduitManager(_context);
            _imageRepository = new ImageManager(_context);
            _controller = new MarqueController(_mapper, marqueRepository, _produitRepository, _imageRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ShouldGetMarqueById()
        {
            // Given: Une marque enregistrée
            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            // When: On récupère la marque par son ID
            var result = await _controller.GetById(marque.IdMarque);

            // Then: La marque est retournée avec un code 200
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var marqueDto = okResult.Value as MarqueDTO;
            Assert.IsNotNull(marqueDto);
            Assert.AreEqual("Nike", marqueDto.Nom);
        }

        [TestMethod]
        public async Task GetMarqueByIdShouldReturnNotFound()
        {
            // Given: Aucune marque en base

            // When: On essaie de récupérer une marque qui n'existe pas
            var result = await _controller.GetById(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetAllMarques()
        {
            // Given: Plusieurs marques enregistrées
            var marques = new List<Marque>
            {
                new Marque { Nom = "Nike" },
                new Marque { Nom = "Adidas" },
                new Marque { Nom = "Puma" }
            };
            _context.Marques.AddRange(marques);
            await _context.SaveChangesAsync();

            // When: On récupère toutes les marques
            var result = await _controller.GetAll();

            // Then: Toutes les marques sont retournées
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var marqueDtos = okResult.Value as IEnumerable<MarqueDTO>;
            Assert.IsNotNull(marqueDtos);
            Assert.AreEqual(3, marqueDtos.Count());
        }

        [TestMethod]
        public async Task ShouldGetMarqueByName()
        {
            // Given: Une marque avec un nom spécifique
            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            // When: On recherche la marque par son nom
            var result = await _controller.GetByName("Nike");

            // Then: La marque est retournée
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var marqueDto = okResult.Value as MarqueDTO;
            Assert.IsNotNull(marqueDto);
            Assert.AreEqual("Nike", marqueDto.Nom);
        }

        [TestMethod]
        public async Task ShouldAddMarque()
        {
            // Given: Un DTO de marque à ajouter
            var marqueDto = new MarqueDTO { Nom = "Reebok" };

            // When: On ajoute la marque
            var result = await _controller.Add(marqueDto);

            // Then: La marque est créée avec un code 201
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedDto = createdResult.Value as MarqueDTO;
            Assert.IsNotNull(returnedDto);
            Assert.AreEqual("Reebok", returnedDto.Nom);

            var marqueInDb = await _context.Marques.FindAsync(returnedDto.IdMarque);
            Assert.IsNotNull(marqueInDb);
        }

        [TestMethod]
        public async Task ShouldUpdateMarque()
        {
            // Given: Une marque existante
            var marque = new Marque { Nom = "Nike Original" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var updatedDto = new MarqueDTO
            {
                IdMarque = marque.IdMarque,
                Nom = "Nike Modifié"
            };

            // When: On met à jour la marque
            var result = await _controller.Update(marque.IdMarque, updatedDto);

            // Then: La marque est mise à jour avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var marqueInDb = await _context.Marques.FindAsync(marque.IdMarque);
            Assert.IsNotNull(marqueInDb);
            Assert.AreEqual("Nike Modifié", marqueInDb.Nom);
        }

        [TestMethod]
        public async Task UpdateMarqueShouldReturnNotFoundWhenMarqueDoesNotExist()
        {
            // Given: Un DTO pour une marque qui n'existe pas
            var marqueDto = new MarqueDTO
            {
                IdMarque = 999,
                Nom = "Marque Inexistante"
            };

            // When: On essaie de mettre à jour la marque
            var result = await _controller.Update(999, marqueDto);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldDeleteMarque()
        {
            // Given: Une marque existante sans produits
            var marque = new Marque { Nom = "Marque à Supprimer" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();
            var marqueId = marque.IdMarque;

            // When: On supprime la marque
            var result = await _controller.Delete(marqueId);

            // Then: La marque est supprimée avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var marqueInDb = await _context.Marques.FindAsync(marqueId);
            Assert.IsNull(marqueInDb);
        }

        [TestMethod]
        public async Task ShouldDeleteMarqueWithProducts()
        {
            // Given: Une marque avec des produits et images
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var produit = new Produit
            {
                Nom = "Air Max",
                IdMarque = marque.IdMarque,
                IdTypeProduit = typeProduit.IdTypeProduit
            };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var image = new Image
            {
                Nom = "Image Air Max",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var marqueId = marque.IdMarque;
            var produitId = produit.IdProduit;
            var imageId = image.IdImage;

            // When: On supprime la marque
            var result = await _controller.Delete(marqueId);

            // Then: La marque, ses produits et leurs images sont supprimés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var marqueInDb = await _context.Marques.FindAsync(marqueId);
            Assert.IsNull(marqueInDb);

            var produitInDb = await _context.Produits.FindAsync(produitId);
            Assert.IsNull(produitInDb);

            var imageInDb = await _context.Images.FindAsync(imageId);
            Assert.IsNull(imageInDb);
        }

        [TestMethod]
        public async Task DeleteMarqueShouldReturnNotFoundWhenMarqueDoesNotExist()
        {
            // Given: Aucune marque en base

            // When: On essaie de supprimer une marque qui n'existe pas
            var result = await _controller.Delete(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetProduitsCount()
        {
            // Given: Une marque avec plusieurs produits
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var produits = new List<Produit>
            {
                new Produit { Nom = "Air Max", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit },
                new Produit { Nom = "Jordan", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit },
                new Produit { Nom = "Cortez", IdMarque = marque.IdMarque, IdTypeProduit = typeProduit.IdTypeProduit }
            };
            _context.Produits.AddRange(produits);
            await _context.SaveChangesAsync();

            // When: On récupère le nombre de produits
            var result = await _controller.GetProduitsCount(marque.IdMarque);

            // Then: Le bon nombre de produits est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public async Task GetProduitsCountShouldReturnZeroWhenMarqueHasNoProducts()
        {
            // Given: Une marque sans produits
            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            // When: On récupère le nombre de produits
            var result = await _controller.GetProduitsCount(marque.IdMarque);

            // Then: 0 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(0, count);
        }
    }
}