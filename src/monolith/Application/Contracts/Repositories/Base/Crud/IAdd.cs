namespace Application.Contracts.Repositories.Base.Crud;

public interface IAdd<T> where T : BaseEntity
{
    Task<Result<int>> AddAsync(T entity,CancellationToken token=default);
    Task<Result<int>> AddRangeAsync(ICollection<T> entities,CancellationToken token=default);
}