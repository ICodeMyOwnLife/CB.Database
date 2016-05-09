using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public interface IEntityDataQueryAsync<TEntity> where TEntity: class
    {
        #region Abstract
        Task DeleteEntityAsync(TEntity entity);
        Task DeleteEntityAsync(params object[] keyValues);
        Task<TEntity[]> GetEntitiesAsync();
        Task<TEntity> GetEntityAsync(params object[] keyValues);
        #endregion
    }
}