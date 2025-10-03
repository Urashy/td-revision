using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using td_revision.Controllers;
using td_revision.DTO.Produit;
using td_revision.Mapper;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revisionTests.Controllers.Tests
{
    [TestClass()]
    public class ProduitControllerMockTests
    {
        private ProduitController _controller;
        private Mock<INamedRepository<Produit>> _produitRepository;
        private Mock<INamedRepository<Marque>> _marqueRepository;
        private Mock<INamedRepository<TypeProduit>> _typeProduitRepository;
        private Mock<IRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _produitRepository = new Mock<INamedRepository<Produit>>();
            _marqueRepository = new Mock<INamedRepository<Marque>>();
            _typeProduitRepository = new Mock<INamedRepository<TypeProduit>>();
            _imageRepository = new Mock<IRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new ProduitController(
                _mapper,
                _produitRepository.Object,
                _marqueRepository.Object,
                _typeProduitRepository.Object,
                _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetProduitById()
        {
            // Given: Un produit enregistré
            Produit produitInDb = new()
            {
                IdProduit = 1,
                Nom = "Air Max",
                Description = "Une superbe chaussure",
                IdMarque = 1,
                IdTypeProduit = 1,
                Stock = 50,
                StockMini = 10,
                StockMaxi = 100,
                MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
            };

            _produitRepository
                .Setup(repo => repo.GetByIdAsync(produitInDb.IdProduit))
                .ReturnsAsync(produitInDb);

            // When: On récupère le produit par son ID
            ActionResult<ProduitDetailDTO> action = _controller.GetById(produitInDb.IdProduit).GetAwaiter().GetResult();

            // Then: Le produit est retourné avec un code 200
            _produitRepository.Verify(repo => repo.GetByIdAsync(produitInDb.IdProduit), Times.Once);

            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDto = okResult.Value as ProduitDetailDTO;
            Assert.IsNotNull(produitDto);
            Assert.AreEqual("Air Max", produitDto.Nom);
            Assert.AreEqual("Nike", produitDto.Marque);
            Assert.AreEqual("Chaussure", produitDto.Type);
        }

        [TestMethod]
        public void GetProduitByIdShouldReturnNotFound()
        {
            // Given: Pas de produit trouvé par le manager
            _produitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Produit)null);


            // When: On appelle la méthode GetById pour récupérer le produit
            ActionResult<ProduitDetailDTO> action = _controller.GetById(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public void ShouldGetAllProduits()
        {
            // Given: Des produits enregistrés
            IEnumerable<Produit> produitsInDb = new List<Produit>
            {
                new Produit
                {
                    IdProduit = 1,
                    Nom = "Air Max",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 50,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                },
                new Produit
                {
                    IdProduit = 2,
                    Nom = "Jordan",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 30,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produitsInDb);

            // When: On récupère tous les produits
            var action = _controller.GetAll().GetAwaiter().GetResult();

            // Then: Tous les produits sont retournés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(2, produitDtos.Count());

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetProduitByName()
        {
            // Given: Un produit avec un nom spécifique
            Produit produitInDb = new()
            {
                IdProduit = 1,
                Nom = "Air Max",
                IdMarque = 1,
                IdTypeProduit = 1,
                MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
            };

            _produitRepository
                .Setup(repo => repo.GetByNameAsync("Air Max"))
                .ReturnsAsync(produitInDb);

            // When: On recherche le produit par son nom
            ActionResult<ProduitDetailDTO> action = _controller.GetByName("Air Max").GetAwaiter().GetResult();

            // Then: Le produit est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDto = okResult.Value as ProduitDetailDTO;
            Assert.IsNotNull(produitDto);
            Assert.AreEqual("Air Max", produitDto.Nom);

            _produitRepository.Verify(repo => repo.GetByNameAsync("Air Max"), Times.Once);
        }

        [TestMethod]
        public void ShouldAddProduit()
        {
            // Given: Un produit à ajouter
            ProduitPostDTO produitDto = new()
            {
                Nom = "Air Max 90",
                Description = "Nouvelle chaussure",
                Marque = "Nike",
                Type = "Chaussure",
                Stock = 50,
                StockMini = 10,
                StockMaxi = 100
            };

            Marque marque = new() { IdMarque = 1, Nom = "Nike" };
            TypeProduit typeProduit = new() { IdTypeProduit = 1, Nom = "Chaussure" };

            _marqueRepository
                .Setup(repo => repo.GetByNameAsync("Nike"))
                .ReturnsAsync(marque);

            _typeProduitRepository
                .Setup(repo => repo.GetByNameAsync("Chaussure"))
                .ReturnsAsync(typeProduit);

            _produitRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Produit>()));

            // When: On ajoute le produit
            ActionResult<ProduitDetailDTO> action = _controller.Add(produitDto).GetAwaiter().GetResult();

            // Then: Le produit est créé avec un code 201
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _marqueRepository.Verify(repo => repo.GetByNameAsync("Nike"), Times.Once);
            _typeProduitRepository.Verify(repo => repo.GetByNameAsync("Chaussure"), Times.Once);
            _produitRepository.Verify(repo => repo.AddAsync(It.IsAny<Produit>()), Times.Once);
        }

        [TestMethod]
        public void AddProduitShouldReturnBadRequestWhenStockMiniGreaterThanStockMaxi()
        {
            // Given: Un DTO avec stock mini > stock maxi
            ProduitPostDTO produitDto = new()
            {
                Nom = "Air Max",
                Marque = "Nike",
                Type = "Chaussure",
                StockMini = 100,
                StockMaxi = 50
            };

            // When: On essaie d'ajouter le produit
            ActionResult<ProduitDetailDTO> action = _controller.Add(produitDto).GetAwaiter().GetResult();

            // Then: Un code 400 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));

            _produitRepository.Verify(repo => repo.AddAsync(It.IsAny<Produit>()), Times.Never);
        }

        [TestMethod]
        public void AddProduitShouldReturnBadRequestWhenMarqueNotFound()
        {
            // Given: Un DTO avec une marque inexistante
            ProduitPostDTO produitDto = new()
            {
                Nom = "Air Max",
                Marque = "MarqueInexistante",
                Type = "Chaussure",
                StockMini = 10,
                StockMaxi = 100
            };

            _marqueRepository
                .Setup(repo => repo.GetByNameAsync("MarqueInexistante"))
                .ReturnsAsync((Marque)null);

            // When: On essaie d'ajouter le produit
            ActionResult<ProduitDetailDTO> action = _controller.Add(produitDto).GetAwaiter().GetResult();

            // Then: Un code 400 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));

            _marqueRepository.Verify(repo => repo.GetByNameAsync("MarqueInexistante"), Times.Once);
            _produitRepository.Verify(repo => repo.AddAsync(It.IsAny<Produit>()), Times.Never);
        }

        [TestMethod]
        public void AddProduitShouldReturnBadRequestWhenTypeNotFound()
        {
            // Given: Un DTO avec un type inexistant
            ProduitPostDTO produitDto = new()
            {
                Nom = "Air Max",
                Marque = "Nike",
                Type = "TypeInexistant",
                StockMini = 10,
                StockMaxi = 100
            };

            Marque marque = new() { IdMarque = 1, Nom = "Nike" };

            _marqueRepository
                .Setup(repo => repo.GetByNameAsync("Nike"))
                .ReturnsAsync(marque);

            _typeProduitRepository
                .Setup(repo => repo.GetByNameAsync("TypeInexistant"))
                .ReturnsAsync((TypeProduit)null);

            // When: On essaie d'ajouter le produit
            ActionResult<ProduitDetailDTO> action = _controller.Add(produitDto).GetAwaiter().GetResult();

            // Then: Un code 400 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));

            _marqueRepository.Verify(repo => repo.GetByNameAsync("Nike"), Times.Once);
            _typeProduitRepository.Verify(repo => repo.GetByNameAsync("TypeInexistant"), Times.Once);
            _produitRepository.Verify(repo => repo.AddAsync(It.IsAny<Produit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldUpdateProduit()
        {
            // Given: Un produit à mettre à jour
            Produit produitToEdit = new()
            {
                IdProduit = 1,
                Nom = "Air Max Original",
                IdMarque = 1,
                IdTypeProduit = 1,
                Stock = 50
            };

            ProduitDetailDTO updatedDto = new()
            {
                IdProduit = 1,
                Nom = "Air Max Modifié",
                Marque = "Nike",
                Type = "Chaussure",
                Stock = 75
            };

            Marque marque = new() { IdMarque = 1, Nom = "Nike" };
            TypeProduit typeProduit = new() { IdTypeProduit = 1, Nom = "Chaussure" };

            _produitRepository
                .Setup(repo => repo.GetByIdAsync(produitToEdit.IdProduit))
                .ReturnsAsync(produitToEdit);

            _marqueRepository
                .Setup(repo => repo.GetByNameAsync("Nike"))
                .ReturnsAsync(marque);

            _typeProduitRepository
                .Setup(repo => repo.GetByNameAsync("Chaussure"))
                .ReturnsAsync(typeProduit);

            _produitRepository
                .Setup(repo => repo.UpdateAsync(produitToEdit));

            // When: On met à jour le produit
            IActionResult action = _controller.Update(produitToEdit.IdProduit, updatedDto).GetAwaiter().GetResult();

            // Then: Le produit est mis à jour avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(produitToEdit.IdProduit), Times.Once);
            _produitRepository.Verify(repo => repo.UpdateAsync(produitToEdit), Times.Once);
        }

        [TestMethod]
        public void UpdateProduitShouldReturnNotFoundWhenProduitDoesNotExist()
        {
            // Given: Un produit qui n'existe pas
            ProduitDetailDTO produitDto = new()
            {
                IdProduit = 999,
                Nom = "Produit Inexistant"
            };

            _produitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Produit)null);

            // When: On essaie de mettre à jour le produit
            IActionResult action = _controller.Update(999, produitDto).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _produitRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Produit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDeleteProduit()
        {
            // Given: Un produit enregistré
            Produit produitInDb = new()
            {
                IdProduit = 1,
                Nom = "Air Max"
            };

            _produitRepository
                .Setup(repo => repo.GetByIdAsync(produitInDb.IdProduit))
                .ReturnsAsync(produitInDb);

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Image>());

            _produitRepository
                .Setup(repo => repo.DeleteAsync(produitInDb));

            // When: On supprime le produit
            IActionResult action = _controller.Delete(produitInDb.IdProduit).GetAwaiter().GetResult();

            // Then: Le produit est supprimé avec un code 204
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(produitInDb.IdProduit), Times.Once);
            _produitRepository.Verify(repo => repo.DeleteAsync(produitInDb), Times.Once);
        }

        [TestMethod]
        public void ShouldDeleteProduitWithImages()
        {
            // Given: Un produit avec des images
            Produit produitInDb = new()
            {
                IdProduit = 1,
                Nom = "Air Max"
            };

            List<Image> images = new()
            {
                new Image { IdImage = 1, Nom = "Image 1", IdProduit = 1 },
                new Image { IdImage = 2, Nom = "Image 2", IdProduit = 1 }
            };

            _produitRepository
                .Setup(repo => repo.GetByIdAsync(produitInDb.IdProduit))
                .ReturnsAsync(produitInDb);

            _imageRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(images);

            _imageRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Image>()));

            _produitRepository
                .Setup(repo => repo.DeleteAsync(produitInDb));

            // When: On supprime le produit
            IActionResult action = _controller.Delete(produitInDb.IdProduit).GetAwaiter().GetResult();

            // Then: Le produit et ses images sont supprimés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NoContentResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(produitInDb.IdProduit), Times.Once);
            _imageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Image>()), Times.Exactly(2));
            _produitRepository.Verify(repo => repo.DeleteAsync(produitInDb), Times.Once);
        }

        [TestMethod]
        public void DeleteProduitShouldReturnNotFoundWhenProduitDoesNotExist()
        {
            // Given: Aucun produit en base
            _produitRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Produit)null);

            // When: On essaie de supprimer un produit qui n'existe pas
            IActionResult action = _controller.Delete(999).GetAwaiter().GetResult();

            // Then: Un code 404 est retourné
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));

            _produitRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _produitRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Produit>()), Times.Never);
        }

        [TestMethod]
        public void ShouldGetFilteredProduitsByMarque()
        {
            // Given: Plusieurs produits avec différentes marques
            IEnumerable<Produit> produitsInDb = new List<Produit>
            {
                new Produit
                {
                    IdProduit = 1,
                    Nom = "Air Max",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 50,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                },
                new Produit
                {
                    IdProduit = 2,
                    Nom = "Stan Smith",
                    IdMarque = 2,
                    IdTypeProduit = 1,
                    Stock = 30,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 2, Nom = "Adidas" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produitsInDb);

            // When: On filtre par marque Nike
            var action = _controller.GetFiltered(null, "Nike", null).GetAwaiter().GetResult();

            // Then: Seuls les produits Nike sont retournés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(1, produitDtos.Count());
            Assert.IsTrue(produitDtos.All(p => p.Marque == "Nike"));

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetFilteredProduitsByType()
        {
            // Given: Plusieurs produits avec différents types
            IEnumerable<Produit> produitsInDb = new List<Produit>
            {
                new Produit
                {
                    IdProduit = 1,
                    Nom = "Air Max",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 50,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                },
                new Produit
                {
                    IdProduit = 2,
                    Nom = "T-Shirt Nike",
                    IdMarque = 1,
                    IdTypeProduit = 2,
                    Stock = 100,
                    StockMini = 20,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 2, Nom = "Vêtement" }
                }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produitsInDb);

            // When: On filtre par type Chaussure
            var action = _controller.GetFiltered(null, null, "Chaussure").GetAwaiter().GetResult();

            // Then: Seuls les produits de type Chaussure sont retournés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(1, produitDtos.Count());
            Assert.IsTrue(produitDtos.All(p => p.Type == "Chaussure"));

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldGetFilteredProduitsBySearchTerm()
        {
            // Given: Plusieurs produits avec différentes descriptions
            IEnumerable<Produit> produitsInDb = new List<Produit>
            {
                new Produit
                {
                    IdProduit = 1,
                    Nom = "Air Max",
                    Description = "Confortable",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 50,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                },
                new Produit
                {
                    IdProduit = 2,
                    Nom = "Jordan",
                    Description = "Basket",
                    IdMarque = 1,
                    IdTypeProduit = 1,
                    Stock = 30,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { IdMarque = 1, Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { IdTypeProduit = 1, Nom = "Chaussure" }
                }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(produitsInDb);

            // When: On recherche par terme "confortable"
            var action = _controller.GetFiltered("confortable", null, null).GetAwaiter().GetResult();

            // Then: Seuls les produits contenant "confortable" sont retournés
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(1, produitDtos.Count());

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public void GetFilteredShouldReturnEmptyListWhenNoProducts()
        {
            // Given: Aucun produit en base
            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((IEnumerable<Produit>)null);


            // When: On recherche des produits
            var action = _controller.GetFiltered(null, null, null).GetAwaiter().GetResult();

            // Then: Une liste vide est retournée
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
            var okResult = action.Result as OkObjectResult;
            var produitDtos = okResult.Value as IEnumerable<ProduitDTO>;
            Assert.IsNotNull(produitDtos);
            Assert.AreEqual(0, produitDtos.Count());

            _produitRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}