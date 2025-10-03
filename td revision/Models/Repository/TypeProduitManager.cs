using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class TypeProduitManager : NamedManagerGenerique<TypeProduit>
    {
        public TypeProduitManager(ProduitsbdContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<TypeProduit>> GetAllAsync()
        {
            return await dbSet
                .Include(t => t.Produits)
                .ToListAsync();
        }

        public override async Task<TypeProduit?> GetByIdAsync(int id)
        {
            return await dbSet
                .Include(t => t.Produits)
                .FirstOrDefaultAsync(t => t.IdTypeProduit == id);
        }

        public override async Task<TypeProduit?> GetByNameAsync(string name)
        {
            return await dbSet
                .Include(t => t.Produits)
                .FirstOrDefaultAsync(t => t.Nom == name);
        }
    }
}
