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
    public class ImageControllerMockTests
    {
        private ImageController _controller;
        private Mock<IDataRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _imageRepository = new Mock<IDataRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new ImageController(_mapper, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetImageById()
        {
            // Given: Une image enregistrée
            Image imageInDb = new()
            {
                IdImage = 1,
                Nom = "Image Test",
                Url = "https://test.com/image.jpg",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.GetByIdAsync(imageInDb.IdImage))
                .ReturnsAsync(imageInDb);

            // When: On récupère l'image par son ID
            ActionResult<ImageDTO> action = _controller.GetById(imageInDb.IdImage).GetAwaiter().GetResult();

            // Then: L'image est retournée avec un code 200
            _imageRepository.Verify(repo => repo.GetByIdAsync(imageInDb.IdImage), Times.Once);

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var imageDto = okResult.Value as ImageDTO;
            Assert.IsNotNull(imageDto);
            Assert.AreEqual("Image Test", imageDto.Nom);
        }

        [TestMethod]
        public void GetImageByIdShouldReturnNotFound()
        {
            // Given: Pas d'image trouvée par le manager
            _imageRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync(new ActionResult<Image>((Image)null));

            // When: On appelle la méthode GetById pour récupérer l'image
            ActionResult<ImageDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllImages()
        {
            // Given: Des images enregistrées
            IEnumerable<Image> imagesInDb = new List<Image>
            {
                new Image { IdImage = 1, Nom = "Image 1", Url = "https://test.com/1.jpg", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image 2", Url = "https://test.com/2.jpg", IdProduit = 1 },
                new Image { IdImage = 3, Nom = "Image 3", Url = "https://test.com/3.jpg", IdProduit = 2 }
            };

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>(imagesInDb));

            // When: On récupère toutes les images
            var action = _controller.GetAll().GetAwaiter().GetResult();

            // Then: Toutes les images sont retournées
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var imageDtos = okResult.Value as IEnumerable<ImageDTO>;
            Assert.IsNotNull(imageDtos);
            Assert.AreEqual(3, imageDtos.Count());

            _imageRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetImageByName()
        {
            // Given: Une image avec un nom spécifique
            Image imageInDb = new()
            {
                IdImage = 1,
                Nom = "Image Unique",
                Url = "https://test.com/unique.jpg",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.GetByStringAsync("Image Unique"))
                .ReturnsAsync(imageInDb);

            // When: On recherche l'image par son nom
            ActionResult<ImageDTO> action = _controller.GetByName("Image Unique").GetAwaiter().GetResult();

            // Then: L'image est retournée
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var imageDto = okResult.Value as ImageDTO;
            Assert.IsNotNull(imageDto);
            Assert.AreEqual("Image Unique", imageDto.Nom);

            _imageRepository.Verify(repo => repo.GetByStringAsync("Image Unique"), Times.Once);
        }

        [TestMethod]
        public void ShouldGetImagesByProduitId()
        {
            // Given: Plusieurs images associées à différents produits
            IEnumerable<Image> imagesInDb = new List<Image>
            {
                new Image { IdImage = 1, Nom = "Image P1-1", Url = "url1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image P1-2", Url = "url2", IdProduit = 1 },
                new Image { IdImage = 3, Nom = "Image P2-1", Url = "url3", IdProduit = 2 }
            };

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>(imagesInDb));

            // When: On récupère les images du produit 1
            var action = _controller.GetByProduitId(1).GetAwaiter().GetResult();

            // Then: Seules les images du produit 1 sont retournées
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var imageDtos = okResult.Value as IEnumerable<ImageDTO>;
            Assert.IsNotNull(imageDtos);
            Assert.AreEqual(2, imageDtos.Count());

            _imageRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void GetImagesByProduitIdShouldReturnNotFoundWhenNoImages()
        {
            // Given: Aucune image en base
            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Image>>((IEnumerable<Image>)null));

            // When: On essaie de récupérer les images d'un produit
            var action = _controller.GetByProduitId(1).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldAddImage()
        {
            // Given: Une image à ajouter
            ImageDTO imageDto = new()
            {
                Nom = "Nouvelle Image",
                Url = "https://test.com/new.jpg",
                IdProduit = 1
            };

            Image image = new()
            {
                IdImage = 1,
                Nom = "Nouvelle Image",
                Url = "https://test.com/new.jpg",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Image>()));

            // When: On ajoute l'image
            ActionResult<ImageDTO> action = _controller.Add(imageDto).GetAwaiter().GetResult();

            // Then: L'image est créée avec un code 201
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _imageRepository.Verify(repo => repo.AddAsync(It.IsAny<Image>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateImage()
        {
            // Given: Une image à mettre à jour
            Image imageToEdit = new()
            {
                IdImage = 1,
                Nom = "Image Originale",
                Url = "https://test.com/original.jpg",
                IdProduit = 1
            };

            ImageDTO updatedDto = new()
            {
                IdImage = 1,
                Nom = "Image Modifiée",
                Url = "https://test.com/modified.jpg",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.GetByIdAsync(imageToEdit.IdImage))
                .ReturnsAsync(imageToEdit);

            _imageRepository
                .Setup(repo => repo.UpdateAsync(imageToEdit, It.IsAny<Image>()));

            // When: On met à jour l'image
            IActionResult action = _controller.Update(imageToEdit.IdImage, updatedDto).GetAwaiter().GetResult();

            // Then: L'image est mise à jour avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(imageToEdit.IdImage), Times.Once);
            _imageRepository.Verify(repo => repo.UpdateAsync(imageToEdit, It.IsAny<Image>()), Times.Once);
        }

        [TestMethod]
        public void UpdateImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
            // Given: Une image qui n'existe pas
            ImageDTO imageDto = new()
            {
                IdImage = 999,
                Nom = "Image Inexistante",
                Url = "url",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Image)null);

            // When: On essaie de mettre à jour l'image
            IActionResult action = _controller.Update(999, imageDto).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _imageRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Image>(), It.IsAny<Image>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteImage()
        {
            // Given: Une image enregistrée
            Image imageInDb = new()
            {
                IdImage = 1,
                Nom = "Image à Supprimer",
                Url = "url",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.GetByIdAsync(imageInDb.IdImage))
                .ReturnsAsync(imageInDb);

            _imageRepository
                .Setup(repo => repo.DeleteAsync(imageInDb));

            // When: On supprime l'image
            IActionResult action = _controller.Delete(imageInDb.IdImage).GetAwaiter().GetResult();

            // Then: L'image est supprimée avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(imageInDb.IdImage), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(imageInDb), Times.Once);
        }

        [TestMethod]
        public void DeleteImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
            // Given: Aucune image en base
            _imageRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Image)null);

            // When: On essaie de supprimer une image qui n'existe pas
            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Never);
        }
    }
}