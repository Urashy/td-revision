using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public class MarqueManager : ManagerGenerique<Marque>
    {
        public MarqueManager(ProduitsbdContext context) : base(context)
        {
        }

        public override async Task<ActionResult<Marque?>> GetByStringAsync(string str)
        {
            return await GetByStringPropertyAsync(m => m.Nom, str);
        }
    }
}
