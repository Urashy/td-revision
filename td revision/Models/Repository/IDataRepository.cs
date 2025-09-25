using Microsoft.AspNetCore.Mvc;

namespace td_revision.Models.Repository
{
    public interface IDataRepository<Tentity>
    {
        Task<ActionResult<IEnumerable<Tentity>>> GetAllAsync();
        Task<ActionResult<Tentity?>> GetByIdAsync(int id);
        Task<ActionResult<Tentity?>> GetByStringAsync(string str);
        Task AddAsync(Tentity entity);
        Task UpdateAsync(Tentity entityToUpdate,Tentity entity);
        Task DeleteAsync(Tentity entity);


    }
}
