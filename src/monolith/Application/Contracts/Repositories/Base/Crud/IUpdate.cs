namespace Application.Contracts.Repositories.Base.Crud;

public interface IUpdate<in T> where T : BaseEntity
{
    Task<Result<int>> UpdateAsync(T value,CancellationToken token=default);
    Task<Result<int>> UpdateAsync(Guid id, T value,CancellationToken token=default);
}