namespace WebApplication.E2ETests.PageObjects;

public class BasePage
{
    protected readonly IPage Page;
    protected readonly string BaseUrl;

    public BasePage(IPage page, string baseUrl = "http://localhost:5000")
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    public async Task NavigateTo(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
    }

    public async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> IsElementVisible(string selector, int timeout = 5000)
    {
        try
        {
            await Page.WaitForSelectorAsync(selector, new() { Timeout = timeout });
            return await Page.IsVisibleAsync(selector);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetToastMessage()
    {
        var toast = Page.Locator(".toast.show .toast-body");
        await toast.WaitForAsync(new() { Timeout = 5000 });
        return await toast.TextContentAsync();
    }

    public async Task WaitForToastToDisappear()
    {
        await Page.WaitForSelectorAsync(".toast.show", new() { State = WaitForSelectorState.Hidden, Timeout = 10000 });
    }
}