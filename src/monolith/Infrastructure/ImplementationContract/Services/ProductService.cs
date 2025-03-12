namespace Infrastructure.ImplementationContract.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    ILogger<ProductService> logger) : IProductService
{
    public async Task<Result<PagedResponse<IEnumerable<Product>>>> GetProductAsync(ProductFilter filter,
        CancellationToken token = default)
    {
        try
        {
            Result<IEnumerable<Product>> products = await productRepository.GetAllAsync(filter, token);
            if (!products.IsSuccess)
                return Result<PagedResponse<IEnumerable<Product>>>.Failure(products.Error);

            PagedResponse<IEnumerable<Product>> response =
                PagedResponse<IEnumerable<Product>>
                    .Create(filter.PageSize, filter.PageNumber, products.Value!.Count(), products.Value);
            return Result<PagedResponse<IEnumerable<Product>>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute query.");
            return Result<PagedResponse<IEnumerable<Product>>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Product>> GetProductDetailAsync(Guid productId, CancellationToken token = default)
    {
        try
        {
            Result<Product?> result = await productRepository.GetByIdAsync(productId, token);
            if (!result.IsSuccess)
                return Result<Product>.Failure(result.Error);

            return Result<Product>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<Product>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> CreateProductAsync(Product request, CancellationToken token = default)
    {
        try
        {
            Result<int> result = await productRepository.AddAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(request.Id) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> UpdateProductAsync(Guid productId, Product request, CancellationToken token = default)
    {
        try
        {
            request.Id = productId;
            Result<int> result = await productRepository.UpdateAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(productId) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<BaseResult> DeleteProductAsync(Guid productId, CancellationToken token = default)
    {
        try
        {
            Result<int> result = await productRepository.DeleteAsync(productId, token);
            return result.IsSuccess ? BaseResult.Success() : BaseResult.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return BaseResult.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }
}
