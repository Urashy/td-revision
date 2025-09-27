using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class TypeProduitManager : ManagerGenerique<TypeProduit>
    {
        public TypeProduitManager(ProduitsbdContext context) : base(context)
        {
        }

        public override async Task<ActionResult<IEnumerable<TypeProduit>>> GetAllAsync()
        {
            return await dbSet
                .Include(m => m.Produits)
                .ToListAsync();
        }

        public override async Task<ActionResult<TypeProduit?>> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.IdTypeProduit == id);
        }

        public override async Task<ActionResult<TypeProduit?>> GetByStringAsync(string str)
        {
            return await dbSet
                .Include(m => m.Produits)
                .FirstOrDefaultAsync(m => m.Nom == str);
        }
    }
}
