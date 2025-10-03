namespace td_revision.Models.Repository
{
    public interface INamedRepository<T> : IRepository<T> where T : class
    {
        Task<T?> GetByNameAsync(string name);
    }
}
