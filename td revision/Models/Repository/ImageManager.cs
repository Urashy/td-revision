using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class ImageManager : ManagerGenerique<Image>
    {
        public ImageManager(ProduitsbdContext context) : base(context)
        {
        }

        // Méthode spécifique aux images
        public async Task<IEnumerable<Image>> GetByProduitIdAsync(int produitId)
        {
            return await dbSet
                .Where(i => i.IdProduit == produitId)
                .ToListAsync();
        }

    }
}
