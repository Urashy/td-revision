using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;
using td_revision.DTO;

namespace td_revision.Models.Repository
{
    public class ProduitManager : NamedManagerGenerique<Produit>
    {
        public ProduitManager(ProduitsbdContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Produit>> GetAllAsync()
        {
            return await dbSet
                .Include(p => p.MarqueProduitNavigation)
                .Include(p => p.TypeProduitNavigation)
                .ToListAsync();
        }

        public override async Task<Produit?> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(p => p.MarqueProduitNavigation)
                .Include(p => p.TypeProduitNavigation)
                .FirstOrDefaultAsync(p => p.IdProduit == id);
        }

        public override async Task<Produit?> GetByNameAsync(string name)
        {
            return await dbSet
                .Include(p => p.MarqueProduitNavigation)
                .Include(p => p.TypeProduitNavigation)
                .FirstOrDefaultAsync(p => p.Nom == name);
        }
    }
}
