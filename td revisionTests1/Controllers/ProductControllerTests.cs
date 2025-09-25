using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using td_revision.Controllers;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;

namespace td_revision.Controllers.Tests
{
    [TestClass()]
    [TestCategory("integration")]
    public class ProductControllerTests
    {
        private readonly ProduitsbdContext _context;
        private readonly ProductController _controller;
        private readonly ProductManager _manager;

        public ProductControllerTests()
        {
            _context = new ProduitsbdContext();
            _manager = new ProductManager(_context);
            _controller = new ProductController(_manager);

        }

        [TestMethod()]
        public void GetTest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,

            };

            _context.Produits.Add(produit);
            _context.SaveChanges();

            ActionResult<Produit> action = _controller.Get(produit.IdProduit).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Value, typeof(Produit));

            Produit returnedproduit = action.Value;
            Assert.AreEqual(produit.Nom, returnedproduit.Nom);
        }

        [TestMethod()]
        public void GetTest_NotFound()
        {
            ActionResult<Produit> action = _controller.Get(-1).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        }
        [TestMethod()]
        public void GetAllTest()
        {
            ActionResult<IEnumerable<Produit>> action = _controller.GetAll().GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Value, typeof(IEnumerable<Produit>));
            IEnumerable<Produit> produits = action.Value;
            Assert.IsTrue(produits.Any());
        }
        [TestMethod()]
        public void GetAllTest_NotFound() {

            ActionResult<IEnumerable<Produit>> action = _controller.GetAll().GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult)); 
                }
        [TestMethod()]
        public void DeleteTest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,
            };
            _context.Produits.Add(produit);
            _context.SaveChanges();
            IActionResult action = _controller.Delete(produit.IdProduit).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            Produit? deletedproduit = _context.Produits.Find(produit.IdProduit);
            Assert.IsNull(deletedproduit);
        }
        [TestMethod()]
        public void DeleteTest_NotFound()
        {
            IActionResult action = _controller.Delete(-1).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        }
        [TestMethod()]
        public void PostTest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,
            };
            ActionResult<Produit> action = _controller.Post(produit).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
            CreatedAtActionResult createdAtAction = (CreatedAtActionResult)action.Result;
            Assert.IsInstanceOfType(createdAtAction.Value, typeof(Produit));
            Produit createdproduit = (Produit)createdAtAction.Value;
            Assert.AreEqual(produit.Nom, createdproduit.Nom);
            Produit? dbproduit = _context.Produits.Find(createdproduit.IdProduit);
            Assert.IsNotNull(dbproduit);
        }
        [TestMethod()]
        public void PutTest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,
            };
            _context.Produits.Add(produit);
            _context.SaveChanges();
            Produit updatedproduit = new Produit()
            {
                IdProduit = produit.IdProduit,
                Nom = "Updated Test",
                Description = "Updated description",
                NomPhoto = "updated_test.jpg",
                UrlPhoto = "http://example.com/updated_test.jpg",
                StockReel = 15,
                StockMini = 7,
                StockMaxi = 25,
            };
            IActionResult action = _controller.Put(produit.IdProduit, updatedproduit).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            Produit? dbproduit = _context.Produits.Find(produit.IdProduit);
            Assert.IsNotNull(dbproduit);
            Assert.AreEqual(updatedproduit.Nom, dbproduit.Nom);
            Assert.AreEqual(updatedproduit.Description, dbproduit.Description);
        }
        [TestMethod()]
        public void PutTest_BadRequest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,
            };
            _context.Produits.Add(produit);
            _context.SaveChanges();
            Produit updatedproduit = new Produit()
            {
                IdProduit = produit.IdProduit + 1, // Intentionally incorrect ID
                Nom = "Updated Test",
                Description = "Updated description",
                NomPhoto = "updated_test.jpg",
                UrlPhoto = "http://example.com/updated_test.jpg",
                StockReel = 15,
                StockMini = 7,
                StockMaxi = 25,
            };
            IActionResult action = _controller.Put(produit.IdProduit, updatedproduit).GetAwaiter().GetResult();
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        }
        [TestMethod()]
        public void DeleteTest_BadRequest()
        {
            Produit produit = new Produit()
            {
                Nom = "Test",
                Description = "Test description",
                NomPhoto = "test.jpg",
                UrlPhoto = "http://example.com/test.jpg",
                StockReel = 10,
                StockMini = 5,
                StockMaxi = 20,
            };
            _context.Produits.Add(produit);
            _context.SaveChanges();
            IActionResult action = _controller.Delete(produit.IdProduit + 1).GetAwaiter().GetResult(); // Intentionally incorrect ID
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        }
        
        }
}