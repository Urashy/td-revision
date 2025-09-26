using System.Net.Http.Json;
using System.Text.Json;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ProduitService : IProduitService
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

        public async Task<IEnumerable<ProduitSimple>> GetAllSimpleAsync()
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

                    // Le serveur retourne ProduitDTO, on le map vers ProduitSimple
                    var produitsDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProduitSimple>>();
                    return produitsDtos ?? new List<ProduitSimple>();
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

        public async Task<Produit> GetByIdDetailAsync(int id)
        {
            try
            {
                var url = $"api/{ControllerName}/GetById/{id}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Produit>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetByIdDetailAsync : {ex.Message}");
                throw;
            }
        }

        public async Task<Produit> GetByNameDetailAsync(string name)
        {
            try
            {
                var url = $"api/{ControllerName}/GetByName?name={Uri.EscapeDataString(name)}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Produit>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetByNameDetailAsync : {ex.Message}");
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
    }
}
