using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;

namespace td_revision.Models.Repository
{
    public abstract class NamedManagerGenerique<TEntity> : ManagerGenerique<TEntity>, INamedRepository<TEntity>
    where TEntity : class, INamedEntity 
    {
        protected NamedManagerGenerique(ProduitsbdContext context) : base(context)
        {
        }

        public virtual async Task<TEntity?> GetByNameAsync(string name)
        {
            return await dbSet.FirstOrDefaultAsync(e => e.Nom == name);
        }
    }
}
