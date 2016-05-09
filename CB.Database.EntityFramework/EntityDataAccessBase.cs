using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public abstract class EntityDataAccessBase<TDbContext>: IEntityDataAccess where TDbContext: DbContext, new()
    {
        #region Abstract
        public abstract TEntity[] GetEntities<TEntity, TProperty>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class;

        public abstract TEntity[] GetEntities<TEntity>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntities, params string[] inclusions)
            where TEntity: class;

        public abstract Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, params string[] inclusions)
            where TEntity: class;

        public abstract Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transformEntities, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class;
        #endregion


        #region Methods
        public virtual void DeleteEntity<TEntity>(TEntity entity) where TEntity: class
            => UseDataContext(context => DeleteEntity(context, entity));

        public virtual void DeleteEntity<TEntity>(params object[] keyValues) where TEntity : class
            => UseDataContext(context => DeleteEntity(context, GetEntity<TEntity>(context, keyValues)));

        public virtual async Task DeleteEntityAsync<TEntity>(params object[] keyValues) where TEntity : class
            => await UseDataContextAsync(async context => await DeleteEntityAsync(context, await GetEntityAsync<TEntity>(context, keyValues)));


        public virtual async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity: class
            => await UseDataContextAsync(async context => { await DeleteEntityAsync(context, entity); });

        public virtual void ForEach<TEntity>(Action<TEntity> action) where TEntity: class
            => UseDataContext(context =>
            {
                foreach (var entity in context.Set<TEntity>())
                {
                    action(entity);
                }
            });

        public virtual async Task ForEachAsync<TEntity>(Action<TEntity> action) where TEntity: class
            => await UseDataContextAsync(async context => await context.Set<TEntity>().ForEachAsync(action));

        public virtual TEntity[] GetEntities<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> path)
            where TEntity: class
            => GetEntities(entities => entities, path);

        public virtual TEntity[] GetEntities<TEntity>(params string[] inclusions) where TEntity: class
            => GetEntities<TEntity>(entities => entities, inclusions);

        public virtual TEntity[] GetEntities<TEntity, TProperty>(Func<TEntity, bool> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => GetEntities(entities => entities.Where(predicate), path);

        public virtual TEntity[] GetEntities<TEntity>(Func<TEntity, bool> predicate,
            params string[] inclusions) where TEntity: class
            => GetEntities<TEntity>(entities => entities.Where(predicate), inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => await GetEntitiesAsync(entities => entities, path);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity>(params string[] inclusions) where TEntity: class
            => await GetEntitiesAsync<TEntity>(entities => entities, inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate, params string[] inclusions) where TEntity: class
            => await
               GetEntitiesAsync<TEntity>(entities => entities.Where(predicate), inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => await GetEntitiesAsync(entities => entities.Where(predicate), path);

        public virtual TEntity GetEntity<TEntity>(params object[] keyValues) where TEntity: class
            => FetchDataContext(context => GetEntity<TEntity>(context, keyValues));

        public virtual async Task<TEntity> GetEntityAsync<TEntity>(params object[] keyValues) where TEntity: class
            => await FetchDataContextAsync(async context => await GetEntityAsync<TEntity>(context, keyValues));
        #endregion


        #region Implementation
        protected virtual TDbContext CreateContext()
        {
            return new TDbContext();
        }

        protected static void DeleteEntity<TEntity>(TDbContext context, TEntity entity) where TEntity: class
        {
            if (entity == null) return;

            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        protected static async Task DeleteEntityAsync<TEntity>(TDbContext context, TEntity entity) where TEntity: class
        {
            if (entity == null) return;

            context.Entry(entity).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        protected virtual TResult FetchDataContext<TResult>(Func<TDbContext, TResult> fetchContext)
        {
            using (var context = CreateContext())
            {
                return fetchContext(context);
            }
        }

        protected virtual async Task<TResult> FetchDataContextAsync<TResult>(
            Func<TDbContext, Task<TResult>> fetchContextAsync)
        {
            using (var context = new TDbContext())
            {
                return await fetchContextAsync(context);
            }
        }

        protected virtual TResult FetchEntitySet<TEntity, TResult>(Func<DbSet<TEntity>, TResult> fetchEntitySet)
            where TEntity: class
            => FetchDataContext(context =>
            {
                var entitySet = context.Set<TEntity>();
                return fetchEntitySet(entitySet);
            });

        protected virtual async Task<TResult> FetchEntitySetAsync<TEntity, TResult>(
            Func<DbSet<TEntity>, Task<TResult>> fetchEntitySet) where TEntity: class
            => await FetchDataContextAsync(async context =>
            {
                var entitySet = context.Set<TEntity>();
                return await fetchEntitySet(entitySet);
            });

        protected virtual TResult FetchIncludedEntitySet<TEntity, TResult>(Func<DbQuery<TEntity>, TResult> fetchQuery,
            string[] inclusions) where TEntity: class
            => FetchEntitySet<TEntity, TResult>(entitySet =>
            {
                var query = IncludeToEntitySet(entitySet, inclusions);
                return fetchQuery(query);
            });

        protected virtual TResult FetchIncludedEntitySet<TEntity, TResult, TProperty>(
            Func<IQueryable<TEntity>, TResult> fetchQuery, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class
            => FetchEntitySet<TEntity, TResult>(entitySet => fetchQuery(entitySet.Include(path)));

        protected virtual async Task<TResult> FetchIncludedEntitySetAsync<TEntity, TResult, TProperty>(
            Func<IQueryable<TEntity>, Task<TResult>> fetchQuery, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class
            => await FetchEntitySetAsync<TEntity, TResult>(async entitySet => await fetchQuery(entitySet.Include(path)));

        protected virtual async Task<TResult> FetchIncludedEntitySetAsync<TEntity, TResult>(
            Func<IQueryable<TEntity>, Task<TResult>> fetchQuery,
            string[] inclusions) where TEntity: class
            => await FetchEntitySetAsync<TEntity, TResult>(async entitySet =>
            {
                var query = IncludeToEntitySet(entitySet, inclusions);
                return await fetchQuery(query);
            });

        protected virtual TEntity GetEntity<TEntity>(TDbContext context, params object[] keyValues) where TEntity: class
            => context.Set<TEntity>().Find(keyValues);

        protected virtual TEntity GetEntity<TEntity>(IDbSet<TEntity> entitySet, params object[] keyValues)
            where TEntity: class
            => entitySet.Find(keyValues);

        protected virtual async Task<TEntity> GetEntityAsync<TEntity>(TDbContext context, params object[] keyValues)
            where TEntity: class
            => await context.Set<TEntity>().FindAsync(keyValues);

        protected virtual async Task<TEntity> GetEntityAsync<TEntity>(DbSet<TEntity> entitySet,
            params object[] keyValues)
            where TEntity: class
            => await entitySet.FindAsync(keyValues);

        protected static DbQuery<TEntity> IncludeToEntitySet<TEntity>(DbQuery<TEntity> entitySet,
            IEnumerable<string> inclusions) where TEntity: class
            => inclusions.Aggregate(entitySet, (current, t) => current.Include(t));

        protected virtual void UseDataContext(Action<TDbContext> useContext)
            => FetchDataContext<object>(context =>
            {
                useContext(context);
                return null;
            });

        protected virtual async Task UseDataContextAsync(Func<TDbContext, Task> useContextAsync)
            => await FetchDataContextAsync<object>(async context =>
            {
                await useContextAsync(context);
                return null;
            });
        #endregion
    }
}