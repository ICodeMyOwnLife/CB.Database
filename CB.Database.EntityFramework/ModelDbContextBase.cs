using System;
using System.Data.Entity;
using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public abstract class ModelDbContextBase<TDbContext> where TDbContext: DbContext, new()
    {
        #region Implementation
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

        protected virtual void UseDataContext(Action<TDbContext> useContext)
        {
            FetchDataContext<object>(context =>
            {
                useContext(context);
                return null;
            });
        }

        protected virtual async Task UseDataContextAsync(Func<TDbContext, Task> useContextAsync)
        {
            await FetchDataContextAsync<object>(async context =>
            {
                await useContextAsync(context);
                return null;
            });
        }
        #endregion
    }
}