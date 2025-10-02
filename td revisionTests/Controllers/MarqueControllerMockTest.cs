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
        private Mock<IDataRepository<Marque>> _marqueRepository;
        private Mock<IDataRepository<Produit>> _produitRepository;
        private Mock<IDataRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _marqueRepository = new Mock<IDataRepository<Marque>>();
            _produitRepository = new Mock<IDataRepository<Produit>>();
            _imageRepository = new Mock<IDataRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new MarqueController(_mapper, _marqueRepository.Object, _produitRepository.Object, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetMarqueById()
        {
            // Given: Une marque enregistrée
            Marque marqueInDb = new()
            {
                IdMarque = 1,
                Nom = "Nike"
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(marqueInDb.IdMarque))
                .ReturnsAsync(marqueInDb);

            // When: On récupère la marque par son ID
            ActionResult<MarqueDTO> action = _controller.GetById(marqueInDb.IdMarque).GetAwaiter().GetResult();

            // Then: La marque est retournée avec un code 200
            _marqueRepository.Verify(repo => repo.GetByIdAsync(marqueInDb.IdMarque), Times.Once);

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var marqueDto = okResult.Value as MarqueDTO;
            Assert.IsNotNull(marqueDto);
            Assert.AreEqual("Nike", marqueDto.Nom);
        }

        [TestMethod]
        public void GetMarqueByIdShouldReturnNotFound()
        {
            // Given: Pas de marque trouvée par le manager
            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync(new ActionResult<Marque>((Marque)null));

            // When: On appelle la méthode GetById pour récupérer la marque
            ActionResult<MarqueDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllMarques()
        {
            // Given: Des marques enregistrées
            IEnumerable<Marque> marquesInDb = new List<Marque>
            {
                new Marque { IdMarque = 1, Nom = "Nike" },
                new Marque { IdMarque = 2, Nom = "Adidas" },
                new Marque { IdMarque = 3, Nom = "Puma" }
            };

            _marqueRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Marque>>(marquesInDb));

            // When: On récupère toutes les marques
            var action = _controller.GetAll().GetAwaiter().GetResult();

            // Then: Toutes les marques sont retournées
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var marqueDtos = okResult.Value as IEnumerable<MarqueDTO>;
            Assert.IsNotNull(marqueDtos);
            Assert.AreEqual(3, marqueDtos.Count());

            _marqueRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetMarqueByName()
        {
            // Given: Une marque avec un nom spécifique
            Marque marqueInDb = new()
            {
                IdMarque = 1,
                Nom = "Nike"
            };

            _marqueRepository
                .Setup(repo => repo.GetByStringAsync("Nike"))
                .ReturnsAsync(marqueInDb);

            // When: On recherche la marque par son nom
            ActionResult<MarqueDTO> action = _controller.GetByName("Nike").GetAwaiter().GetResult();

            // Then: La marque est retournée
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var marqueDto = okResult.Value as MarqueDTO;
            Assert.IsNotNull(marqueDto);
            Assert.AreEqual("Nike", marqueDto.Nom);

            _marqueRepository.Verify(repo => repo.GetByStringAsync("Nike"), Times.Once);
        }

        [TestMethod]
        public void ShouldAddMarque()
        {
            // Given: Une marque à ajouter
            MarqueDTO marqueDto = new() { Nom = "Reebok" };
            Marque marque = new() { IdMarque = 1, Nom = "Reebok" };

            _marqueRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Marque>()));

            // When: On ajoute la marque
            ActionResult<MarqueDTO> action = _controller.Add(marqueDto).GetAwaiter().GetResult();

            // Then: La marque est créée avec un code 201
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _marqueRepository.Verify(repo => repo.AddAsync(It.IsAny<Marque>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateMarque()
        {
            // Given: Une marque à mettre à jour
            Marque marqueToEdit = new()
            {
                IdMarque = 1,
                Nom = "Nike Original"
            };

            MarqueDTO updatedDto = new()
            {
                IdMarque = 1,
                Nom = "Nike Modifié"
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(marqueToEdit.IdMarque))
                .ReturnsAsync(marqueToEdit);

            _marqueRepository
                .Setup(repo => repo.UpdateAsync(marqueToEdit, It.IsAny<Marque>()));

            // When: On met à jour la marque
            IActionResult action = _controller.Update(marqueToEdit.IdMarque, updatedDto).GetAwaiter().GetResult();

            // Then: La marque est mise à jour avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(marqueToEdit.IdMarque), Times.Once);
            _marqueRepository.Verify(repo => repo.UpdateAsync(marqueToEdit, It.IsAny<Marque>()), Times.Once);
        }

        [TestMethod]
        public void UpdateMarqueShouldReturnNotFoundWhenMarqueDoesNotExist()
        {
            // Given: Une marque qui n'existe pas
            MarqueDTO marqueDto = new()
            {
                IdMarque = 999,
                Nom = "Marque Inexistante"
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Marque)null);

            // When: On essaie de mettre à jour la marque
            IActionResult action = _controller.Update(999, marqueDto).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _marqueRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Marque>(), It.IsAny<Marque>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteMarque()
        {
            // Given: Une marque sans produits
            var marqueInDb = new Marque
            {
                IdMarque = 1,
                Nom = "Nike"
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(marqueInDb.IdMarque))
                .ReturnsAsync(new ActionResult<Marque>(marqueInDb));

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(new List<Produit>()));

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>(new List<Image>()));

            _marqueRepository
                .Setup(repo => repo.DeleteAsync(marqueInDb))
                .Returns(Task.CompletedTask);

            // When: On supprime la marque
            IActionResult action = _controller.Delete(marqueInDb.IdMarque).GetAwaiter().GetResult();

            // Then: La marque est supprimée avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(marqueInDb.IdMarque), Times.Once);
            _marqueRepository.Verify(repo => repo.DeleteAsync(marqueInDb), Times.Once);
        }


        [TestMethod]
        public void ShouldDeleteMarqueWithProducts()
        {
            // Given: Une marque avec des produits et images
            Marque marqueInDb = new()
            {
                IdMarque = 1,
                Nom = "Nike"
            };

            List<Produit> produits = new()
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdMarque = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdMarque = 1 }
            };

            List<Image> images = new()
            {
                new Image { IdImage = 1, Nom = "Image 1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image 2", IdProduit = 2 }
            };

            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(marqueInDb.IdMarque))
                .ReturnsAsync(marqueInDb);

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

            _marqueRepository
                .Setup(repo => repo.DeleteAsync(marqueInDb));

            // When: On supprime la marque
            IActionResult action = _controller.Delete(marqueInDb.IdMarque).GetAwaiter().GetResult();

            // Then: La marque, ses produits et leurs images sont supprimés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(marqueInDb.IdMarque), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Exactly(2));
            _produitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Produit>()), Times.Exactly(2));
            _marqueRepository.Verify(repo => repo.DeleteAsync(marqueInDb), Times.Once);
        }

        [TestMethod]
        public void DeleteMarqueShouldReturnNotFoundWhenMarqueDoesNotExist()
        {
            // Given: Aucune marque en base
            _marqueRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Marque)null);

            // When: On essaie de supprimer une marque qui n'existe pas
            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _marqueRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _marqueRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Marque>()), Times.Never);
        }

        [TestMethod]
        public void ShouldGetProduitsCount()
        {
            // Given: Une marque avec plusieurs produits
            List<Produit> produits = new()
            {
                new Produit { IdProduit = 1, Nom = "Air Max", IdMarque = 1 },
                new Produit { IdProduit = 2, Nom = "Jordan", IdMarque = 1 },
                new Produit { IdProduit = 3, Nom = "Cortez", IdMarque = 1 },
                new Produit { IdProduit = 4, Nom = "Stan Smith", IdMarque = 2 }
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