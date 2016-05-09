namespace CB.Database.EntityFramework
{
    public interface IEntityDataQuerySync<TEntity> where TEntity: class
    {
        #region Abstract
        void DeleteEntity(TEntity entity);
        void DeleteEntity(params object[] keyValues);
        TEntity[] GetEntities();
        TEntity GetEntity(params object[] keyValues);
        #endregion
    }
}