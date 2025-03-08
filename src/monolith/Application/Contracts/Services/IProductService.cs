namespace Application.Contracts.Services;

public interface IProductService
{
    Task<Result<PagedResponse<IEnumerable<Product>>>> GetProductAsync(
        ProductFilter filter,
        CancellationToken token = default);

    Task<Result<Product>> GetProductDetailAsync(
        Guid productId,
        CancellationToken token = default);

    Task<Result<Guid>> CreateProductAsync(
        Product request,
        CancellationToken token = default);

    Task<Result<Guid>> UpdateProductAsync(
        Guid productId,
        Product request,
        CancellationToken token = default);

    Task<BaseResult> DeleteProductAsync(
        Guid productId,
        CancellationToken token = default);
}