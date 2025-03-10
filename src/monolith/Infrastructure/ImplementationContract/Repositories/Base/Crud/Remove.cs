namespace Infrastructure.ImplementationContract.Repositories.Base.Crud;

public sealed class Remove<T>:IRemove<T> where T : BaseEntity
{
    public async Task<Result<int>> DeleteAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<int>> DeleteAsync(T value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}