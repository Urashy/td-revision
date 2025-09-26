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

        public override async Task<ActionResult<TypeProduit?>> GetByStringAsync(string str)
        {
            return await dbSet.FirstOrDefaultAsync(t => t.Nom== str);
        }
    }
}
