using System;
using System.Data.Entity;
using System.Linq.Expressions;


namespace CB.Database.EntityFramework
{
    public class NoTrackingEntityDataAccess<TDbContext, TModel>: IEntityDataAccess<TModel>
        where TDbContext: DbContext, new() where TModel: class
    {
        #region Fields
        private readonly NoTrackingModelDbContextBase<TDbContext> _dataAccess =
            new NoTrackingModelDbContextBase<TDbContext>();
        #endregion


        #region Methods
        public void DeleteEntity(TModel entity)
            => _dataAccess.DeleteModel(entity);

        public void DeleteEntity(params object[] keyValues)
            => _dataAccess.DeleteModel<TModel>(keyValues);

        public TModel[] GetEntities()
            => _dataAccess.GetModels<TModel>();

        public TModel GetEntity(params object[] keyValues)
            => _dataAccess.GetModel<TModel>(keyValues);

        public void RegisterDependent<TProperty>(Expression<Func<TModel, TProperty>> dependentExpression)
        {
            _dataAccess.RegisterDependent(dependentExpression);
        }

        public TModel SaveItem(TModel entity)
            => _dataAccess.SaveModel(entity);
        #endregion
    }
}