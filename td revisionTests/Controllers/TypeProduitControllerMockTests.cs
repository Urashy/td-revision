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
        private Mock<IDataRepository<TypeProduit>> _typeProduitRepository;
        private Mock<IDataRepository<Produit>> _produitRepository;
        private Mock<IDataRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _typeProduitRepository = new Mock<IDataRepository<TypeProduit>>();
            _produitRepository = new Mock<IDataRepository<Produit>>();
            _imageRepository = new Mock<IDataRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new TypeProduitController(_mapper, _typeProduitRepository.Object, _produitRepository.Object, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetTypeProduitById()
        {
            // Given: Un type de produit enregistré
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit))
                .ReturnsAsync(typeProduitInDb);

            // When: On récupère le type de produit par son ID
            ActionResult<TypeProduitDTO> action = _controller.GetById(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then: Le type de produit est retourné avec un code 200
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
            // Given: Pas de type de produit trouvé par le manager
            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));

            // When: On appelle la méthode GetById pour récupérer le type de produit
            ActionResult<TypeProduitDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllTypeProduits()
        {
            // Given: Des types de produits enregistrés
            IEnumerable<TypeProduit> typeProduitsInDb = new List<TypeProduit>
            {
                new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" },
                new TypeProduit { IdTypeProduit = 2, Nom = "Vêtement" },
                new TypeProduit { IdTypeProduit = 3, Nom = "Accessoire" }
            };

            _typeProduitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<TypeProduit>>(typeProduitsInDb));

            // When: On récupère tous les types de produits
            var action = _controller.GetAll().GetAwaiter().GetResult();

            // Then: Tous les types de produits sont retournés
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
            // Given: Un type de produit avec un nom spécifique
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByStringAsync("Chaussure"))
                .ReturnsAsync(typeProduitInDb);

            // When: On recherche le type de produit par son nom
            ActionResult<TypeProduitDTO> action = _controller.GetByName("Chaussure").GetAwaiter().GetResult();

            // Then: Le type de produit est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var typeProduitDto = okResult.Value as TypeProduitDTO;
            Assert.IsNotNull(typeProduitDto);
            Assert.AreEqual("Chaussure", typeProduitDto.Nom);

            _typeProduitRepository.Verify(repo => repo.GetByStringAsync("Chaussure"), Times.Once);
        }

        [TestMethod]
        public void ShouldAddTypeProduit()
        {
            // Given: Un type de produit à ajouter
            TypeProduitDTO typeProduitDto = new() { Nom = "Électronique" };
            TypeProduit typeProduit = new() { IdTypeProduit = 1, Nom = "Électronique" };

            _typeProduitRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TypeProduit>()));

            // When: On ajoute le type de produit
            ActionResult<TypeProduitDTO> action = _controller.Add(typeProduitDto).GetAwaiter().GetResult();

            // Then: Le type de produit est créé avec un code 201
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _typeProduitRepository.Verify(repo => repo.AddAsync(It.IsAny<TypeProduit>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateTypeProduit()
        {
            // Given: Un type de produit à mettre à jour
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
                .Setup(repo => repo.UpdateAsync(typeProduitToEdit, It.IsAny<TypeProduit>()));

            // When: On met à jour le type de produit
            IActionResult action = _controller.Update(typeProduitToEdit.IdTypeProduit, updatedDto).GetAwaiter().GetResult();

            // Then: Le type de produit est mis à jour avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitToEdit.IdTypeProduit), Times.Once);
            _typeProduitRepository.Verify(repo => repo.UpdateAsync(typeProduitToEdit, It.IsAny<TypeProduit>()), Times.Once);
        }

        [TestMethod]
        public void UpdateTypeProduitShouldReturnNotFoundWhenTypeProduitDoesNotExist()
        {
            // Given: Un type de produit qui n'existe pas
            TypeProduitDTO typeProduitDto = new()
            {
                IdTypeProduit = 999,
                Nom = "Type Inexistant"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((TypeProduit)null);

            // When: On essaie de mettre à jour le type de produit
            IActionResult action = _controller.Update(999, typeProduitDto).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _typeProduitRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TypeProduit>(), It.IsAny<TypeProduit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteTypeProduit()
        {
            // Given: Un type de produit sans produits
            TypeProduit typeProduitInDb = new()
            {
                IdTypeProduit = 1,
                Nom = "Chaussure"
            };

            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit))
                .ReturnsAsync(new ActionResult<TypeProduit>(typeProduitInDb));

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(new List<Produit>()));

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>(new List<Image>()));

            _typeProduitRepository
                .Setup(repo => repo.DeleteAsync(typeProduitInDb))
                .Returns(Task.CompletedTask);

            // When: On supprime le type de produit
            IActionResult action = _controller.Delete(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then: Le type de produit est supprimé avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(typeProduitInDb.IdTypeProduit), Times.Once);
            _typeProduitRepository.Verify(repo => repo.DeleteAsync(typeProduitInDb), Times.Once);
        }


        [TestMethod]
        public void ShouldDeleteTypeProduitWithProducts()
        {
            // Given: Un type de produit avec des produits et images
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
                .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produits));

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>(images));

            _imageRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Image>()));

            _produitRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Produit>()));

            _typeProduitRepository
                .Setup(repo => repo.DeleteAsync(typeProduitInDb));

            // When: On supprime le type de produit
            IActionResult action = _controller.Delete(typeProduitInDb.IdTypeProduit).GetAwaiter().GetResult();

            // Then: Le type de produit, ses produits et leurs images sont supprimés
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
            // Given: Aucun type de produit en base
            _typeProduitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((TypeProduit)null);

            // When: On essaie de supprimer un type de produit qui n'existe pas
            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _typeProduitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _typeProduitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TypeProduit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldGetProduitsCount()
        {
            // Given: Un type de produit avec plusieurs produits
            List<Produit> produits = new()
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdTypeProduit = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdTypeProduit = 1 },
                new Produit { IdProduit = 3, Nom = "Cortez", IdTypeProduit = 1 },
                new Produit { IdProduit = 4, Nom = "Stan Smith", IdTypeProduit = 2 }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produits));

            // When: On récupère le nombre de produits
            var action = _controller.GetProduitsCount(1).GetAwaiter().GetResult();

            // Then: Le bon nombre de produits est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var count = (int)okResult.Value;
            Assert.AreEqual(3, count);

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}