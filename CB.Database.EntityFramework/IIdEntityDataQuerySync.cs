using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public interface IIdEntityDataQuerySync<TEntity>: IEntityDataQuerySync<TEntity> where TEntity: IdEntityBase
    {
        #region Abstract
        void DeleteEntity(int entityId);
        TEntity GetEntity(int entityId);
        TEntity SaveEntity(TEntity entity);
        #endregion
    }
}