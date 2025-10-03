using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApplication;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5049/") });

// Services génériques
builder.Services.AddScoped<IGenericService<Produit>, WebServiceGenerique<Produit>>();
builder.Services.AddScoped<IGenericService<Marque>, WebServiceGenerique<Marque>>();
builder.Services.AddScoped<IGenericService<TypeProduit>, WebServiceGenerique<TypeProduit>>();
builder.Services.AddScoped<IGenericService<Image>, WebServiceGenerique<Image>>();



// Service de produits spécialisé
builder.Services.AddScoped<IGenericService<Produit>, ProduitService>();


// Service de notification (SINGLETON pour persister les notifications dans toute l'application)
builder.Services.AddSingleton<INotificationService, NotificationService>();


builder.Services.AddScoped<AjouterProduitViewModel>();
builder.Services.AddScoped<ProduitPageViewModel>();

await builder.Build().RunAsync();