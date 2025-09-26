using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;
using td_revision.DTO;

namespace td_revision.Models.Repository
{
    public class ProduitManager : ManagerGenerique<Produit>
    {
        private readonly ProduitsbdContext _context;
        public ProduitManager(ProduitsbdContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResult<IEnumerable<Produit>>> GetAllAsync()
        {
            return await dbSet
                .Include(p => p.MarqueProduitNavigation)
                .Include(p => p.TypeProduitNavigation)
                .ToListAsync();
        }

        public override async Task<ActionResult<Produit?>> GetByStringAsync(string str)
        {
            return await dbSet.FirstOrDefaultAsync(p => p.Nom == str);
        }
    }
}
