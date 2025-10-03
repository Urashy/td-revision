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
    public class TypeProduitControllerMockTests
    {
        private TypeProduitController _controller;
        private Mock<INamedRepository<TypeProduit>> _typeProduitRepository;
        private Mock<INamedRepository<Produit>> _produitRepository;
        private Mock<IRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _typeProduitRepository = new Mock<INamedRepository<TypeProduit>>();
            _produitRepository = new Mock<INamedRepository<Produit>>();
            _imageRepository = new Mock<IRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new TypeProduitController(_mapper, _typeProduitRepository.Object, _produitRepository.Object, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetTypeProduitById()
        {
            // Given
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit))
                .ReturnsAsync(typeProduitInDb);

            // When
            ActionResult<TypeProduitDTO> action = _controller.GetById(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then
            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit), Times.Once);

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var typeProduitDto = okResult.Value as TypeProduitDTO;
            Assert.IsNotNull(typeProduitDto);
            Assert.AreEqual("Chaussure", typeProduitDto.Nom);
        }

        [TestMethod]
        public void GetTypeProduitByIdShouldReturnNotFound()
        {
            // Given
            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((TypeProduit)null);

            // When
            ActionResult<TypeProduitDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllTypeProduits()
        {
            // Given
            IEnumerable<TypeProduit> typeProduitsInDb = new List<TypeProduit>
            {
                new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" },
                new TypeProduit { IdTypeProduit = 2, Nom = "Vêtement" },
                new TypeProduit { IdTypeProduit = 3, Nom = "Accessoire" }
            };

            _typeProduitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(typeProduitsInDb);

            // When
            var action = _controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var typeProduitDtos = okResult.Value as IEnumerable<TypeProduitDTO>;
            Assert.IsNotNull(typeProduitDtos);
            Assert.AreEqual(3, typeProduitDtos.Count());

            _typeProduitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetTypeProduitByName()
        {
            // Given
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByNameAsync("Chaussure"))
                .ReturnsAsync(typeProduitInDb);

            // When
            ActionResult<TypeProduitDTO> action = _controller.GetByName("Chaussure").GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var typeProduitDto = okResult.Value as TypeProduitDTO;
            Assert.IsNotNull(typeProduitDto);
            Assert.AreEqual("Chaussure", typeProduitDto.Nom);

            _typeProduitRepository.Verify(repo => repo.GetByNameAsync("Chaussure"), Times.Once);
        }

        [TestMethod]
        public void ShouldAddTypeProduit()
        {
            // Given
            TypeProduitDTO typeProduitDto = new() { Nom = "Électronique" };

            _typeProduitRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TypeProduit>()));

            // When
            ActionResult<TypeProduitDTO> action = _controller.Add(typeProduitDto).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _typeProduitRepository.Verify(repo => repo.AddAsync(It.IsAny<TypeProduit>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateTypeProduit()
        {
            // Given
            TypeProduit typeProduitToEdit = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure Original"
            };

            TypeProduitDTO updatedDto = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure Modifié"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitToEdit.IdTypeProduit))
                .ReturnsAsync(typeProduitToEdit);

            _typeProduitRepository
                .Setup(repo => repo.UpdateAsync(typeProduitToEdit));

            // When
            IActionResult action = _controller.Update(typeProduitToEdit.IdTypeProduit, updatedDto).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitToEdit.IdTypeProduit), Times.Once);
            _typeProduitRepository.Verify(repo => repo.UpdateAsync(typeProduitToEdit), Times.Once);
        }

        [TestMethod]
        public void UpdateTypeProduitShouldReturnNotFoundWhenTypeProduitDoesNotExist()
        {
            // Given
            TypeProduitDTO typeProduitDto = new()
            {
                IdTypeProduit = 999,
                Nom = "Type Inexistant"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((TypeProduit)null);

            // When
            IActionResult action = _controller.Update(999, typeProduitDto).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _typeProduitRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TypeProduit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteTypeProduit()
        {
            // Given
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit))
                .ReturnsAsync(typeProduitInDb);

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Produit>());

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Image>());

            _typeProduitRepository
                .Setup(repo => repo.DeleteAsync(typeProduitInDb))
                .Returns(Task.CompletedTask);

            // When
            IActionResult action = _controller.Delete(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit), Times.Once);
            _typeProduitRepository.Verify(repo => repo.DeleteAsync(typeProduitInDb), Times.Once);
        }

        [TestMethod]
        public void ShouldDeleteTypeProduitWithProducts()
        {
            // Given
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            List<Produit> produits = new()
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdTypeProduit = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdTypeProduit = 1 }
            };

            List<Image> images = new()
            {
                new Image { IdImage = 1, Nom = "Image 1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image 2", IdProduit = 2 }
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit))
                .ReturnsAsync(typeProduitInDb);

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

            _typeProduitRepository
                .Setup(repo => repo.DeleteAsync(typeProduitInDb));

            // When
            IActionResult action = _controller.Delete(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Exactly(2));
            _produitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Produit>()), Times.Exactly(2));
            _typeProduitRepository.Verify(repo => repo.DeleteAsync(typeProduitInDb), Times.Once);
        }

        [TestMethod]
        public void DeleteTypeProduitShouldReturnNotFoundWhenTypeProduitDoesNotExist()
        {
            // Given
            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((TypeProduit)null);

            // When
            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _typeProduitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TypeProduit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldGetProduitsCount()
        {
            // Given
            List<Produit> produits = new()
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdTypeProduit = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdTypeProduit = 1 },
                new Produit { IdProduit = 3, Nom = "Cortez", IdTypeProduit = 1 },
                new Produit { IdProduit = 4, Nom = "Stan Smith", IdTypeProduit = 2 }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produits);

            // When
            var action = _controller.GetProduitsCount(1).GetAwaiter().GetResult();

            // Then
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(3, count);

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
