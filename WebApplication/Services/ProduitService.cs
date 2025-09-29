using System.Net.Http.Json;
using System.Text.Json;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ProduitService : IGenericService<Produit>
    {
        private readonly HttpClient _httpClient;
        private const string ControllerName = "Produit";

        public ProduitService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5049/");
            }
        }

        #region IGenericService<Produit> Implementation

        public async Task<IEnumerable<Produit>> GetAllAsync()
        {
            try
            {
                var url = $"api/{ControllerName}/GetAll";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {content}");

                    // Désérialiser en List<ProduitSimple> puis convertir en IEnumerable<Produit>
                    var produitsSimples = await response.Content.ReadFromJsonAsync<List<ProduitSimple>>();

                    // Conversion ProduitSimple -> Produit
                    var produits = produitsSimples?.Select(ps => new Produit
                    {
                        IdProduit = ps.IdProduit,
                        Nom = ps.Nom,
                        Type = ps.Type,
                        Marque = ps.Marque
                    }) ?? Enumerable.Empty<Produit>();

                    return produits;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetAllAsync : {ex.Message}");
                throw;
            }
        }

        public async Task<Produit> GetByIdAsync(int id)
        {
            try
            {
                var url = $"api/{ControllerName}/GetById/{id}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var produit = await response.Content.ReadFromJsonAsync<Produit>();
                    return produit ?? throw new InvalidOperationException($"Produit avec ID {id} introuvable");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Produit avec ID {id} introuvable");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetByIdAsync : {ex.Message}");
                throw;
            }
        }

        public async Task<Produit> GetByNameAsync(string name)
        {
            try
            {
                var url = $"api/{ControllerName}/GetByName?name={Uri.EscapeDataString(name)}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var produit = await response.Content.ReadFromJsonAsync<Produit>();
                    return produit ?? throw new InvalidOperationException($"Produit avec le nom '{name}' introuvable");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Produit avec le nom '{name}' introuvable");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetByNameAsync : {ex.Message}");
                throw;
            }
        }

        public async Task AddAsync(Produit entity)
        {
            try
            {
                var url = $"api/{ControllerName}/Add";
                Console.WriteLine($"POST Request URL: {_httpClient.BaseAddress}{url}");

                var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"Payload: {json}");

                var response = await _httpClient.PostAsJsonAsync(url, entity);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }

                Console.WriteLine($"Ajout réussi pour le produit");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur AddAsync : {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(int id, Produit entity)
        {
            try
            {
                var url = $"api/{ControllerName}/Update/{id}";
                Console.WriteLine($"PUT Request URL: {_httpClient.BaseAddress}{url}");

                var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"Payload: {json}");

                var response = await _httpClient.PutAsJsonAsync(url, entity);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }

                Console.WriteLine($"Mise à jour réussie pour le produit avec ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur UpdateAsync : {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var url = $"api/{ControllerName}/Delete/{id}";
                Console.WriteLine($"DELETE Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }

                Console.WriteLine($"Suppression réussie pour le produit avec ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur DeleteAsync : {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Méthodes d'extension spécifiques (non dans l'interface)

        /// <summary>
        /// Récupère tous les produits en version simplifiée
        /// </summary>
        public async Task<IEnumerable<ProduitSimple>> GetAllSimpleAsync()
        {
            try
            {
                var url = $"api/{ControllerName}/GetAll";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var produitsDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProduitSimple>>();
                    return produitsDtos ?? Enumerable.Empty<ProduitSimple>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetAllSimpleAsync : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Récupère les produits filtrés via l'API
        /// </summary>
        public async Task<IEnumerable<ProduitSimple>> GetFilteredAsync(string? searchTerm = null, string? marque = null, string? type = null)
        {
            try
            {
                // Construction de l'URL avec les paramètres de query
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                if (!string.IsNullOrEmpty(marque))
                    queryParams.Add($"marque={Uri.EscapeDataString(marque)}");

                if (!string.IsNullOrEmpty(type))
                    queryParams.Add($"type={Uri.EscapeDataString(type)}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var url = $"api/{ControllerName}/GetFiltered{queryString}";

                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var produits = await response.Content.ReadFromJsonAsync<IEnumerable<ProduitSimple>>();
                    return produits ?? Enumerable.Empty<ProduitSimple>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetFilteredAsync : {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}