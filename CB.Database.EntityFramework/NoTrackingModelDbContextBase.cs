using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class NoTrackingModelDbContextBase<TDbContext> where TDbContext: DbContext, new()
    {
        #region Fields
        private readonly EntityManager _entityManager = new EntityManager();
        #endregion


        #region Methods
        public virtual void DeleteModel<TModel>(params object[] keyValues) where TModel: class
            => UseDataContext(context =>
            {
                var model = GetModel<TModel>(context, keyValues);
                DeleteModel(context, model);
            });

        public virtual void DeleteModel<TModel>(TModel model) where TModel: class
        {
            if (model != null)
                UseDataContext(context => DeleteModel(context, model));
        }

        public virtual void DeleteModel<TModel>(int modelId) where TModel: IdModelBase, new()
            => UseDataContext(context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                context.SaveChanges();
            });

        public virtual async void DeleteModelAsync<TModel>(params object[] keyValues) where TModel: class
            => await UseDataContextAsync(
                async context => await DeleteModelAsync(context, await GetModelAsync<TModel>(context, keyValues)));

        public virtual async Task DeleteModelAsync<TModel>(TModel model) where TModel: IdModelBase, new()
        {
            if (model == null) return;

            await UseDataContextAsync(async context => { await DeleteModelAsync(context, model); });
        }

        public virtual async Task DeleteModelAsync<TModel>(int modelId) where TModel: IdModelBase, new()
            => await UseDataContextAsync(async context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                await context.SaveChangesAsync();
            });

        public virtual TModel GetModel<TModel>(params object[] keyValues) where TModel: class
            => FetchDataContext(context => GetModel<TModel>(context, keyValues));

        /*public virtual TModel GetModel<TModel>(TModel inModel) where TModel: class
            => GetModel<TModel>(_entityManager.GetEntityIdValues(inModel).ToArray());*/

        public virtual TModel GetModel<TModel>(int modelId) where TModel: class
            => FetchDataContext(context => GetModel(modelId, context.Set<TModel>()));



        /*public virtual async Task<TModel> GetModelAsync<TModel>(TModel inModel) where TModel: class
            => await GetModelAsync<TModel>(_entityManager.GetEntityIdValues(inModel).ToArray());*/

        public virtual async Task<TModel> GetModelAsync<TModel>(params object[] keyValues) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync<TModel>(context, keyValues));

        public virtual async Task<TModel> GetModelAsync<TModel>(int modelId) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync(modelId, context.Set<TModel>()));

        public virtual TModel[] GetModels<TModel>(params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.ToArray(), inclusions);

        public virtual TModel[] GetModels<TModel, TProperty>(Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => FetchIncludedModelSet(query => query.ToArray(), path);

        public virtual TModel[] GetModels<TModel>(Func<IEnumerable<TModel>, IEnumerable<TModel>> transformModelSet,
            params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => transformModelSet(query).ToArray(), inclusions);

        public virtual TModel[] GetModels<TModel, TProperty>(
            Func<IEnumerable<TModel>, IEnumerable<TModel>> transformModelSet,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => FetchIncludedModelSet(query => transformModelSet(query).ToArray(), path);

        public virtual TModel[] GetModels<TModel>(Func<TModel, bool> predicate, params string[] inclusions)
            where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.Where(predicate).ToArray(), inclusions);

        public virtual TModel[] GetModels<TModel, TProperty>(Func<TModel, bool> predicate,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => FetchIncludedModelSet(query => query.Where(predicate).ToArray(), path);

        public virtual async Task<TModel[]> GetModelsAsync<TModel>(params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query => await query.ToArrayAsync(), inclusions);

        public virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => await FetchIncludedModelSetAsync(async query => await query.ToArrayAsync(), path);

        public virtual async Task<TModel[]> GetModelsAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query
                                                                  => await transform(query).ToArrayAsync(), inclusions);

        public virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => await FetchIncludedModelSetAsync(async query => await transform(query).ToArrayAsync(), path);

        public virtual async Task<TModel[]> GetModelsAsync<TModel>(Expression<Func<TModel, bool>> predicate,
            params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await query.Where(predicate).ToArrayAsync(), inclusions);

        public virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => await FetchIncludedModelSetAsync(async query => await query.Where(predicate).ToArrayAsync(), path);

        public virtual TModel[] GetModelsWithNoTracking<TModel>(Func<TModel, bool> predicate,
            params string[] inclusions) where TModel: class
            => GetModelsWithNoTracking<TModel>(models => models.Where(predicate), inclusions);

        public virtual TModel[] GetModelsWithNoTracking<TModel>(params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.AsNoTracking().ToArray(), inclusions);

        public virtual TModel[] GetModelsWithNoTracking<TModel>(
            Func<IEnumerable<TModel>, IEnumerable<TModel>> transform, params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => transform(query.AsNoTracking()).ToArray(), inclusions);

        public virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<TModel, bool> predicate, params string[] inclusions) where TModel: class
            => await GetModelsWithNoTrackingAsync<TModel>(models => models.Where(m => predicate(m)), inclusions);

        public virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(params string[] inclusions)
            where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await query.AsNoTracking().ToArrayAsync(), inclusions);

        public virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await transform(query.AsNoTracking()).ToArrayAsync(), inclusions);

        public bool IsModelExists<TModel>(TModel model) where TModel: class
            => FetchDataContext(context =>
            {
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                var entitySet = objContext.CreateObjectSet<TModel>().EntitySet;
                
                var entityKeyValues = entitySet.ElementType.KeyMembers.Select(
                    keyMember =>
                    {
                        var keyMemberName = keyMember.Name;
                        var keyMemberValue = typeof(TModel).GetProperty(keyMemberName).GetValue(model);
                        return new KeyValuePair<string, object>(keyMemberName, keyMemberValue);
                    }).ToArray();

                if (entityKeyValues.Any(p => p.Value == null)) return false;

                object value;
                var entityKey = new EntityKey(GetQualifiedEntitySetName(entitySet), entityKeyValues);
                return objContext.TryGetObjectByKey(entityKey, out value);
            });

        /*public void RegisterDependent<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
            => _entityManager.AddDependent(propertyExpression);*/

        /*public void RegisterDependentProperties<TModel>(
            params Expression<Func<TModel, object>>[] dependentPropertyExpressions)
            => _entityManager.AddDependent(dependentPropertyExpressions);*/

        /*public void RegisterIdProperties<TModel>(params Expression<Func<TModel, object>>[] idPropertyExpressions)
            => _entityManager.AddIdProperties(idPropertyExpressions);*/

        public virtual TModel SaveModel<TModel>(TModel model) where TModel: class
            => FetchDataContext(context =>
            {
                MarkSavedModel(model, context);
                context.SaveChanges();
                return model;
            });

        public virtual async Task<TModel> SaveModelAsync<TModel>(TModel model) where TModel: class
            => await FetchDataContextAsync(async context =>
            {
                MarkSavedModel(model, context);
                await context.SaveChangesAsync();
                return model;
            });

        public void SaveModelV2<TModel>(TModel model) where TModel: class
        {
            UseDataContext(context =>
            {
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                var entitySet = objContext.CreateObjectSet<TModel>().EntitySet;
            });
        }

        public bool TryGetModel<TModel>(TModel noTrackingModel, out TModel trackingModel) where TModel: class
        {
            object value = null;
            var success = false;

            UseDataContext(context =>
            {
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                var entitySet = objContext.CreateObjectSet<TModel>().EntitySet;

                var entityKeyValues = entitySet.ElementType.KeyMembers.Select(
                    keyMember =>
                    {
                        var keyMemberName = keyMember.Name;
                        var keyMemberValue = typeof(TModel).GetProperty(keyMemberName).GetValue(noTrackingModel);
                        return new KeyValuePair<string, object>(keyMemberName, keyMemberValue);
                    }).ToArray();

                if (entityKeyValues.Any(p => p.Value == null)) return;

                var entityKey = new EntityKey(GetQualifiedEntitySetName(entitySet), entityKeyValues);
                success = objContext.TryGetObjectByKey(entityKey, out value);
            });

            trackingModel = (TModel)value;
            return success;
        }
        #endregion


        #region Implementation
        protected virtual TDbContext CreateContext()
        {
            return new TDbContext();
        }

        private static void DeleteModel<TModel>(TDbContext context, TModel model) where TModel: class
        {
            MarkDeletedModel(model, context);
            context.SaveChanges();
        }

        private static async Task DeleteModelAsync<TModel>(TDbContext context, TModel model) where TModel: class
        {
            MarkDeletedModel(model, context);
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

        protected virtual TResult FetchIncludedModelSet<TModel, TResult>(Func<DbQuery<TModel>, TResult> fetchQuery,
            string[] inclusions) where TModel: class
            => FetchModelSet<TModel, TResult>(modelSet =>
            {
                var query = IncludeToModelSet(modelSet, inclusions);
                return fetchQuery(query);
            });

        protected virtual TResult FetchIncludedModelSet<TModel, TResult, TProperty>(
            Func<IQueryable<TModel>, TResult> fetchQuery, Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => FetchModelSet<TModel, TResult>(modelSet => fetchQuery(modelSet.Include(path)));

        protected virtual async Task<TResult> FetchIncludedModelSetAsync<TModel, TResult, TProperty>(
            Func<IQueryable<TModel>, Task<TResult>> fetchQuery, Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => await FetchModelSetAsync<TModel, TResult>(async modelSet => await fetchQuery(modelSet.Include(path)));

        protected virtual async Task<TResult> FetchIncludedModelSetAsync<TModel, TResult>(
            Func<IQueryable<TModel>, Task<TResult>> fetchQuery,
            string[] inclusions) where TModel: class
            => await FetchModelSetAsync<TModel, TResult>(async modelSet =>
            {
                var query = IncludeToModelSet(modelSet, inclusions);
                return await fetchQuery(query);
            });

        protected virtual TResult FetchModelSet<TModel, TResult>(Func<DbSet<TModel>, TResult> fetchModelSet)
            where TModel: class
            => FetchDataContext(context =>
            {
                var modelSet = context.Set<TModel>();
                return fetchModelSet(modelSet);
            });

        protected virtual async Task<TResult> FetchModelSetAsync<TModel, TResult>(
            Func<DbSet<TModel>, Task<TResult>> fetchModelSet) where TModel: class
            => await FetchDataContextAsync(async context =>
            {
                var modelSet = context.Set<TModel>();
                return await fetchModelSet(modelSet);
            });

        private static TModel GetModel<TModel>(TDbContext context, object[] keyValues) where TModel: class
            => context.Set<TModel>().Find(keyValues);

        private static TModel GetModel<TModel>(int modelId, IDbSet<TModel> modelSet) where TModel: class
            => modelSet.Find(modelId);

        private static async Task<TModel> GetModelAsync<TModel>(TDbContext context, object[] keyValues)
            where TModel: class
            => await context.Set<TModel>().FindAsync(keyValues);

        private static async Task<TModel> GetModelAsync<TModel>(int modelId, DbSet<TModel> modelSet)
            where TModel: class
            => await modelSet.FindAsync(modelId);

        private static string GetQualifiedEntitySetName(EntitySetBase entitySet)
        {
            return $"{entitySet.EntityContainer.Name}.{entitySet.Name}";
        }

        private static DbQuery<TModel> IncludeToModelSet<TModel>(DbQuery<TModel> modelSet,
            IEnumerable<string> inclusions) where TModel: class
            => inclusions.Aggregate(modelSet, (current, t) => current.Include(t));

        private static void MarkDeletedModel<TModel>(int modelId, TDbContext context) where TModel: IdModelBase, new()
            => MarkDeletedModel(new TModel { Id = modelId }, context);

        private static void MarkDeletedModel<TModel>(TModel model, TDbContext context) where TModel: class
            => context.Entry(model).State = EntityState.Deleted;

        private void MarkSavedModel<TModel>(TModel model, TDbContext context) where TModel: class
        {
            //context.Entry(model).
            context.Entry(model).State = IsModelExists(model) ? EntityState.Modified : EntityState.Added;
            //_entityManager.SetPropertiesState(model, context, EntityState.Modified);
        }

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