using System;
using System.Data.Entity;
using System.Threading.Tasks;


namespace CB.Database.EntityFramework
{
    public abstract class ModelDbContextBase<TDbContext> where TDbContext: DbContext, new()
    {
        protected virtual Task<TResult> FetchDataContextAsync<TResult>(Func<TDbContext, Task<TResult>> fetchContextAsync)
        {
            return FetchDataContext(fetchContextAsync);
        }

        protected virtual TResult FetchDataContext<TResult>(Func<TDbContext, TResult> fetchContext)
        {
            using (var context = new TDbContext())
            {
                return fetchContext(context);
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

/*        private static Task UseDataContextAsync(Func<TDbContext, Task> useContextAsync)
        {
            return FetchDataContextAsync<Task>(context => useContextAsync(context));
        }*/
    }
}