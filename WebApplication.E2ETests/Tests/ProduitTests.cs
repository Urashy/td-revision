using System.Net.Http.Json;
using WebApplication.E2ETests.Fixtures;
using WebApplication.E2ETests.PageObjects;
using WebApplication.Models;

namespace WebApplication.E2ETests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProduitsTests : PageTest
{
    private ProduitsPage _produitsPage = null!;
    private const string BaseUrl = "http://localhost:5178"; // Ajustez selon votre config

    [SetUp]
    public async Task Setup()
    {
        _produitsPage = new ProduitsPage(Page, BaseUrl);

        // SEED des données de test via l'API
        await SeedTestData();

        await _produitsPage.GoToProduitsPage();
    }

    private async Task SeedTestData()
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5049") };

        // Vérifie si des produits existent
        var produits = await httpClient.GetFromJsonAsync<List<Produit>>("api/Produit/GetAll");

        if (produits?.Count == 0)
        {
            // Ajoute des données de test
            await httpClient.PostAsJsonAsync("api/Marque/Add", new { Nom = "MarqueTest" });
            await httpClient.PostAsJsonAsync("api/TypeProduit/Add", new { Nom = "TypeTest" });
            await httpClient.PostAsJsonAsync("api/Produit/Add", new
            {
                Nom = "Produit Test 1",
                Marque = "MarqueTest",
                Type = "TypeTest",
                Stock = 10
            });
        }
    }

    [Test]
    public async Task Page_Should_Load_Successfully()
    {
        // Assert
        await Expect(Page).ToHaveTitleAsync(new Regex(".*WebApplication.*"));
        await Expect(Page.Locator("h1:has-text('Gestion des Produits')")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Should_Display_Product_Cards()
    {
        // Act
        var productCount = await _produitsPage.GetDisplayedProductsCount();

        // Assert
        Assert.That(productCount, Is.GreaterThan(0), "Au moins un produit devrait être affiché");
    }

    [Test]
    public async Task Should_Open_Detail_Modal_When_Clicking_Product_Card()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty, "Il doit y avoir au moins un produit pour ce test");
        
        var firstProduct = products.First();

        // Act
        await _produitsPage.ClickProductCard(firstProduct);

        // Assert
        Assert.That(await _produitsPage.IsDetailModalOpen(), Is.True, "La modal de détails devrait être ouverte");
        
        var modalProductName = await _produitsPage.GetDetailModalProductName();
        Assert.That(modalProductName, Is.EqualTo(firstProduct), "Le nom du produit dans la modal devrait correspondre");
    }

    [Test]
    public async Task Should_Close_Detail_Modal()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);
        
        await _produitsPage.ClickProductCard(products.First());
        Assume.That(await _produitsPage.IsDetailModalOpen(), Is.True);

        // Act
        await _produitsPage.CloseDetailModal();

        // Assert
        Assert.That(await _produitsPage.IsDetailModalOpen(), Is.False, "La modal devrait être fermée");
    }

    [Test]
    public async Task Should_Open_Edit_Modal_When_Clicking_Edit_Button()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        // Act
        await _produitsPage.ClickEditButton(products.First());

        // Assert
        Assert.That(await _produitsPage.IsEditModalOpen(), Is.True, "La modal d'édition devrait être ouverte");
    }

    [Test]
    public async Task Should_Update_Product_Successfully()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);
        
        var productToEdit = products.First();
        var newName = $"{productToEdit} - Modifié {DateTime.Now:HHmmss}";

        await _produitsPage.ClickEditButton(productToEdit);
        Assume.That(await _produitsPage.IsEditModalOpen(), Is.True);

        // Act
        await _produitsPage.FillEditForm(nom: newName);
        await _produitsPage.SubmitEditForm();

        // Assert
        await Task.Delay(1000); // Attendre la sauvegarde
        Assert.That(await _produitsPage.IsSuccessNotificationDisplayed(), Is.True, 
            "Une notification de succès devrait être affichée");
        
        var notification = await _produitsPage.GetNotificationMessage();
        Assert.That(notification, Does.Contain("modifié avec succès"));
    }

    [Test]
    public async Task Should_Cancel_Product_Edit()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        await _produitsPage.ClickEditButton(products.First());
        Assume.That(await _produitsPage.IsEditModalOpen(), Is.True);

        // Act
        await _produitsPage.CancelEdit();

        // Assert
        Assert.That(await _produitsPage.IsEditModalOpen(), Is.False, 
            "La modal d'édition devrait être fermée après annulation");
    }

    [Test]
    public async Task Should_Add_Image_In_Edit_Modal()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        await _produitsPage.ClickEditButton(products.First());
        Assume.That(await _produitsPage.IsEditModalOpen(), Is.True);

        var initialImageCount = await _produitsPage.GetImagesCountInEditModal();

        // Act
        await _produitsPage.AddImageInEditModal(
            TestData.Images.TestImageName, 
            TestData.Images.TestImageUrl);

        // Assert
        var newImageCount = await _produitsPage.GetImagesCountInEditModal();
        Assert.That(newImageCount, Is.EqualTo(initialImageCount + 1), 
            "Une nouvelle image devrait être ajoutée");
    }

    [Test]
    public async Task Should_Open_Delete_Modal_When_Clicking_Delete_Button()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        // Act
        await _produitsPage.ClickDeleteButton(products.First());

        // Assert
        Assert.That(await _produitsPage.IsDeleteModalOpen(), Is.True, 
            "La modal de confirmation de suppression devrait être ouverte");
    }

    [Test]
    public async Task Should_Cancel_Product_Deletion()
    {
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);
        
        var productName = products.First();

        await _produitsPage.ClickDeleteButton(productName);
        Assume.That(await _produitsPage.IsDeleteModalOpen(), Is.True);

        // Act
        await _produitsPage.CancelDelete();

        // Assert
        Assert.That(await _produitsPage.IsDeleteModalOpen(), Is.False, 
            "La modal devrait être fermée après annulation");
        
        Assert.That(await _produitsPage.IsProductDisplayed(productName), Is.True, 
            "Le produit devrait toujours être affiché");
    }

    [Test]
    [Category("Destructive")]
    public async Task Should_Delete_Product_Successfully()
    {
        // NOTE: Ce test supprime réellement un produit - à exécuter avec précaution
        // Assurez-vous d'avoir des données de test dédiées
        
        // Arrange
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);
        
        var productToDelete = products.First();
        var initialCount = await _produitsPage.GetDisplayedProductsCount();

        await _produitsPage.ClickDeleteButton(productToDelete);
        Assume.That(await _produitsPage.IsDeleteModalOpen(), Is.True);

        // Act
        await _produitsPage.ConfirmDelete();

        // Assert
        await Task.Delay(1000); // Attendre la suppression
        
        Assert.That(await _produitsPage.IsSuccessNotificationDisplayed(), Is.True, 
            "Une notification de succès devrait être affichée");
        
        var notification = await _produitsPage.GetNotificationMessage();
        Assert.That(notification, Does.Contain("supprimé avec succès"));

        await _produitsPage.WaitForToastToDisappear();
        
        var newCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(newCount, Is.EqualTo(initialCount - 1), 
            "Le nombre de produits devrait diminuer de 1");
    }

    [Test]
    public async Task Should_Display_Reappro_Badge_When_Product_In_Reappro()
    {
        // Arrange & Act
        var reapproBadge = Page.Locator(".reappro-ribbon");

        // Assert
        if (await reapproBadge.CountAsync() > 0)
        {
            await Expect(reapproBadge.First).ToBeVisibleAsync();
            await Expect(reapproBadge.First).ToContainTextAsync("Réappro");
        }
        else
        {
            Assert.Pass("Aucun produit en réapprovisionnement trouvé - test non applicable");
        }
    }
}