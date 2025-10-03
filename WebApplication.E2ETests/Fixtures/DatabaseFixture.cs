using System.Net.Http.Json;
using WebApplication.Models;

namespace WebApplication.E2ETests.Fixtures;

/// <summary>
/// Fixture pour gérer la base de données de test
/// </summary>
public class DatabaseFixture : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "http://localhost:5049";

    public DatabaseFixture()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
    }

    /// <summary>
    /// Nettoie complètement la base de données
    /// </summary>
    public async Task CleanDatabase()
    {
        try
        {
            // Ordre de suppression important à cause des contraintes FK
            await DeleteAllImages();
            await DeleteAllProduits();
            await DeleteAllMarques();
            await DeleteAllTypes();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erreur lors du nettoyage de la BDD: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialise la base avec des données de test minimales
    /// </summary>
    public async Task SeedTestData()
    {
        try
        {
            // Vérifier si des données existent déjà
            var produits = await _httpClient.GetFromJsonAsync<List<Produit>>("api/Produit/GetAll");
            
            if (produits?.Count > 0)
            {
                Console.WriteLine($"ℹ️ {produits.Count} produit(s) déjà présent(s)");
                return;
            }

            // Créer les données de base
            await _httpClient.PostAsJsonAsync("api/Marque/Add", new { Nom = TestData.Products.ExistingProductMarque1 });
            await _httpClient.PostAsJsonAsync("api/TypeProduit/Add", new { Nom = TestData.Products.ExistingProductType1 });
            
            await _httpClient.PostAsJsonAsync("api/Produit/Add", new
            {
                Nom = TestData.Products.ExistingProductName1,
                Marque = TestData.Products.ExistingProductMarque1,
                Type = TestData.Products.ExistingProductType1,
                Stock = 10,
                StockMini = 5,
                StockMaxi = 50
            });

            Console.WriteLine("✅ Données de test initialisées");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erreur lors du seed: {ex.Message}");
        }
    }

    private async Task DeleteAllImages()
    {
        var images = await _httpClient.GetFromJsonAsync<List<dynamic>>("api/Image/GetAll");
        if (images != null)
        {
            foreach (var image in images)
            {
                await _httpClient.DeleteAsync($"api/Image/Delete/{image.GetProperty("idImage").GetInt32()}");
            }
        }
    }

    private async Task DeleteAllProduits()
    {
        var produits = await _httpClient.GetFromJsonAsync<List<Produit>>("api/Produit/GetAll");
        if (produits != null)
        {
            foreach (var produit in produits)
            {
                await _httpClient.DeleteAsync($"api/Produit/Delete/{produit.IdProduit}");
            }
        }
    }

    private async Task DeleteAllMarques()
    {
        var marques = await _httpClient.GetFromJsonAsync<List<dynamic>>("api/Marque/GetAll");
        if (marques != null)
        {
            foreach (var marque in marques)
            {
                await _httpClient.DeleteAsync($"api/Marque/Delete/{marque.GetProperty("idMarque").GetInt32()}");
            }
        }
    }

    private async Task DeleteAllTypes()
    {
        var types = await _httpClient.GetFromJsonAsync<List<dynamic>>("api/TypeProduit/GetAll");
        if (types != null)
        {
            foreach (var type in types)
            {
                await _httpClient.DeleteAsync($"api/TypeProduit/Delete/{type.GetProperty("idTypeProduit").GetInt32()}");
            }
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}