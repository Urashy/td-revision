using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class WebServiceGenerique<T> : IGenericService<T> where T : class, IEntity
    {
        private readonly HttpClient _httpClient;
        private readonly string _controllerName;

        public WebServiceGenerique(HttpClient httpClient)
        {
            _controllerName = typeof(T).Name;

            // Utiliser l'HttpClient injecté et définir la base address
            _httpClient = httpClient;

            // S'assurer que l'URL de base est correcte
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5049/");
            }

            Console.WriteLine($"WebServiceGenerique pour {_controllerName} initialisé avec base URL: {_httpClient.BaseAddress}");
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var url = $"api/{_controllerName}/GetAll";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {content}");

                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<T>>();
                    return result ?? new List<T>();
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
                Console.WriteLine($"Erreur GetAllAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var url = $"api/{_controllerName}/GetById/{id}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetByIdAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        public async Task<T> GetByNameAsync(string name)
        {
            try
            {
                var url = $"api/{_controllerName}/GetByName?name={Uri.EscapeDataString(name)}";
                Console.WriteLine($"GET Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
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
                Console.WriteLine($"Erreur GetByNameAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                var url = $"api/{_controllerName}/Add";
                Console.WriteLine($"POST Request URL: {_httpClient.BaseAddress}{url}");

                // Sérialiser l'objet pour debug
                var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"Payload: {json}");

                var response = await _httpClient.PostAsJsonAsync(url, entity);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }

                Console.WriteLine($"Ajout réussi pour {_controllerName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur AddAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                var url = $"api/{_controllerName}/Update/{entity.Id}";
                Console.WriteLine($"PUT Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.PutAsJsonAsync(url, entity);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur UpdateAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var url = $"api/{_controllerName}/Delete/{id}";
                Console.WriteLine($"DELETE Request URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erreur {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur DeleteAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }
    }
}