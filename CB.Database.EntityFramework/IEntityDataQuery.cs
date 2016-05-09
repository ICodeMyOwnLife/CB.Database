namespace CB.Database.EntityFramework
{
    public interface IEntityDataQuery<TEntity>: IEntityDataQuerySync<TEntity>, IEntityDataQueryAsync<TEntity> where TEntity: class { }
}