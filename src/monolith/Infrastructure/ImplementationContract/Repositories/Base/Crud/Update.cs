namespace Infrastructure.ImplementationContract.Repositories.Base.Crud;

public sealed class Update<T>:IUpdate<T> where T : BaseEntity
{
    public async Task<Result<int>> UpdateAsync(T value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<int>> UpdateAsync(Guid id, T value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}