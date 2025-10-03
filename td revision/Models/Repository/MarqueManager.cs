using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class MarqueManager : NamedManagerGenerique<Marque>
    {
        public MarqueManager(ProduitsbdContext context) : base(context)
        {
        }

        // Override pour inclure les relations
        public override async Task<IEnumerable<Marque>> GetAllAsync()
        {
            return await dbSet
                .Include(m => m.Produits)
                .ToListAsync();
        }

        public override async Task<Marque?> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.IdMarque == id);
        }

        public override async Task<Marque?> GetByNameAsync(string name)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.Nom == name);
        }
    }
}