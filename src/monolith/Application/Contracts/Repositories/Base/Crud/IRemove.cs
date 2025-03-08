namespace Application.Contracts.Repositories.Base.Crud;

public interface IRemove<in T> where T : BaseEntity
{
    Task<Result<int>> DeleteAsync(Guid id,CancellationToken token=default);
    Task<Result<int>> DeleteAsync(T value,CancellationToken token=default);
}