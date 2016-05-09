using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public interface IIdEntityDataQueryAsync<TEntity>: IEntityDataQueryAsync<TEntity> where TEntity: IdEntityBase
    {
        #region Abstract
        Task DeleteEntityAsync(int entityId);
        Task<TEntity> GetEntityAsync(int entityId);
        Task<TEntity> SaveEntityAsync(TEntity entity);
        #endregion
    }
}