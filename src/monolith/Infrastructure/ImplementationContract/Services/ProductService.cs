namespace Infrastructure.ImplementationContract.Services;

public sealed class ProductService : IProductService
{
    public async Task<Result<PagedResponse<IEnumerable<Product>>>> GetProductAsync(ProductFilter filter,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Product>> GetProductDetailAsync(Guid productId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> CreateProductAsync(Product request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> UpdateProductAsync(Guid productId, Product request,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResult> DeleteProductAsync(Guid productId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}