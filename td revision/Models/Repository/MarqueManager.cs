using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class MarqueManager : ManagerGenerique<Marque>
    {
        private readonly ProduitsbdContext _context;

        public MarqueManager(ProduitsbdContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResult<IEnumerable<Marque>>> GetAllAsync()
        {
            return await dbSet
                .Include(m => m.Produits)
                .ToListAsync();
        }

        public override async Task<ActionResult<Marque?>> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.IdMarque == id);
        }

        public override async Task<ActionResult<Marque?>> GetByStringAsync(string str)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.Nom == str);
        }
    }
}