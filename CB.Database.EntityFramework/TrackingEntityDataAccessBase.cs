using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public abstract class TrackingEntityDataAccessBase<TDbContext>: EntityDataAccessBase<TDbContext>
        where TDbContext: DbContext, new()
    {
        #region Override
        public override TEntity[] GetEntities<TEntity, TProperty>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities,
            Expression<Func<TEntity, TProperty>> path)
            => FetchIncludedEntitySet(entities => transformEntities(entities).ToArray(), path);

        public override TEntity[] GetEntities<TEntity>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities,
            params string[] inclusions)
            => FetchIncludedEntitySet<TEntity, TEntity[]>(entities => transformEntities(entities).ToArray(), inclusions);

        public override async Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, params string[] inclusions)
            =>
                await
                FetchIncludedEntitySetAsync<TEntity, TEntity[]>(
                    async entities => await transformEntities(entities).ToArrayAsync(),
                    inclusions);

        public override async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, Expression<Func<TEntity, TProperty>> path)
            => await FetchIncludedEntitySetAsync(async entites => await transformEntities(entites).ToArrayAsync(), path);
        #endregion
    }
}