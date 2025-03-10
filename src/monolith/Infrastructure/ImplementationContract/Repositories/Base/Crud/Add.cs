namespace Infrastructure.ImplementationContract.Repositories.Base.Crud;

public sealed class Add<T> : IAdd<T> where T : BaseEntity
{
    public async Task<Result<int>> AddAsync(T entity, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<int>> AddRangeAsync(ICollection<T> entities, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

   
}