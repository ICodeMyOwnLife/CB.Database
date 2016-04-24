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
    public abstract class ModelDbContextBase<TDbContext> where TDbContext: DbContext, new()
    {
        #region Implementation
        protected virtual TDbContext CreateContext()
        {
            return new TDbContext();
        }

        protected virtual void DeleteMode<TModel>(TModel model) where TModel: IdModelBase
            => UseDataContext(context =>
            {
                MarkDeletedModel(model, context);
                context.SaveChanges();
            });

        protected virtual void DeleteModel<TModel>(int modelId) where TModel: IdModelBase, new()
            => UseDataContext(context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                context.SaveChanges();
            });

        protected virtual async Task DeleteModelAsync<TModel>(TModel model) where TModel: IdModelBase, new()
            => await UseDataContextAsync(async context =>
            {
                MarkDeletedModel(model, context);
                await context.SaveChangesAsync();
            });

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

        protected virtual async Task<TResult> FetchIncludedModelSetAsync<TModel, TResult>(
            Func<DbQuery<TModel>, Task<TResult>> fetchQuery,
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
                var modelSet = GetModelSet<TModel>(context);
                return fetchModelSet(modelSet);
            });

        protected virtual async Task<TResult> FetchModelSetAsync<TModel, TResult>(
            Func<DbSet<TModel>, Task<TResult>> fetchModelSet) where TModel: class
            => await FetchDataContextAsync(async context =>
            {
                var modelSet = GetModelSet<TModel>(context);
                return await fetchModelSet(modelSet);
            });

        protected virtual TModel GetModel<TModel>(int modelId) where TModel: class
            => FetchDataContext(context => GetModel(modelId, GetModelSet<TModel>(context)));

        private static TModel GetModel<TModel>(int modelId, IDbSet<TModel> modelSet) where TModel: class
            => modelSet.Find(modelId);

        protected virtual async Task<TModel> GetModelAsync<TModel>(int modelId) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync(modelId, GetModelSet<TModel>(context)));

        private static async Task<TModel> GetModelAsync<TModel>(int modelId, DbSet<TModel> modelSet)
            where TModel: class
            => await modelSet.FindAsync(modelId);

        protected virtual TModel[] GetModels<TModel>(params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.ToArray(), inclusions);

        protected virtual TModel[] GetModels<TModel>(Func<IEnumerable<TModel>, IEnumerable<TModel>> transformModelSet,
            params string[] inclusions) where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => transformModelSet(query).ToArray(), inclusions);

        protected virtual TModel[] GetModels<TModel>(Func<TModel, bool> predicate, params string[] inclusions)
            where TModel: class
            => FetchIncludedModelSet<TModel, TModel[]>(query => query.Where(predicate).ToArray(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query => await query.ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform, params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(async query
                                                                  => await transform(query).ToArrayAsync(), inclusions);

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(Expression<Func<TModel, bool>> predicate,
            params string[] inclusions) where TModel: class
            => await FetchIncludedModelSetAsync<TModel, TModel[]>(
                async query => await query.Where(predicate).ToArrayAsync(), inclusions);

        private static DbSet<TModel> GetModelSet<TModel>(TDbContext context) where TModel: class
        {
            Type contextType = typeof(TDbContext),
                 dbSetType = typeof(DbSet<TModel>);

            var dbSetProp = contextType.GetProperties().FirstOrDefault(p => p.PropertyType == dbSetType);

            if (dbSetProp == null)
            {
                throw new InvalidOperationException($"Cannot find a property of type {dbSetType} in {contextType}");
            }
            return dbSetProp.GetValue(context) as DbSet<TModel>;
        }

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

        private static void MarkDeletedModel<TModel>(TModel model, TDbContext context) where TModel: IdModelBase
            => context.Entry(model).State = EntityState.Deleted;

        private static void MarkSavedModel<TModel>(TModel model, TDbContext context) where TModel: IdModelBase
            => context.Entry(model).State = !model.Id.HasValue || model.Id.Value == 0
                                                ? EntityState.Added
                                                : EntityState.Modified;

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