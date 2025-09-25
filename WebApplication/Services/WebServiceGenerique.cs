using System.Net.Http;
using System.Net.Http.Json;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class WebServiceGenerique<T> : IGenericService<T> where T : class, IEntity
    {
        private readonly HttpClient _httpClient;
        private readonly string _controllerName;

        // Constructeur pour l'injection de dépendances
        public WebServiceGenerique(HttpClient httpClient)
        {
            _controllerName = typeof(T).Name;

            // Créer un nouveau HttpClient avec la bonne adresse de base
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"http://localhost:5049/api/{_controllerName}/")
            };
        }

        // Constructeur sans paramètres (pour compatibilité)
        public WebServiceGenerique()
        {
            _controllerName = typeof(T).Name;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://localhost:5049/api/{_controllerName}/")
            };
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<T>>("GetAll");
                return response ?? new List<T>();
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
                return await _httpClient.GetFromJsonAsync<T>($"GetById/{id}");
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
                var response = await _httpClient.GetAsync($"GetByName?name={Uri.EscapeDataString(name)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
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
                var response = await _httpClient.PostAsJsonAsync("Add", entity);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.PutAsJsonAsync($"Update/{entity.Id}", entity);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"Delete/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur DeleteAsync pour {_controllerName}: {ex.Message}");
                throw;
            }
        }

        // Libérer les ressources
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}