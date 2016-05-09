using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class NoTrackingIdEntityDataAccess<TDbContext>: NoTrackingEntityDataAccessBase<TDbContext>
        where TDbContext: DbContext, new()
    {
        #region Fields
        private readonly EntityManager _entityManager = new EntityManager();
        #endregion


        #region Methods
        public virtual void DeleteEntity<TEntity>(int entityId) where TEntity: class, IIdEntity, new()
            => UseDataContext(context => DeleteEntity<TEntity>(context, entityId));

        public virtual async Task DeleteEntityAsync<TEntity>(int entityId) where TEntity: class, IIdEntity, new()
            => await UseDataContextAsync(async context => { await DeleteEntityAsync<TEntity>(context, entityId); });

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

        public virtual bool IsEntityExists<TEntity>(int entityId) where TEntity: class, IIdEntity
            => FetchDataContext(context => context.Set<TEntity>().Any(e => e.Id == entityId));

        public bool IsEntityExists<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => IsEntityExists<TEntity>(entity.Id);

        /*public TEntity InsertEntity<TEntity>(TEntity entity, bool includeDependent = false)
            where TEntity: class, IIdEntity
            => SaveEntity(entity, EntityState.Added);

        public async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity, bool includeDependent = false)
            where TEntity: class, IIdEntity
            => await SaveEntityAsync(entity, EntityState.Added);*/

        public void RegisterDependentProperties<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> dependentPropertyExpression,
            Expression<Func<TEntity, int>> dependentPropertyIdExpression) where TEntity: IIdEntity
            where TProperty: IIdEntity
            => _entityManager.AddDependent(dependentPropertyExpression, dependentPropertyIdExpression);

        /*public override TEntity SaveEntity<TEntity>(TEntity entity, EntityState entityState)
            => FetchDataContext(context =>
            {
                context.Entry(entity).State = entityState;
                _entityManager.SetDependent(entity);
                context.SaveChanges();
                return entity;
            });

        public override async Task<TEntity> SaveEntityAsync<TEntity>(TEntity entity, EntityState entityState)
            => await FetchDataContextAsync(async context =>
            {
                context.Entry(entity).State = entityState;
                _entityManager.SetDependent(entity);
                await context.SaveChangesAsync();
                return entity;
            });*/

        /*public TEntity UpdateEntity<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => SaveEntity(entity, EntityState.Modified);

        public async Task<TEntity> UpdateEntityAsync<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => await SaveEntityAsync(entity, EntityState.Modified);*/

        public TEntity SaveEntity<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => FetchDataContext(context =>
            {
                _entityManager.SetDependent(entity);
                context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
                _entityManager.ResetDependent(entity);
                return entity;
            });

        public async Task<TEntity> SaveEntityAsync<TEntity>(TEntity entity) where TEntity: class, IIdEntity
            => await FetchDataContextAsync(async context =>
            {
                _entityManager.SetDependent(entity);
                context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
                await context.SaveChangesAsync();
                _entityManager.ResetDependent(entity);
                return entity;
            });
        #endregion


        #region Implementation
        private static void DeleteEntity<TEntity>(TDbContext context, int entityId)
            where TEntity: class, IIdEntity, new()
        {
            if (entityId > 0)
                DeleteEntity(context, new TEntity { Id = entityId });
        }

        private static async Task DeleteEntityAsync<TEntity>(TDbContext context, int entityId)
            where TEntity: class, IIdEntity, new()
        {
            if (entityId > 0)
                await DeleteEntityAsync(context, new TEntity { Id = entityId });
        }

        private TEntity GetEntity<TEntity>(int entityId, IDbSet<TEntity> entitySet, string[] inclusions)
            where TEntity: class, IIdEntity
            =>
                inclusions == null || inclusions.Length == 0
                    ? GetEntity(entitySet, entityId)
                    : FetchIncludedEntitySet<TEntity, TEntity>(query => query.FirstOrDefault(e => e.Id == entityId),
                        inclusions);

        private TEntity GetEntity<TEntity, TProperty>(int entityId, IDbSet<TEntity> entitySet,
            Expression<Func<TEntity, TProperty>> path)
            where TEntity: class, IIdEntity
            => path == null ? GetEntity(entitySet, entityId)
                   : FetchIncludedEntitySet(query => query.FirstOrDefault(e => e.Id == entityId), path);

        private async Task<TEntity> GetEntityAsync<TEntity>(int entityId, DbSet<TEntity> entitySet, string[] inclusions)
            where TEntity: class, IIdEntity
            =>
                inclusions == null || inclusions.Length == 0
                    ? await GetEntityAsync(entitySet, entityId)
                    : await
                      FetchIncludedEntitySetAsync<TEntity, TEntity>(
                          async query => await query.FirstOrDefaultAsync(e => e.Id == entityId), inclusions);

        private async Task<TEntity> GetEntityAsync<TEntity, TProperty>(int entityId, DbSet<TEntity> entitySet,
            Expression<Func<TEntity, TProperty>> path)
            where TEntity: class, IIdEntity
            => path == null ? await GetEntityAsync(entitySet, entityId)
                   : await
                     FetchIncludedEntitySetAsync(async query => await query.FirstOrDefaultAsync(e => e.Id == entityId),
                         path);
        #endregion
    }
}