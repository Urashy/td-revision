using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class ImageManager : ManagerGenerique<Image>
    {
        private readonly ProduitsbdContext _context;
        public ImageManager(ProduitsbdContext context) : base(context)
        {
            _context = context;
        }


        public async Task<ActionResult<IEnumerable<Image>>> GetByProduitIdAsync(int produitId)
        {
            var images = await dbSet
                .Where(i => i.IdProduit == produitId)
                .ToListAsync();

            return images;
        }

        public override async Task<ActionResult<Image?>> GetByStringAsync(string str)
        {
            return await dbSet.FirstOrDefaultAsync(i => i.NomImage == str);
        }
    }
}
