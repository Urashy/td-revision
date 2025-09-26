using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;
using System.Linq.Expressions;

namespace td_revision.Models.Repository
{
    public abstract class ManagerGenerique<TEntity> : IDataRepository<TEntity> where TEntity : class
    {
        protected readonly ProduitsbdContext context;
        protected readonly DbSet<TEntity> dbSet;

        public ManagerGenerique(ProduitsbdContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<ActionResult<TEntity?>> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public abstract Task<ActionResult<TEntity?>> GetByStringAsync(string str);

        public virtual async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
