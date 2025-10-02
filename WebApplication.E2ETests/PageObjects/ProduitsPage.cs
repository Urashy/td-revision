namespace WebApplication.E2ETests.PageObjects;

public class ProduitsPage : BasePage
{
    // Sélecteurs
    private const string SearchInput = "#searchNom";
    private const string TypeFilter = "#filterType";
    private const string MarqueFilter = "#filterMarque";
    private const string ClearFiltersButton = "button:has-text('Effacer les filtres')";
    private const string AddProductButton = "a:has-text('Ajouter un produit')";
    private const string ProductCard = ".product-card";
    private const string ResultBadge = ".badge.bg-info.fs-6";

    public ProduitsPage(IPage page, string? baseUrl = null)
    : base(page, baseUrl)
    {
    }

    public async Task GoToProduitsPage()
    {
        await NavigateTo("/produits");
        await WaitForPageLoad();
    }

    #region Filtres

    public async Task SearchByName(string searchTerm)
    {
        await Page.FillAsync(SearchInput, searchTerm);
        await Page.WaitForTimeoutAsync(500); // Attendre le debounce
    }

    public async Task SelectType(string type)
    {
        await Page.SelectOptionAsync(TypeFilter, new[] { type });
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task SelectMarque(string marque)
    {
        await Page.SelectOptionAsync(MarqueFilter, new[] { marque });
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task ClearAllFilters()
    {
        await Page.ClickAsync(ClearFiltersButton);
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task<int> GetResultCount()
    {
        var badgeText = await Page.TextContentAsync(ResultBadge);
        var match = Regex.Match(badgeText ?? "", @"(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }

    public async Task<int> GetDisplayedProductsCount()
    {
        return await Page.Locator(ProductCard).CountAsync();
    }

    #endregion

    #region Actions sur les produits


    public async Task ClickProductCard(string productName)
    {
        await Page.Locator($".product-card:has-text('{productName}')").First.ClickAsync();
        await Page.WaitForTimeoutAsync(500);
    }

    public async Task ClickDetailsButton(string productName)
    {
        var card = Page.Locator($".product-card:has-text('{productName}')").First;
        await card.Locator("button:has-text('Détails')").ClickAsync();
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task ClickEditButton(string productName)
    {
        var card = Page.Locator($".product-card:has-text('{productName}')").First;
        await card.Locator("button.btn-outline-warning").ClickAsync();
        await Page.WaitForTimeoutAsync(500);
    }

    public async Task ClickDeleteButton(string productName)
    {
        var card = Page.Locator($".product-card:has-text('{productName}')").First;
        await card.Locator("button.btn-outline-danger").ClickAsync();
        await Page.WaitForTimeoutAsync(300);
    }



    public async Task<bool> IsProductDisplayed(string productName)
    {
        return await Page.Locator($".product-card:has-text('{productName}')").IsVisibleAsync();
    }

    public async Task<List<string>> GetDisplayedProductNames()
    {
        var names = new List<string>();
        var cards = await Page.Locator(ProductCard).AllAsync();
        
        foreach (var card in cards)
        {
            var name = await card.Locator(".card-title").TextContentAsync();
            if (!string.IsNullOrEmpty(name))
                names.Add(name);
        }
        
        return names;
    }

    #endregion

    #region Modal Détails

    public async Task<bool> IsDetailModalOpen()
    {
        return await Page.Locator(".modal.show:has-text('Détails du produit')").IsVisibleAsync();
    }

    public async Task CloseDetailModal()
    {
        await Page.ClickAsync(".modal.show .btn-close");
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task<string?> GetDetailModalProductName()
    {
        var modal = Page.Locator(".modal.show:has-text('Détails du produit')");
        return await modal.Locator("td:has-text('Nom :') + td").TextContentAsync();
    }

    public async Task<bool> IsProductImageDisplayed()
    {
        var modal = Page.Locator(".modal.show");
        var image = modal.Locator("img[src]").First;
        return await image.IsVisibleAsync();
    }

    #endregion

    #region Modal Édition

    public async Task<bool> IsEditModalOpen()
    {
        return await Page.Locator(".modal.show:has-text('Modifier le produit')").IsVisibleAsync();
    }

    public async Task FillEditForm(string? nom = null, string? marque = null, string? type = null, int? stock = null)
    {
        var modal = Page.Locator(".modal.show:has-text('Modifier le produit')");

        if (nom != null)
            await modal.Locator("#editNom").FillAsync(nom);

        if (marque != null)
            await modal.Locator("#editMarque").SelectOptionAsync(new[] { marque });

        if (type != null)
            await modal.Locator("#editType").SelectOptionAsync(new[] { type });

        if (stock.HasValue)
            await modal.Locator("#editStock").FillAsync(stock.Value.ToString());
    }

    public async Task AddImageInEditModal(string nomImage, string urlImage)
    {
        var modal = Page.Locator(".modal.show:has-text('Modifier le produit')");
        
        await modal.Locator("#editNomImage").FillAsync(nomImage);
        await modal.Locator("#editUrlImage").FillAsync(urlImage);
        await modal.Locator("button:has-text('Ajouter l\\'image')").ClickAsync();
        await Page.WaitForTimeoutAsync(300);
    }

    public async Task<int> GetImagesCountInEditModal()
    {
        var modal = Page.Locator(".modal.show:has-text('Modifier le produit')");
        return await modal.Locator(".card .row.g-0").CountAsync();
    }

    public async Task SubmitEditForm()
    {
        var modal = Page.Locator(".modal.show:has-text('Modifier le produit')");
        await modal.Locator("button[type='submit']:has-text('Sauvegarder')").ClickAsync();
    }

    public async Task CancelEdit()
    {
        var modal = Page.Locator(".modal.show:has-text('Modifier le produit')");
        await modal.Locator("button:has-text('Annuler')").ClickAsync();
    }

    #endregion

    #region Modal Suppression

    public async Task<bool> IsDeleteModalOpen()
    {
        return await Page.Locator(".modal.show:has-text('Confirmer la suppression')").IsVisibleAsync();
    }

    public async Task ConfirmDelete()
    {
        var modal = Page.Locator(".modal.show:has-text('Confirmer la suppression')");
        await modal.Locator("button:has-text('Supprimer définitivement')").ClickAsync();
    }

    public async Task CancelDelete()
    {
        var modal = Page.Locator(".modal.show:has-text('Confirmer la suppression')");
        await modal.Locator("button:has-text('Annuler')").ClickAsync();
    }

    #endregion

    #region Notifications

    public async Task<bool> IsSuccessNotificationDisplayed()
    {
        return await IsElementVisible(".toast.show.bg-success");
    }

    public async Task<bool> IsErrorNotificationDisplayed()
    {
        return await IsElementVisible(".toast.show.bg-danger");
    }

    public async Task<string?> GetNotificationMessage()
    {
        return await GetToastMessage();
    }

    #endregion
}