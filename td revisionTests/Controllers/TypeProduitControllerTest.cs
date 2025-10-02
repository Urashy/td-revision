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

namespace td_revision.Controllers.Tests
{
    [TestClass()]
    public class TypeProduitControllerTests
    {
        private ProduitsbdContext _context;
        private TypeProduitController _controller;
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

            var typeProduitRepository = new TypeProduitManager(_context);
            _produitRepository = new ProduitManager(_context);
            _imageRepository = new ImageManager(_context);
            _controller = new TypeProduitController(_mapper, typeProduitRepository, _produitRepository, _imageRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ShouldGetTypeProduitById()
        {
            // Given: Un type de produit enregistré
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            // When: On récupère le type de produit par son ID
            var result = await _controller.GetById(typeProduit.IdTypeProduit);

            // Then: Le type de produit est retourné avec un code 200
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var typeProduitDto = okResult.Value as TypeProduitDTO;
            Assert.IsNotNull(typeProduitDto);
            Assert.AreEqual("Chaussure", typeProduitDto.Nom);
        }

        [TestMethod]
        public async Task GetTypeProduitByIdShouldReturnNotFound()
        {
            // Given: Aucun type de produit en base

            // When: On essaie de récupérer un type de produit qui n'existe pas
            var result = await _controller.GetById(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetAllTypeProduits()
        {
            // Given: Plusieurs types de produits enregistrés
            var typeProduits = new List<TypeProduit>
            {
                new TypeProduit { Nom = "Chaussure" },
                new TypeProduit { Nom = "Vêtement" },
                new TypeProduit { Nom = "Accessoire" }
            };
            _context.TypeProduits.AddRange(typeProduits);
            await _context.SaveChangesAsync();

            // When: On récupère tous les types de produits
            var result = await _controller.GetAll();

            // Then: Tous les types de produits sont retournés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var typeProduitDtos = okResult.Value as IEnumerable<TypeProduitDTO>;
            Assert.IsNotNull(typeProduitDtos);
            Assert.AreEqual(3, typeProduitDtos.Count());
        }

        [TestMethod]
        public async Task ShouldGetTypeProduitByName()
        {
            // Given: Un type de produit avec un nom spécifique
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            // When: On recherche le type de produit par son nom
            var result = await _controller.GetByName("Chaussure");

            // Then: Le type de produit est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var typeProduitDto = okResult.Value as TypeProduitDTO;
            Assert.IsNotNull(typeProduitDto);
            Assert.AreEqual("Chaussure", typeProduitDto.Nom);
        }

        [TestMethod]
        public async Task ShouldAddTypeProduit()
        {
            // Given: Un DTO de type de produit à ajouter
            var typeProduitDto = new TypeProduitDTO { Nom = "Électronique" };

            // When: On ajoute le type de produit
            var result = await _controller.Add(typeProduitDto);

            // Then: Le type de produit est créé avec un code 201
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedDto = createdResult.Value as TypeProduitDTO;
            Assert.IsNotNull(returnedDto);
            Assert.AreEqual("Électronique", returnedDto.Nom);

            var typeProduitInDb = await _context.TypeProduits.FindAsync(returnedDto.IdTypeProduit);
            Assert.IsNotNull(typeProduitInDb);
        }

        [TestMethod]
        public async Task ShouldUpdateTypeProduit()
        {
            // Given: Un type de produit existant
            var typeProduit = new TypeProduit { Nom = "Chaussure Original" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            var updatedDto = new TypeProduitDTO
            {
                IdTypeProduit = typeProduit.IdTypeProduit,
                Nom = "Chaussure Modifié"
            };

            // When: On met à jour le type de produit
            var result = await _controller.Update(typeProduit.IdTypeProduit, updatedDto);

            // Then: Le type de produit est mis à jour avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var typeProduitInDb = await _context.TypeProduits.FindAsync(typeProduit.IdTypeProduit);
            Assert.IsNotNull(typeProduitInDb);
            Assert.AreEqual("Chaussure Modifié", typeProduitInDb.Nom);
        }

        [TestMethod]
        public async Task UpdateTypeProduitShouldReturnNotFoundWhenTypeProduitDoesNotExist()
        {
            // Given: Un DTO pour un type de produit qui n'existe pas
            var typeProduitDto = new TypeProduitDTO
            {
                IdTypeProduit = 999,
                Nom = "Type Inexistant"
            };

            // When: On essaie de mettre à jour le type de produit
            var result = await _controller.Update(999, typeProduitDto);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldDeleteTypeProduit()
        {
            // Given: Un type de produit existant sans produits
            var typeProduit = new TypeProduit { Nom = "Type à Supprimer" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();
            var typeProduitId = typeProduit.IdTypeProduit;

            // When: On supprime le type de produit
            var result = await _controller.Delete(typeProduitId);

            // Then: Le type de produit est supprimé avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var typeProduitInDb = await _context.TypeProduits.FindAsync(typeProduitId);
            Assert.IsNull(typeProduitInDb);
        }

        [TestMethod]
        public async Task ShouldDeleteTypeProduitWithProducts()
        {
            // Given: Un type de produit avec des produits et images
            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var typeProduit = new TypeProduit { Nom = "Chaussure" };
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

            var image = new Image
            {
                Nom = "Image Air Max",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var typeProduitId = typeProduit.IdTypeProduit;
            var produitId = produit.IdProduit;
            var imageId = image.IdImage;

            // When: On supprime le type de produit
            var result = await _controller.Delete(typeProduitId);

            // Then: Le type de produit, ses produits et leurs images sont supprimés
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var typeProduitInDb = await _context.TypeProduits.FindAsync(typeProduitId);
            Assert.IsNull(typeProduitInDb);

            var produitInDb = await _context.Produits.FindAsync(produitId);
            Assert.IsNull(produitInDb);

            var imageInDb = await _context.Images.FindAsync(imageId);
            Assert.IsNull(imageInDb);
        }

        [TestMethod]
        public async Task DeleteTypeProduitShouldReturnNotFoundWhenTypeProduitDoesNotExist()
        {
            // Given: Aucun type de produit en base

            // When: On essaie de supprimer un type de produit qui n'existe pas
            var result = await _controller.Delete(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetProduitsCount()
        {
            // Given: Un type de produit avec plusieurs produits
            var marque = new Marque { Nom = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
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
            var result = await _controller.GetProduitsCount(typeProduit.IdTypeProduit);

            // Then: Le bon nombre de produits est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public async Task GetProduitsCountShouldReturnZeroWhenTypeProduitHasNoProducts()
        {
            // Given: Un type de produit sans produits
            var typeProduit = new TypeProduit { Nom = "Chaussure" };
            _context.TypeProduits.Add(typeProduit);
            await _context.SaveChangesAsync();

            // When: On récupère le nombre de produits
            var result = await _controller.GetProduitsCount(typeProduit.IdTypeProduit);

            // Then: 0 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(0, count);
        }
    }
}