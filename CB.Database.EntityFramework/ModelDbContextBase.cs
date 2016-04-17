using System;
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
        protected virtual void DeleteModel<TModel>(int modelId) where TModel: IdModelBase, new()
            => UseDataContext(context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                context.SaveChanges();
            });

        protected virtual async Task DeleteModelAsync<TModel>(int modelId) where TModel: IdModelBase, new()
            => await UseDataContextAsync(async context =>
            {
                MarkDeletedModel<TModel>(modelId, context);
                await context.SaveChangesAsync();
            });

        protected virtual TResult FetchDataContext<TResult>(Func<TDbContext, TResult> fetchContext)
        {
            using (var context = new TDbContext())
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
            => FetchDataContext(context => GetModelSet<TModel>(context).ToArray());

        protected virtual TModel[] GetModels<TModel>(Func<TModel, bool> predicate) where TModel: class
            => FetchDataContext(context => GetModelSet<TModel>(context).Where(predicate).ToArray());

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>() where TModel: class
            => await FetchDataContextAsync(async context => await GetModelSet<TModel>(context).ToArrayAsync());

        protected virtual async Task<TModel[]> GetModelsAsync<TModel>(Expression<Func<TModel, bool>> predicate)
            where TModel: class
            => await FetchDataContextAsync(async context
                                           => await GetModelSet<TModel>(context).Where(predicate).ToArrayAsync());

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

        private static void MarkDeletedModel<TModel>(int modelId, TDbContext context) where TModel: IdModelBase, new()
            => context.Entry(new TModel { Id = modelId }).State = EntityState.Deleted;

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