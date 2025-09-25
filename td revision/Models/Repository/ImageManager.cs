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

        public override async Task<ActionResult<Image?>> GetByStringAsync(string str)
        {
            return await GetByStringPropertyAsync(i => i.NomImage, str);
        }

        public async Task<ActionResult<IEnumerable<Image>>> GetByProduitIdAsync(int produitId)
        {
            var images = await dbSet
                .Where(i => i.IdProduit == produitId)
                .ToListAsync();

            return images;
        }
    }
}
