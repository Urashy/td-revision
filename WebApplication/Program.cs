using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5049/")
            });

            // Service spécialisé pour les produits
            builder.Services.AddScoped<IProduitService, ProduitService>();

            // Services génériques pour les autres entités
            builder.Services.AddScoped<IGenericService<Marque>, WebServiceGenerique<Marque>>();
            builder.Services.AddScoped<IGenericService<TypeProduit>, WebServiceGenerique<TypeProduit>>();
            builder.Services.AddScoped<IGenericService<Image>, WebServiceGenerique<Image>>();

            await builder.Build().RunAsync();
        }
    }
}