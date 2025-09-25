using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using td_revision.Models.EntityFramework;
using System.Linq.Expressions;

namespace td_revision.Models.Repository
{
    public class ManagerGenerique<TEntity> : IDataRepository<TEntity> where TEntity : class
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

        public virtual async Task<ActionResult<TEntity?>> GetByStringAsync(string str)
        {
            throw new NotImplementedException("GetByStringAsync must be implemented in derived class");
        }

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

        protected async Task<ActionResult<TEntity?>> GetByPropertyAsync<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            TProperty value)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, ((MemberExpression)propertySelector.Body).Member.Name);
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            return await dbSet.FirstOrDefaultAsync(lambda);
        }

        protected async Task<ActionResult<TEntity?>> GetByStringPropertyAsync(
            Expression<Func<TEntity, string>> propertySelector,
            string value)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, ((MemberExpression)propertySelector.Body).Member.Name);

            var toUpperMethod = typeof(string).GetMethod("ToUpper", new Type[0]);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var propertyToUpper = Expression.Call(property, toUpperMethod);
            var valueToUpper = Expression.Call(Expression.Constant(value), toUpperMethod);
            var contains = Expression.Call(propertyToUpper, containsMethod, valueToUpper);

            var lambda = Expression.Lambda<Func<TEntity, bool>>(contains, parameter);

            return await dbSet.FirstOrDefaultAsync(lambda);
        }
    }
}
