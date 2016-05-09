using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public interface IIdEntityDataQuery<TEntity>
        : IIdEntityDataQuerySync<TEntity>, IIdEntityDataQueryAsync<TEntity>, IEntityDataQuery<TEntity>
        where TEntity: IdEntityBase { }
}