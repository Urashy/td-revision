using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IProduitService
    {
        Task<IEnumerable<ProduitSimple>> GetAllSimpleAsync();
        Task<Produit> GetByIdDetailAsync(int id);
        Task<Produit> GetByNameDetailAsync(string name);
        Task AddAsync(Produit entity);
        Task UpdateAsync(int id, Produit entity);
        Task DeleteAsync(int id);
    }
}
