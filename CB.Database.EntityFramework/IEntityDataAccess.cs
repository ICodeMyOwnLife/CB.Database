using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public interface IEntityDataAccess
    {
        #region Abstract
        void DeleteEntity<TEntity>(TEntity entity) where TEntity: class;
        Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity: class;

        TEntity[] GetEntities<TEntity, TProperty>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class;

        TEntity[] GetEntities<TEntity>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities, params string[] inclusions)
            where TEntity: class;

        TEntity[] GetEntities<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> path)
            where TEntity: class;

        TEntity[] GetEntities<TEntity>(params string[] inclusions) where TEntity: class;

        TEntity[] GetEntities<TEntity, TProperty>(Func<TEntity, bool> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class;

        TEntity[] GetEntities<TEntity>(Func<TEntity, bool> predicate,
            params string[] inclusions) where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, params string[] inclusions)
            where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> path) where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity>(params string[] inclusions) where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate, params string[] inclusions) where TEntity: class;

        Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class;
        #endregion
    }
}