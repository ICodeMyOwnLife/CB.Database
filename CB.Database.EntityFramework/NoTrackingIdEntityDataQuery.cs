using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class NoTrackingIdEntityDataQuery<TDbContext, TEntity>: IIdEntityDataQuery<TEntity>
        where TDbContext: DbContext, new() where TEntity: IdEntityBase, new()
    {
        #region Fields
        protected readonly NoTrackingIdEntityDataAccess<TDbContext> _dataAccess =
            new NoTrackingIdEntityDataAccess<TDbContext>();
        #endregion


        #region Methods
        public virtual void DeleteEntity(TEntity entity)
            => _dataAccess.DeleteEntity(entity);

        public virtual void DeleteEntity(params object[] keyValues)
            => _dataAccess.DeleteEntity<TEntity>(keyValues);

        public virtual void DeleteEntity(int entityId)
            => _dataAccess.DeleteEntity<TEntity>(entityId);

        public async Task DeleteEntityAsync(TEntity entity)
            => await _dataAccess.DeleteEntityAsync(entity);

        public async Task DeleteEntityAsync(params object[] keyValues)
            => await _dataAccess.DeleteEntityAsync<TEntity>(keyValues);

        public async Task DeleteEntityAsync(int entityId)
            => await _dataAccess.DeleteEntityAsync<TEntity>(entityId);

        public virtual TEntity[] GetEntities()
            => _dataAccess.GetEntities<TEntity>();

        public async Task<TEntity[]> GetEntitiesAsync()
            => await _dataAccess.GetEntitiesAsync<TEntity>();

        public virtual TEntity GetEntity(params object[] keyValues)
            => _dataAccess.GetEntity<TEntity>(keyValues);

        public virtual TEntity GetEntity(int entityId)
            => _dataAccess.GetEntity<TEntity>(entityId);

        public async Task<TEntity> GetEntityAsync(params object[] keyValues)
            => await _dataAccess.GetEntityAsync<TEntity>(keyValues);

        public async Task<TEntity> GetEntityAsync(int entityId)
            => await _dataAccess.GetEntityAsync<TEntity>(entityId);

        public void RegisterDependent<TProperty>(Expression<Func<TEntity, TProperty>> dependentPropertyExpression,
            Expression<Func<TEntity, int>> dependentPropertyIdExpression) where TProperty: IIdEntity
        {
            _dataAccess.RegisterDependentProperties(dependentPropertyExpression, dependentPropertyIdExpression);
        }

        public virtual TEntity SaveEntity(TEntity entity)
            => _dataAccess.SaveEntity(entity);

        public async Task<TEntity> SaveEntityAsync(TEntity entity)
            => await _dataAccess.SaveEntityAsync(entity);
        #endregion
    }
}