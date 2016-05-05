namespace CB.Database.EntityFramework
{
    public interface IEntityDataAccess<TModel>
    {
        #region Abstract
        void DeleteEntity(TModel entity);
        void DeleteEntity(params object[] keyValues);
        TModel[] GetEntities();
        TModel GetEntity(params object[] keyValues);
        TModel SaveItem(TModel entity);
        #endregion
    }
}