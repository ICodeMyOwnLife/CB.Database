using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class ModelInfo
    {
        #region  Properties & Indexers
        public PropertyInfo Id { get; set; }
        public PropertyInfo Property { get; set; }
        public PropertyInfo PropertyId { get; set; }
        #endregion


        #region Methods
        public object GetProperty(object source)
            => Property.GetValue(source);
        #endregion
    }

    public class DependentPropertyDictionary
    {
        #region Fields
        readonly IDictionary<Type, IList<ModelInfo>> _dependentDictionary =
            new Dictionary<Type, IList<ModelInfo>>();
        #endregion


        #region Methods
        public void Add<TModel>(PropertyInfo property, PropertyInfo id, PropertyInfo propertyId)
            => Add(typeof(TModel), property, id, propertyId);

        public void Add<TModel, TProperty, TId>(Expression<Func<TModel, TProperty>> propertyExpression,
            Expression<Func<TModel, TId>> idExpression, Expression<Func<TProperty, TId>> propertyIdExpression)
            =>
                Add<TModel>(propertyExpression.GetPropertyInfo(), idExpression.GetPropertyInfo(),
                    propertyIdExpression.GetPropertyInfo());

        public IEnumerable<object> GetProperties<TModel>(TModel model)
        {
            if (model == null) return null;
            var modelType = model.GetType();
            return _dependentDictionary.ContainsKey(modelType)
                       ? _dependentDictionary[modelType].Select(mi => mi.GetProperty(model)) : null;
        }
        #endregion


        #region Implementation
        private void Add(Type type, PropertyInfo property, PropertyInfo id, PropertyInfo propertyId)
        {
            IList<ModelInfo> dependentProperties;

            if (!_dependentDictionary.ContainsKey(type))
            {
                dependentProperties = new List<ModelInfo>();
                _dependentDictionary[type] = dependentProperties;
            }
            else
            {
                dependentProperties = _dependentDictionary[type];
            }
            dependentProperties.Add(new ModelInfo
            {
                Property = property,
                Id = id,
                PropertyId = propertyId
            });
        }
        #endregion
    }

    public abstract class ModelDbContextBase<TDbContext> where TDbContext: DbContext, new()
    {
        #region Fields
        private readonly DependentPropertyDictionary _dependentPropertyDictionary = new DependentPropertyDictionary();
        #endregion


        #region Implementation
        protected virtual TDbContext CreateContext()
        {
            return new TDbContext();
        }

        protected virtual void DeleteModel<TModel>(params object[] keyValues) where TModel: class
            => UseDataContext(context =>
            {
                var model = GetModel<TModel>(context, keyValues);
                DeleteModel(context, model);
            });

        protected virtual void DeleteModel<TModel>(TModel model) where TModel: IdModelBase
        {
            if (model != null)
                UseDataContext(context => DeleteModel(context, model));
        }

        private static void DeleteModel<TModel>(TDbContext context, TModel model) where TModel: class
        {
            MarkDeletedModel(model, context);
            context.SaveChanges();
        }

        protected virtual void DeleteModel<TModel>(int modelId) where TModel: IdModelBase, new()
            => UseDataContext(context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                context.SaveChanges();
            });

        protected virtual async void DeleteModelAsync<TModel>(params object[] keyValues) where TModel: class
            => await UseDataContextAsync(
                async context => await DeleteModelAsync(context, await GetModelAsync<TModel>(context, keyValues)));

        protected virtual async Task DeleteModelAsync<TModel>(TModel model) where TModel: IdModelBase, new()
        {
            if (model == null) return;

            await UseDataContextAsync(async context => { await DeleteModelAsync(context, model); });
        }

        private static async Task DeleteModelAsync<TModel>(TDbContext context, TModel model) where TModel: class
        {
            MarkDeletedModel(model, context);
            await context.SaveChangesAsync();
        }

        protected virtual async Task DeleteModelAsync<TModel>(int modelId) where TModel: IdModelBase, new()
            => await UseDataContextAsync(async context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                await context.SaveChangesAsync();
            });

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

        protected virtual TModel GetModel<TModel>(params object[] keyValues) where TModel: class
            => FetchDataContext(context => GetModel<TModel>(context, keyValues));

        protected virtual TModel GetModel<TModel>(int modelId) where TModel: class
            => FetchDataContext(context => GetModel(modelId, context.Set<TModel>()));

        private static TModel GetModel<TModel>(int modelId, IDbSet<TModel> modelSet) where TModel: class
            => modelSet.Find(modelId);

        private static async Task<TModel> GetModelAsync<TModel>(TDbContext context, object[] keyValues)
            where TModel: class
            => await context.Set<TModel>().FindAsync(keyValues);

        protected virtual async Task<TModel> GetModelAsync<TModel>(params object[] keyValues) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync<TModel>(context, keyValues));

        protected virtual async Task<TModel> GetModelAsync<TModel>(int modelId) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync(modelId, context.Set<TModel>()));

        private static async Task<TModel> GetModelAsync<TModel>(int modelId, DbSet<TModel> modelSet)
            where TModel: class
            => await modelSet.FindAsync(modelId);

        protected virtual TModel[] GetModels<TModel>(params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.ToArray(), inclusions);

        // UNDONE: GetModels(path)
        protected virtual TModel[] GetModels<TModel, TProperty>(Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => FetchIncludedModelSet(query => query.ToArray(), path);

        protected virtual TModel[] GetModels<TModel>(Func<IEnumerable<TModel>, IEnumerable<TModel>> transformModelSet,
            params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => transformModelSet(query).ToArray(), inclusions);

        protected virtual TModel[] GetModels<TModel, TProperty>(
            Func<IEnumerable<TModel>, IEnumerable<TModel>> transformModelSet,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => FetchIncludedModelSet(query => transformModelSet(query).ToArray(), path);

        protected virtual TModel[] GetModels<TModel>(Func<TModel, bool> predicate, params string[] inclusions)
            where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.Where(predicate).ToArray(), inclusions);

        protected virtual TModel[] GetModels<TModel, TProperty>(Func<TModel, bool> predicate,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => FetchIncludedModelSet(query => query.Where(predicate).ToArray(), path);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query => await query.ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => await FetchIncludedModelSetAsync(async query => await query.ToArrayAsync(), path);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query
                                                                  => await transform(query).ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, Expression<Func<TModel, TProperty>> path)
            where TModel: class
            => await FetchIncludedModelSetAsync(async query => await transform(query).ToArrayAsync(), path);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(Expression<Func<TModel, bool>> predicate,
            params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await query.Where(predicate).ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> path) where TModel: class
            => await FetchIncludedModelSetAsync(async query => await query.Where(predicate).ToArrayAsync(), path);

        /*private static DbSet<TModel> GetModelSet<TModel>(TDbContext context) where TModel: class
        {
            Type contextType = typeof(TDbContext),
                 dbSetType = typeof(DbSet<TModel>);

            var dbSetProp = contextType.GetProperties().FirstOrDefault(p => p.PropertyType == dbSetType);

            if (dbSetProp == null)
            {
                throw new InvalidOperationException($"Cannot find a property of type {dbSetType} in {contextType}");
            }
            return dbSetProp.GetValue(context) as DbSet<TModel>;
        }*/

        protected virtual TModel[] GetModelsWithNoTracking<TModel>(Func<TModel, bool> predicate,
            params string[] inclusions) where TModel: class
            => GetModelsWithNoTracking<TModel>(models => models.Where(predicate), inclusions);

        protected virtual TModel[] GetModelsWithNoTracking<TModel>(params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.AsNoTracking().ToArray(), inclusions);

        protected virtual TModel[] GetModelsWithNoTracking<TModel>(
            Func<IEnumerable<TModel>, IEnumerable<TModel>> transform, params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => transform(query.AsNoTracking()).ToArray(), inclusions);

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<TModel, bool> predicate, params string[] inclusions) where TModel: class
            => await GetModelsWithNoTrackingAsync<TModel>(models => models.Where(m => predicate(m)), inclusions);

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(params string[] inclusions)
            where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await query.AsNoTracking().ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await transform(query.AsNoTracking()).ToArrayAsync(), inclusions);

        private static DbQuery<TModel> IncludeToModelSet<TModel>(DbQuery<TModel> modelSet,
            IEnumerable<string> inclusions) where TModel: class
            => inclusions.Aggregate(modelSet, (current, t) => current.Include(t));

        private static void MarkDeletedModel<TModel>(int modelId, TDbContext context) where TModel: IdModelBase, new()
            => MarkDeletedModel(new TModel { Id = modelId }, context);

        private static void MarkDeletedModel<TModel>(TModel model, TDbContext context) where TModel: class
            => context.Entry(model).State = EntityState.Deleted;

        private void MarkSavedModel<TModel>(TModel model, TDbContext context) where TModel: IdModelBase
        {
            context.Entry(model).State = !model.Id.HasValue || model.Id.Value == 0
                                             ? EntityState.Added
                                             : EntityState.Modified;
            var modelProperties = _dependentPropertyDictionary.GetProperties(model);
            if (modelProperties == null) return;

            foreach (var property in modelProperties.Where(p => p != null))
            {
                context.Entry(property).State = EntityState.Unchanged;
            }
        }

        protected void RegisterDependent<TModel, TProperty, TId>(
            Expression<Func<TModel, TProperty>> propertyExpression, Expression<Func<TModel, TId>> idExpression,
            Expression<Func<TProperty, TId>> propertyIdExpression)
            => _dependentPropertyDictionary.Add(propertyExpression, idExpression, propertyIdExpression);

        protected virtual TModel SaveModel<TModel>(TModel model) where TModel: IdModelBase
            => FetchDataContext(context =>
            {
                MarkSavedModel(model, context);
                context.SaveChanges();
                return model;
            });

        protected virtual async Task<TModel> SaveModelAsync<TModel>(TModel model) where TModel: IdModelBase
            => await FetchDataContextAsync(async context =>
            {
                MarkSavedModel(model, context);
                await context.SaveChangesAsync();
                return model;
            });

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