using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using td_revision.Controllers;
using td_revision.DTO.Produit;
using td_revision.Mapper;
using td_revision.Models;
using td_revision.Models.Repository;

namespace td_revisionTests.Controllers
{
    [TestClass()]
    public class ProduitControllerMockTests
    {
        private ProduitController _controller;
        private Mock<IDataRepository<Produit>> _produitRepository;
        private Mock<IDataRepository<Marque>> _marqueRepository;
        private Mock<IDataRepository<TypeProduit>> _typeProduitRepository;
        private Mock<IDataRepository<Image>> _imageRepository;
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            _produitRepository = new Mock<IDataRepository<Produit>>();
            _marqueRepository = new Mock<IDataRepository<Marque>>();
            _typeProduitRepository = new Mock<IDataRepository<TypeProduit>>();
            _imageRepository = new Mock<IDataRepository<Image>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _controller = new ProduitController(_mapper, _produitRepository.Object, _marqueRepository.Object, _typeProduitRepository.Object, _imageRepository.Object);
        }

        [TestMethod]
        public void ShouldGetProduitById()
        {
            // Given: Un produit enregistré avec marque et type
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
                .ReturnsAsync(new ActionResult<Produit>((Produit)null));

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
                    Stock = 50,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { Nom = "Chaussure" }
                },
                new Produit
                {
                    IdProduit = 2,
                    Nom = "Jordan",
                    Stock = 30,
                    StockMini = 10,
                    MarqueProduitNavigation = new Marque { Nom = "Nike" },
                    TypeProduitNavigation = new TypeProduit { Nom = "Chaussure" }
                }
            };

            _produitRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produitsInDb));

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
                MarqueProduitNavigation = new Marque { Nom = "Nike" },
                TypeProduitNavigation = new TypeProduit { Nom = "Chaussure" }
            };

            _produitRepository
                .Setup(repo => repo.GetByStringAsync("Air Max"))
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

            _produitRepository.Verify(repo => repo.GetByStringAsync("Air Max"), Times.Once);
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
                .Setup(repo => repo.GetByStringAsync("Nike"))
                .ReturnsAsync(marque);

            _typeProduitRepository
                .Setup(repo => repo.GetByStringAsync("Chaussure"))
                .ReturnsAsync(typeProduit);

            _produitRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Produit>()));

            // When: On ajoute le produit
            ActionResult<ProduitDetailDTO> action = _controller.Add(produitDto).GetAwaiter().GetResult();

            // Then: Le produit est créé avec un code 201
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

            _marqueRepository.Verify(repo => repo.GetByStringAsync("Nike"), Times.Once);
            _typeProduitRepository.Verify(repo => repo.GetByStringAsync("Chaussure"), Times.Once);
            _produitRepository.Verify(repo => repo.AddAsync(It.IsAny<Produit>()), Times.Once);
        }

    }
}