using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using td_revision.Controllers;
using td_revision.DTO;
using td_revision.Mapper;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revisionTests.Controllers.Tests
{
    [TestClass()]
    public class MarqueControllerMockTests
    {
        private MarqueController _controller;
        private Mock<INamedRepository<Marque>> _marqueRepository;
        private Mock<INamedRepository<Produit>> _produitRepository;
        private Mock<IRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _marqueRepository = new Mock<INamedRepository<Marque>>();
            _produitRepository = new Mock<INamedRepository<Produit>>();
            _imageRepository = new Mock<IRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new MarqueController(_mapper, _marqueRepository.Object, _produitRepository.Object, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetMarqueById()
        {
            var marqueInDb = new Marque { IdMarque = 1, Nom = "Nike" };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(marqueInDb.IdMarque))
                .ReturnsAsync(marqueInDb);

            var action = _controller.GetById(marqueInDb.IdMarque).GetAwaiter().GetResult();

            _marqueRepository.Verify(repo => repo.GetByIdAsync(marqueInDb.IdMarque), Times.Once);

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var ok = action.Result as OkObjectResult;
            var dto = ok.Value as MarqueDTO;
            Assert.AreEqual("Nike", dto.Nom);
        }

        [TestMethod]
        public void GetMarqueByIdShouldReturnNotFound()
        {
            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Marque)null);

            var action = _controller.GetById(999).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            _marqueRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllMarques()
        {
            var marquesInDb = new List<Marque>
            {
                new Marque { IdMarque = 1, Nom = "Nike" },
                new Marque { IdMarque = 2, Nom = "Adidas" },
                new Marque { IdMarque = 3, Nom = "Puma" }
            };

            _marqueRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(marquesInDb);

            var action = _controller.GetAll().GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var ok = action.Result as OkObjectResult;
            var dtos = ok.Value as IEnumerable<MarqueDTO>;
            Assert.AreEqual(3, dtos.Count());
        }

        [TestMethod]
        public void ShouldGetMarqueByName()
        {
            var marqueInDb = new Marque { IdMarque = 1, Nom = "Nike" };

            _marqueRepository
                .Setup(repo => repo.GetByNameAsync("Nike"))
                .ReturnsAsync(marqueInDb);

            var action = _controller.GetByName("Nike").GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var ok = action.Result as OkObjectResult;
            var dto = ok.Value as MarqueDTO;
            Assert.AreEqual("Nike", dto.Nom);
        }

        [TestMethod]
        public void ShouldAddMarque()
        {
            var dto = new MarqueDTO { Nom = "Reebok" };

            _marqueRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Marque>()));

            var action = _controller.Add(dto).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
            _marqueRepository.Verify(repo => repo.AddAsync(It.IsAny<Marque>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateMarque()
        {
            var marque = new Marque { IdMarque = 1, Nom = "Nike" };
            var dto = new MarqueDTO { IdMarque = 1, Nom = "Nike Modifié" };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(marque);

            _marqueRepository
                .Setup(repo => repo.UpdateAsync(marque));

            var action = _controller.Update(1, dto).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            _marqueRepository.Verify(repo => repo.UpdateAsync(marque), Times.Once);
        }

        [TestMethod]
        public void UpdateMarqueShouldReturnNotFound()
        {
            var dto = new MarqueDTO { IdMarque = 999, Nom = "Inexistante" };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Marque)null);

            var action = _controller.Update(999, dto).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
            _marqueRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Marque>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteMarque()
        {
            var marque = new Marque { IdMarque = 1, Nom = "Nike" };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(marque);

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Produit>());

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Image>());

            _marqueRepository
                .Setup(repo => repo.DeleteAsync(marque));

            var action = _controller.Delete(1).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            _marqueRepository.Verify(repo => repo.DeleteAsync(marque), Times.Once);
        }

        [TestMethod]
        public void ShouldDeleteMarqueWithProducts()
        {
            var marque = new Marque { IdMarque = 1, Nom = "Nike" };

            var produits = new List<Produit>
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdMarque = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdMarque = 1 }
            };

            var images = new List<Image>
            {
                new Image { IdImage = 1, Nom = "Img1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Img2", IdProduit = 2 }
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(marque);

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produits);

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(images);

            _imageRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Image>()));

            _produitRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Produit>()));

            _marqueRepository
                .Setup(repo => repo.DeleteAsync(marque));

            var action = _controller.Delete(1).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Exactly(2));
            _produitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Produit>()), Times.Exactly(2));
            _marqueRepository.Verify(repo => repo.DeleteAsync(marque), Times.Once);
        }

        [TestMethod]
        public void DeleteMarqueShouldReturnNotFound()
        {
            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Marque)null);

            var action = _controller.Delete(999).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        }

        [TestMethod]
        public void ShouldGetProduitsCount()
        {
            var produits = new List<Produit>
            {
                new Produit { IdProduit = 1, IdMarque = 1 },
                new Produit { IdProduit = 2, IdMarque = 1 },
                new Produit { IdProduit = 3, IdMarque = 1 },
                new Produit { IdProduit = 4, IdMarque = 2 }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produits);

            var action = _controller.GetProduitsCount(1).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var ok = action.Result as OkObjectResult;
            Assert.AreEqual(3, (int)ok.Value);
        }
    }
}
