using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using td_revision.Controllers;
using td_revision.DTO;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;
using td_revision.Mapper;


namespace td_revisionTests.Controllers.Tests
{
    [TestClass()]
    public class ImageControllerTests
    {
        private ProduitsbdContext _context;
        private ImageController _controller;
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

            var repository = new ImageManager(_context);
            _controller = new ImageController(_mapper, repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ShouldGetImageById()
        {
            // Given: Une image enregistrée avec un produit
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var image = new Image
            {
                Nom = "Image Test",
                Url = "https://test.com/image.jpg",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            // When: On récupère l'image par son ID
            var result = await _controller.GetById(image.IdImage);

            // Then: L'image est retournée avec un code 200
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var imageDto = okResult.Value as ImageDTO;
            Assert.IsNotNull(imageDto);
            Assert.AreEqual("Image Test", imageDto.Nom);
        }

        [TestMethod]
        public async Task GetImageByIdShouldReturnNotFound()
        {
            // Given: Aucune image en base

            // When: On essaie de récupérer une image qui n'existe pas
            var result = await _controller.GetById(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldGetAllImages()
        {
            // Given: Plusieurs images enregistrées
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var images = new List<Image>
        {
            new Image { Nom = "Image 1", Url = "https://test.com/1.jpg", IdProduit = produit.IdProduit },
            new Image { Nom = "Image 2", Url = "https://test.com/2.jpg", IdProduit = produit.IdProduit }
        };
            _context.Images.AddRange(images);
            await _context.SaveChangesAsync();

            // When: On récupère toutes les images
            var result = await _controller.GetAll();

            // Then: Toutes les images sont retournées
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var imageDtos = okResult.Value as IEnumerable<ImageDTO>;
            Assert.IsNotNull(imageDtos);
            Assert.AreEqual(2, imageDtos.Count());
        }

        [TestMethod]
        public async Task ShouldGetImageByName()
        {
            // Given: Une image avec un nom spécifique
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var image = new Image
            {
                Nom = "Image Unique",
                Url = "https://test.com/unique.jpg",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            // When: On recherche l'image par son nom
            var result = await _controller.GetByName("Image Unique");

            // Then: L'image est retournée
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var imageDto = okResult.Value as ImageDTO;
            Assert.IsNotNull(imageDto);
            Assert.AreEqual("Image Unique", imageDto.Nom);
        }

        [TestMethod]
        public async Task ShouldGetImagesByProduitId()
        {
            // Given: Plusieurs images associées à un produit
            var produit1 = new Produit { Nom = "Produit 1" };
            var produit2 = new Produit { Nom = "Produit 2" };
            _context.Produits.AddRange(produit1, produit2);
            await _context.SaveChangesAsync();

            var images = new List<Image>
        {
            new Image { Nom = "Image P1-1", Url = "url1", IdProduit = produit1.IdProduit },
            new Image { Nom = "Image P1-2", Url = "url2", IdProduit = produit1.IdProduit },
            new Image { Nom = "Image P2-1", Url = "url3", IdProduit = produit2.IdProduit }
        };
            _context.Images.AddRange(images);
            await _context.SaveChangesAsync();

            // When: On récupère les images du produit 1
            var result = await _controller.GetByProduitId(produit1.IdProduit);

            // Then: Seules les images du produit 1 sont retournées
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var imageDtos = okResult.Value as IEnumerable<ImageDTO>;
            Assert.IsNotNull(imageDtos);
            Assert.AreEqual(2, imageDtos.Count());
        }

        [TestMethod]
        public async Task ShouldAddImage()
        {
            // Given: Un DTO d'image à ajouter
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var imageDto = new ImageDTO
            {
                Nom = "Nouvelle Image",
                Url = "https://test.com/new.jpg",
                IdProduit = produit.IdProduit
            };

            // When: On ajoute l'image
            var result = await _controller.Add(imageDto);

            // Then: L'image est créée avec un code 201
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedDto = createdResult.Value as ImageDTO;
            Assert.IsNotNull(returnedDto);
            Assert.AreEqual("Nouvelle Image", returnedDto.Nom);

            var imageInDb = await _context.Images.FindAsync(returnedDto.IdImage);
            Assert.IsNotNull(imageInDb);
        }

        [TestMethod]
        public async Task ShouldUpdateImage()
        {
            // Given: Une image existante
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var image = new Image
            {
                Nom = "Image Originale",
                Url = "https://test.com/original.jpg",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var updatedDto = new ImageDTO
            {
                IdImage = image.IdImage,
                Nom = "Image Modifiée",
                Url = "https://test.com/modified.jpg",
                IdProduit = produit.IdProduit
            };

            // When: On met à jour l'image
            var result = await _controller.Update(image.IdImage, updatedDto);

            // Then: L'image est mise à jour avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var imageInDb = await _context.Images.FindAsync(image.IdImage);
            Assert.IsNotNull(imageInDb);
            Assert.AreEqual("Image Modifiée", imageInDb.Nom);
        }

        [TestMethod]
        public async Task UpdateImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
            // Given: Un DTO pour une image qui n'existe pas
            var imageDto = new ImageDTO
            {
                IdImage = 999,
                Nom = "Image Inexistante",
                Url = "url",
                IdProduit = 1
            };

            // When: On essaie de mettre à jour l'image
            var result = await _controller.Update(999, imageDto);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ShouldDeleteImage()
        {
            // Given: Une image existante
            var produit = new Produit { Nom = "Test Produit" };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var image = new Image
            {
                Nom = "Image à Supprimer",
                Url = "url",
                IdProduit = produit.IdProduit
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            var imageId = image.IdImage;

            // When: On supprime l'image
            var result = await _controller.Delete(imageId);

            // Then: L'image est supprimée avec un code 204
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var imageInDb = await _context.Images.FindAsync(imageId);
            Assert.IsNull(imageInDb);
        }

        [TestMethod]
        public async Task DeleteImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
            // Given: Aucune image en base

            // When: On essaie de supprimer une image qui n'existe pas
            var result = await _controller.Delete(999);

            // Then: Un code 404 est retourné
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}