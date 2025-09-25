using Microsoft.VisualStudio.TestTools.UnitTesting;
using td_revision.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;
using td_revision.Models;

namespace td_revision.Controllers.Tests
{
    [TestClass()]
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
                Nom="Test",
                Description="Test description",
                NomPhoto="test.jpg",
            };

            _context.Produits.Add(produit);
            ActionResult<Produit> action = _controller.Get(produit.IdProduit).GetAwaiter().GetResult();


        }
    }
}