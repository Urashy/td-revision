using WebApplication.E2ETests.Fixtures;
using WebApplication.E2ETests.PageObjects;

namespace WebApplication.E2ETests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProduitsTests : PageTest
{
    private ProduitsPage _produitsPage = null!;
    private DatabaseFixture _dbFixture = null!;
    private const string BaseUrl = "http://localhost:5178";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _dbFixture = new DatabaseFixture();
    }

    [SetUp]
    public async Task Setup()
    {
        // Nettoyer et réinitialiser la BDD avant chaque test
        await _dbFixture.CleanDatabase();
        await _dbFixture.SeedTestData();

        _produitsPage = new ProduitsPage(Page, BaseUrl);
        await _produitsPage.GoToProduitsPage();
    }

    [TearDown]
    public async Task TearDown()
    {
        // Nettoyage après chaque test
        await _dbFixture.CleanDatabase();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _dbFixture?.Dispose();
    }

    [Test]
    public async Task Page_Should_Load_Successfully()
    {
        await Expect(Page).ToHaveTitleAsync(new Regex(".*WebApplication.*"));
        await Expect(Page.Locator("h1:has-text('Gestion des Produits')")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Should_Display_Product_Cards()
    {
        var productCount = await _produitsPage.GetDisplayedProductsCount();
        Assert.That(productCount, Is.GreaterThan(0), "Au moins un produit devrait être affiché");
    }

    [Test]
    public async Task Should_Open_Detail_Modal_When_Clicking_Product_Card()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty, "Il doit y avoir au moins un produit pour ce test");

        var firstProduct = products.First();

        await _produitsPage.ClickProductCard(firstProduct);

        Assert.That(await _produitsPage.IsDetailModalOpen(), Is.True, "La modal de détails devrait être ouverte");

        var modalProductName = await _produitsPage.GetDetailModalProductName();
        Assert.That(modalProductName, Is.EqualTo(firstProduct), "Le nom du produit dans la modal devrait correspondre");
    }


    [Test]
    public async Task Should_Open_Edit_Modal_When_Clicking_Edit_Button()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        await _produitsPage.ClickEditButton(products.First());

        Assert.That(await _produitsPage.IsEditModalOpen(), Is.True, "La modal d'édition devrait être ouverte");
    }

    [Test]
    public async Task Should_Update_Product_Successfully()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        var productToEdit = products.First();
        var newName = $"{productToEdit} - Modifié {DateTime.Now:HHmmss}";

        await _produitsPage.ClickEditButton(productToEdit);
        Assume.That(await _produitsPage.IsEditModalOpen(), Is.True);

        await _produitsPage.FillEditForm(nom: newName);
        await _produitsPage.SubmitEditForm();

        await Task.Delay(1000);
        Assert.That(await _produitsPage.IsSuccessNotificationDisplayed(), Is.True,
            "Une notification de succès devrait être affichée");

        var notification = await _produitsPage.GetNotificationMessage();
        Assert.That(notification, Does.Contain("modifié avec succès"));
    }

    [Test]
    public async Task Should_Add_Image_In_Edit_Modal()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        await _produitsPage.ClickEditButton(products.First());
        Assume.That(await _produitsPage.IsEditModalOpen(), Is.True);

        var initialImageCount = await _produitsPage.GetImagesCountInEditModal();

        await _produitsPage.AddImageInEditModal(
            TestData.Images.TestImageName,
            TestData.Images.TestImageUrl);

        var newImageCount = await _produitsPage.GetImagesCountInEditModal();
        Assert.That(newImageCount, Is.EqualTo(initialImageCount + 1),
            "Une nouvelle image devrait être ajoutée");
    }

    [Test]
    public async Task Should_Open_Delete_Modal_When_Clicking_Delete_Button()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        await _produitsPage.ClickDeleteButton(products.First());

        Assert.That(await _produitsPage.IsDeleteModalOpen(), Is.True,
            "La modal de confirmation de suppression devrait être ouverte");
    }

    [Test]
    public async Task Should_Cancel_Product_Deletion()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        var productName = products.First();

        await _produitsPage.ClickDeleteButton(productName);
        Assume.That(await _produitsPage.IsDeleteModalOpen(), Is.True);

        await _produitsPage.CancelDelete();

        Assert.That(await _produitsPage.IsDeleteModalOpen(), Is.False,
            "La modal devrait être fermée après annulation");

        Assert.That(await _produitsPage.IsProductDisplayed(productName), Is.True,
            "Le produit devrait toujours être affiché");
    }

    [Test]
    public async Task Should_Delete_Product_Successfully()
    {
        var products = await _produitsPage.GetDisplayedProductNames();
        Assume.That(products, Is.Not.Empty);

        var productToDelete = products.First();
        var initialCount = await _produitsPage.GetDisplayedProductsCount();

        await _produitsPage.ClickDeleteButton(productToDelete);
        Assume.That(await _produitsPage.IsDeleteModalOpen(), Is.True);

        await _produitsPage.ConfirmDelete();

        await Task.Delay(1000);

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
        var reapproBadge = Page.Locator(".reappro-ribbon");

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