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
        private Mock<IRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _imageRepository = new Mock<IRepository<Image>>();

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

            // When
            ActionResult<ImageDTO> action = _controller.GetById(imageInDb.IdImage).GetAwaiter().GetResult();

            // Then
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
            _imageRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Image)null);

            ActionResult<ImageDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllImages()
        {
            IEnumerable<Image> imagesInDb = new List<Image>
            {
                new Image { IdImage = 1, Nom = "Image 1", Url = "https://test.com/1.jpg", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image 2", Url = "https://test.com/2.jpg", IdProduit = 1 },
                new Image { IdImage = 3, Nom = "Image 3", Url = "https://test.com/3.jpg", IdProduit = 2 }
            };

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(imagesInDb);

            var action = _controller.GetAll().GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var imageDtos = okResult.Value as IEnumerable<ImageDTO>;
            Assert.IsNotNull(imageDtos);
            Assert.AreEqual(3, imageDtos.Count());

            _imageRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }


        [TestMethod]
        public void ShouldGetImagesByProduitId()
        {
            IEnumerable<Image> imagesInDb = new List<Image>
            {
                new Image { IdImage = 1, Nom = "Image P1-1", Url = "url1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image P1-2", Url = "url2", IdProduit = 1 },
                new Image { IdImage = 3, Nom = "Image P2-1", Url = "url3", IdProduit = 2 }
            };

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(imagesInDb);

            var action = _controller.GetByProduitId(1).GetAwaiter().GetResult();

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
            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((IEnumerable<Image>)null);

            var action = _controller.GetByProduitId(1).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldAddImage()
        {
            ImageDTO imageDto = new()
            {
                Nom = "Nouvelle Image",
                Url = "https://test.com/new.jpg",
                IdProduit = 1
            };

            _imageRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Image>()));

            ActionResult<ImageDTO> action = _controller.Add(imageDto).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _imageRepository.Verify(repo => repo.AddAsync(It.IsAny<Image>()), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateImage()
        {
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
                .Setup(repo => repo.UpdateAsync(imageToEdit));

            IActionResult action = _controller.Update(imageToEdit.IdImage, updatedDto).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(imageToEdit.IdImage), Times.Once);
            _imageRepository.Verify(repo => repo.UpdateAsync(imageToEdit), Times.Once);
        }

        [TestMethod]
        public void UpdateImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
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

            IActionResult action = _controller.Update(999, imageDto).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _imageRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Image>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteImage()
        {
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

            IActionResult action = _controller.Delete(imageInDb.IdImage).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(imageInDb.IdImage), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(imageInDb), Times.Once);
        }

        [TestMethod]
        public void DeleteImageShouldReturnNotFoundWhenImageDoesNotExist()
        {
            _imageRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Image)null);

            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _imageRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Never);
        }
    }
}
