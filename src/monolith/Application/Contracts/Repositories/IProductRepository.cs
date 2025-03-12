namespace Application.Contracts.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Result<IEnumerable<Product>>> GetAllAsync(ProductFilter filter, CancellationToken token = default);

}