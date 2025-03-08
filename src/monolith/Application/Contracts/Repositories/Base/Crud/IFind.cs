namespace Application.Contracts.Repositories.Base.Crud;

public interface IFind<T> where T : BaseEntity
{
    Task<Result<T?>> GetByIdAsync(Guid id,CancellationToken token=default);
    Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken token=default);
    Result<IQueryable<T>> Find(Expression<Func<T, bool>> expression,CancellationToken token=default);
}