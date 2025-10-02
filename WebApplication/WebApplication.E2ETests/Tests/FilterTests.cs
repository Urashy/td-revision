using WebApplication.E2ETests.PageObjects;
using WebApplication.E2ETests.Fixtures;

namespace WebApplication.E2ETests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class FiltersTests : PageTest
{
    private ProduitsPage _produitsPage = null!;
    private const string BaseUrl = "http://localhost:5000";

    [SetUp]
    public async Task Setup()
    {
        _produitsPage = new ProduitsPage(Page, BaseUrl);
        await _produitsPage.GoToProduitsPage();
    }

    [Test]
    public async Task Should_Filter_Products_By_Name()
    {
        // Arrange
        var initialCount = await _produitsPage.GetDisplayedProductsCount();
        Assume.That(initialCount, Is.GreaterThan(0));

        // Act
        await _produitsPage.SearchByName(TestData.Filters.SearchTerm);
        await Task.Delay(1000); // Attendre le filtrage

        // Assert
        var filteredCount = await _produitsPage.GetDisplayedProductsCount();
        var products = await _produitsPage.GetDisplayedProductNames();

        Assert.That(products, Is.All.Contains(TestData.Filters.SearchTerm)
            .Or.All.Contains(TestData.Filters.SearchTerm.ToLower()),
            "Tous les produits affichés devraient contenir le terme de recherche");

        var resultCount = await _produitsPage.GetResultCount();
        Assert.That(resultCount, Is.EqualTo(filteredCount),
            "Le compteur de résultats devrait correspondre au nombre de produits affichés");
    }

    [Test]
    public async Task Should_Filter_Products_By_Type()
    {
        // Arrange
        var initialCount = await _produitsPage.GetDisplayedProductsCount();
        Assume.That(initialCount, Is.GreaterThan(0));

        // Act
        await _produitsPage.SelectType(TestData.Filters.ValidType);
        await Task.Delay(500);

        // Assert
        var filteredCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(filteredCount, Is.LessThanOrEqualTo(initialCount),
            "Le nombre de produits filtrés devrait être inférieur ou égal au nombre initial");

        // Vérifier que tous les produits affichés ont le bon type
        var productCards = await Page.Locator(".product-card").AllAsync();
        foreach (var card in productCards)
        {
            var typeText = await card.Locator(".badge.bg-secondary").TextContentAsync();
            Assert.That(typeText, Does.Contain(TestData.Filters.ValidType));
        }
    }

    [Test]
    public async Task Should_Filter_Products_By_Marque()
    {
        // Arrange
        var initialCount = await _produitsPage.GetDisplayedProductsCount();
        Assume.That(initialCount, Is.GreaterThan(0));

        // Act
        await _produitsPage.SelectMarque(TestData.Filters.ValidMarque);
        await Task.Delay(500);

        // Assert
        var filteredCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(filteredCount, Is.LessThanOrEqualTo(initialCount));

        // Vérifier que tous les produits affichés ont la bonne marque
        var productCards = await Page.Locator(".product-card").AllAsync();
        foreach (var card in productCards)
        {
            var marqueText = await card.Locator(".badge.bg-info").TextContentAsync();
            Assert.That(marqueText, Does.Contain(TestData.Filters.ValidMarque));
        }
    }

    [Test]
    public async Task Should_Combine_Multiple_Filters()
    {
        // Arrange
        var initialCount = await _produitsPage.GetDisplayedProductsCount();
        Assume.That(initialCount, Is.GreaterThan(0));

        // Act
        await _produitsPage.SearchByName(TestData.Filters.SearchTerm);
        await Task.Delay(500);

        await _produitsPage.SelectType(TestData.Filters.ValidType);
        await Task.Delay(500);

        await _produitsPage.SelectMarque(TestData.Filters.ValidMarque);
        await Task.Delay(500);

        // Assert
        var filteredCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(filteredCount, Is.LessThanOrEqualTo(initialCount),
            "Les filtres combinés devraient réduire ou maintenir le nombre de produits");

        var resultCount = await _produitsPage.GetResultCount();
        Assert.That(resultCount, Is.EqualTo(filteredCount));
    }

    [Test]
    public async Task Should_Clear_All_Filters()
    {
        // Arrange
        var initialCount = await _produitsPage.GetDisplayedProductsCount();

        await _produitsPage.SearchByName(TestData.Filters.SearchTerm);
        await _produitsPage.SelectType(TestData.Filters.ValidType);
        await Task.Delay(500);

        var filteredCount = await _produitsPage.GetDisplayedProductsCount();
        Assume.That(filteredCount, Is.LessThan(initialCount));

        // Act
        await _produitsPage.ClearAllFilters();
        await Task.Delay(500);

        // Assert
        var clearedCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(clearedCount, Is.EqualTo(initialCount),
            "Tous les produits devraient être réaffichés après avoir effacé les filtres");
    }

    [Test]
    public async Task Should_Show_No_Results_Message_When_No_Match()
    {
        // Act
        await _produitsPage.SearchByName("ProduitQuiNExistePas12345XYZ");
        await Task.Delay(1000);

        // Assert
        var noResultsMessage = Page.Locator("p:has-text('Aucun produit trouvé avec ces critères')");
        await Expect(noResultsMessage).ToBeVisibleAsync();

        var productCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(productCount, Is.EqualTo(0), "Aucun produit ne devrait être affiché");

        var resultCount = await _produitsPage.GetResultCount();
        Assert.That(resultCount, Is.EqualTo(0), "Le compteur devrait afficher 0");
    }

}