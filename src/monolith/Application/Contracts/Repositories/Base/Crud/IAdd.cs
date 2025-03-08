namespace Application.Contracts.Repositories.Base.Crud;

public interface IAdd<in T> where T : BaseEntity
{
    Task<Result<int>> AddAsync(T entity,CancellationToken token=default);
    Task<Result<int>> AddRangeAsync(IEnumerable<T> entities,CancellationToken token=default);
}