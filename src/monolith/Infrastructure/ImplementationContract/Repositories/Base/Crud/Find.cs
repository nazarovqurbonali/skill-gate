namespace Infrastructure.ImplementationContract.Repositories.Base.Crud;

public sealed class Find<T> : IFind<T> where T : BaseEntity
{
    public async Task<Result<T?>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync(string query, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

   
}