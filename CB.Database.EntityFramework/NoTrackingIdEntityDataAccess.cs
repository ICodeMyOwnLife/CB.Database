using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class NoTrackingIdEntityDataAccess<TDbContext> where TDbContext: DbContext, new()
    {
        #region Fields
        private readonly EntityManager _entityManager = new EntityManager();
        #endregion


        #region Methods
        public virtual void DeleteEntity<TEntity>(TEntity entity) where TEntity: class
            => UseDataContext(context => DeleteEntity(context, entity));

        public virtual void DeleteEntity<TEntity>(int entityId) where TEntity: class, IIdEntity, new()
            => UseDataContext(context => DeleteEntity<TEntity>(context, entityId));

        public virtual async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity: class, IIdEntity, new()
            => await UseDataContextAsync(async context => { await DeleteEntityAsync(context, entity); });

        public virtual async Task DeleteEntityAsync<TEntity>(int entityId) where TEntity: class, IIdEntity, new()
            => await UseDataContextAsync(async context => { await DeleteEntityAsync<TEntity>(context, entityId); });

        public virtual TEntity[] GetEntities<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> path)
            where TEntity: class
            => FetchIncludedEntitySet(query => query.AsNoTracking().ToArray(), path);

        public virtual TEntity[] GetEntities<TEntity, TProperty>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformEntitySet,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => FetchIncludedEntitySet(query => transformEntitySet(query.AsNoTracking()).ToArray(), path);

        public virtual TEntity[] GetEntities<TEntity, TProperty>(Func<TEntity, bool> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => FetchIncludedEntitySet(query => query.AsNoTracking().Where(predicate).ToArray(), path);

        public virtual TEntity[] GetEntities<TEntity>(Func<TEntity, bool> predicate,
            params string[] inclusions) where TEntity: class
            => GetEntities<TEntity>(entities => entities.Where(predicate), inclusions);

        public virtual TEntity[] GetEntities<TEntity>(params string[] inclusions) where TEntity: class
            => FetchIncludedEntitySet<TEntity, TEntity[]>(query => query.AsNoTracking().ToArray(), inclusions);

        public virtual TEntity[] GetEntities<TEntity>(
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>> transformQuery, params string[] inclusions)
            where TEntity: class
            =>
                FetchIncludedEntitySet<TEntity, TEntity[]>(query => transformQuery(query.AsNoTracking()).ToArray(),
                    inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            => await FetchIncludedEntitySetAsync(async query => await query.AsNoTracking().ToArrayAsync(), path);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transform, params string[] inclusions) where TEntity: class
            => await FetchIncludedEntitySetAsync<TEntity, TEntity[]>(
                async query => await transform(query.AsNoTracking()).ToArrayAsync(), inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> transform, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class
            =>
                await
                FetchIncludedEntitySetAsync(async query => await transform(query.AsNoTracking()).ToArrayAsync(), path);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate, params string[] inclusions) where TEntity: class
            => await GetEntitiesAsync<TEntity>(entities => entities.AsNoTracking().Where(predicate), inclusions);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity, TProperty>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProperty>> path) where TEntity: class
            =>
                await
                FetchIncludedEntitySetAsync(async query => await query.AsNoTracking().Where(predicate).ToArrayAsync(),
                    path);

        public virtual async Task<TEntity[]> GetEntitiesAsync<TEntity>(params string[] inclusions)
            where TEntity: class
            => await FetchIncludedEntitySetAsync<TEntity, TEntity[]>(
                async query => await query.AsNoTracking().ToArrayAsync(), inclusions);

        public virtual TEntity GetEntity<TEntity>(TEntity entity, params string[] inclusions)
            where TEntity: class, IIdEntity
            => GetEntity<TEntity>(entity.Id, inclusions);

        public virtual TEntity GetEntity<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> path)
            where TEntity: class, IIdEntity
            => GetEntity(entity.Id, path);

        public virtual TEntity GetEntity<TEntity>(int entityId, params string[] inclusions)
            where TEntity: class, IIdEntity
            => FetchDataContext(context => GetEntity(entityId, context.Set<TEntity>(), inclusions));

        public virtual TEntity GetEntity<TEntity, TProperty>(int entityId,
            Expression<Func<TEntity, TProperty>> path = null) where TEntity: class, IIdEntity
            => FetchDataContext(context => GetEntity(entityId, context.Set<TEntity>(), path));

        public virtual async Task<TEntity> GetEntityAsync<TEntity>(TEntity entity, params string[] inclusions)
            where TEntity: class, IIdEntity
            => await GetEntityAsync<TEntity>(entity.Id, inclusions);

        public virtual async Task<TEntity> GetEntityAsync<TEntity, TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> path = null) where TEntity: class, IIdEntity
            => await GetEntityAsync(entity.Id, path);

        public virtual async Task<TEntity> GetEntityAsync<TEntity>(int entityId, params string[] inclusions)
            where TEntity: class, IIdEntity
            =>
                await
                FetchDataContextAsync(
                    async context => await GetEntityAsync(entityId, context.Set<TEntity>(), inclusions));

        public virtual async Task<TEntity> GetEntityAsync<TEntity, TProperty>(int entityId,
            Expression<Func<TEntity, TProperty>> path = null) where TEntity: class, IIdEntity
            =>
                await
                FetchDataContextAsync(async context => await GetEntityAsync(entityId, context.Set<TEntity>(), path));

        public TEntity InsertEntity<TEntity>(TEntity entity, bool includeDependent = false)
            where TEntity: class, IIdEntity
            => SaveEntity(entity, EntityState.Added);

        public async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity, bool includeDependent = false)
            where TEntity: class, IIdEntity
            => await SaveEntityAsync(entity, EntityState.Added);

        public bool IsEntityExists<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => IsEntityExists<TEntity>(entity.Id);

        public bool IsEntityExists<TEntity>(int entityId) where TEntity: class, IIdEntity
            => FetchDataContext(context => context.Set<TEntity>().Any(e => e.Id == entityId));

        public void RegisterDependentProperties<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> dependentPropertyExpression,
            Expression<Func<TEntity, int>> dependentPropertyIdExpression) where TEntity: IIdEntity
            where TProperty: IIdEntity
            => _entityManager.AddDependent(dependentPropertyExpression, dependentPropertyIdExpression);

        public TEntity SaveEntity<TEntity>(TEntity entity, EntityState entityState)
            where TEntity: class, IIdEntity
            => FetchDataContext(context =>
            {
                context.Entry(entity).State = entityState;
                _entityManager.SetDependent(entity);
                context.SaveChanges();
                return entity;
            });

        public async Task<TEntity> SaveEntityAsync<TEntity>(TEntity entity, EntityState entityState)
            where TEntity: class, IIdEntity
            => await FetchDataContextAsync(async context =>
            {
                context.Entry(entity).State = entityState;
                _entityManager.SetDependent(entity);
                await context.SaveChangesAsync();
                return entity;
            });

        public TEntity UpdateEntity<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => SaveEntity(entity, EntityState.Modified);

        public async Task<TEntity> UpdateEntityAsync<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => await SaveEntityAsync(entity, EntityState.Modified);
        #endregion


        #region Implementation
        protected virtual TDbContext CreateContext()
        {
            return new TDbContext();
        }

        private static void DeleteEntity<TEntity>(TDbContext context, int entityId)
            where TEntity: class, IIdEntity, new()
        {
            if (entityId > 0)
                DeleteEntity(context, new TEntity { Id = entityId });
        }

        private static void DeleteEntity<TEntity>(TDbContext context, TEntity entity) where TEntity: class
        {
            if (entity == null) return;

            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        private static async Task DeleteEntityAsync<TEntity>(TDbContext context, int entityId)
            where TEntity: class, IIdEntity, new()
        {
            if (entityId > 0)
                await DeleteEntityAsync(context, new TEntity { Id = entityId });
        }

        private static async Task DeleteEntityAsync<TEntity>(TDbContext context, TEntity entity) where TEntity: class
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

        private TEntity GetEntity<TEntity>(int entityId, IDbSet<TEntity> entitySet) where TEntity: class
            => entitySet.Find(entityId);

        private TEntity GetEntity<TEntity>(int entityId, IDbSet<TEntity> entitySet, string[] inclusions)
            where TEntity: class, IIdEntity
            =>
                inclusions == null || inclusions.Length == 0
                    ? GetEntity(entityId, entitySet)
                    : FetchIncludedEntitySet<TEntity, TEntity>(query => query.FirstOrDefault(e => e.Id == entityId),
                        inclusions);

        private TEntity GetEntity<TEntity, TProperty>(int entityId, IDbSet<TEntity> entitySet,
            Expression<Func<TEntity, TProperty>> path)
            where TEntity: class, IIdEntity
            => path == null ? GetEntity(entityId, entitySet)
                   : FetchIncludedEntitySet(query => query.FirstOrDefault(e => e.Id == entityId), path);

        private async Task<TEntity> GetEntityAsync<TEntity>(int entityId, DbSet<TEntity> entitySet)
            where TEntity: class
            => await entitySet.FindAsync(entityId);

        private async Task<TEntity> GetEntityAsync<TEntity>(int entityId, DbSet<TEntity> entitySet, string[] inclusions)
            where TEntity: class, IIdEntity
            =>
                inclusions == null || inclusions.Length == 0
                    ? await GetEntityAsync(entityId, entitySet)
                    : await
                      FetchIncludedEntitySetAsync<TEntity, TEntity>(
                          async query => await query.FirstOrDefaultAsync(e => e.Id == entityId), inclusions);

        private async Task<TEntity> GetEntityAsync<TEntity, TProperty>(int entityId, DbSet<TEntity> entitySet,
            Expression<Func<TEntity, TProperty>> path)
            where TEntity: class, IIdEntity
            => path == null ? await GetEntityAsync(entityId, entitySet)
                   : await
                     FetchIncludedEntitySetAsync(async query => await query.FirstOrDefaultAsync(e => e.Id == entityId),
                         path);

        private static DbQuery<TEntity> IncludeToEntitySet<TEntity>(DbQuery<TEntity> entitySet,
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