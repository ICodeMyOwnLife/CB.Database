using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        protected virtual TModel GetModel<TModel>(int modelId) where TModel: class
            => FetchDataContext(context => GetModel(modelId, GetModelSet<TModel>(context)));

        private static TModel GetModel<TModel>(int modelId, IDbSet<TModel> modelSet) where TModel: class
            => modelSet.Find(modelId);

        protected virtual async Task<TModel> GetModelAsync<TModel>(int modelId) where TModel: class
            => await FetchDataContextAsync(async context => await GetModelAsync(modelId, GetModelSet<TModel>(context)));

        private static async Task<TModel> GetModelAsync<TModel>(int modelId, DbSet<TModel> modelSet)
            where TModel: class
            => await modelSet.FindAsync(modelId);

        protected virtual TModel[] GetModels<TModel>() where TModel: class
            => GetModelSet<TModel>().ToArray();

        protected virtual TModel[] GetModels<TModel>(Func<IEnumerable<TModel>, IEnumerable<TModel>> transform)
            where TModel: class
            => transform(GetModelSet<TModel>()).ToArray();

        protected virtual TModel[] GetModels<TModel>(Func<TModel, bool> predicate) where TModel: class
            => GetModelSet<TModel>().Where(predicate).ToArray();

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>() where TModel: class
            => await GetModelSet<TModel>().ToArrayAsync();

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform) where TModel: class
            => await transform(GetModelSet<TModel>()).ToArrayAsync();

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(Expression<Func<TModel, bool>> predicate)
            where TModel: class
            => await GetModelSet<TModel>().Where(predicate).ToArrayAsync();

        protected virtual DbSet<TModel> GetModelSet<TModel>() where TModel: class
            => FetchDataContext(GetModelSet<TModel>);

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

        protected virtual TModel[] GetModelsWithNoTracking<TModel>(Func<TModel, bool> predicate) where TModel: class
            => GetModelsWithNoTracking<TModel>(models => models.Where(predicate));

        protected virtual TModel[] GetModelsWithNoTracking<TModel>() where TModel: class
            => GetModelSet<TModel>().AsNoTracking().ToArray();

        protected virtual TModel[] GetModelsWithNoTracking<TModel>(
            Func<IEnumerable<TModel>, IEnumerable<TModel>> transform) where TModel: class
            => transform(GetModelSet<TModel>().AsNoTracking()).ToArray();

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<TModel, bool> predicate) where TModel: class
            => await GetModelsWithNoTrackingAsync<TModel>(models => models.Where(m => predicate(m)));

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>() where TModel: class
            => await GetModelSet<TModel>().AsNoTracking().ToArrayAsync();

        protected virtual async Task<TModel[]> GetModelsWithNoTrackingAsync<TModel>(
            Func<IQueryable<TModel>, IQueryable<TModel>> transform) where TModel: class
            => await transform(GetModelSet<TModel>().AsNoTracking()).ToArrayAsync();

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