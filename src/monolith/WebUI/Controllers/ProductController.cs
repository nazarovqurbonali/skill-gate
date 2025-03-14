namespace WebUI.Controllers;

[Authorize]
[Route("products")]
public sealed class ProductController(
    IProductService productService,
    ILogger<ProductController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProductFilter filter, CancellationToken token)
    {
        logger.LogInformation("Fetching products with filter: {@Filter}", filter);
        Result<PagedResponse<IEnumerable<Product>>> result = await productService.GetProductAsync(filter, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(PagedResponse<IEnumerable<Product>>.Create(filter.PageSize, filter.PageNumber, 0, []));
        }

        return View(result.Value);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching details for product {Id}", id);
        Result<Product> result = await productService.GetProductDetailAsync(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("create")]
    public IActionResult Create()
        => View();

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Creating new product: {ProductName}", model.Name);
        Result<Guid> result = await productService.CreateProductAsync(model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "Product created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching product for edit with id: {Id}", id);
        Result<Product> result = await productService.GetProductDetailAsync(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Product model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Updating product with id: {Id}", id);
        Result<Guid> result = await productService.UpdateProductAsync(id, model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "Product updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        logger.LogWarning("Deleting product with id: {Id}", id);
        BaseResult result = await productService.DeleteProductAsync(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Product deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }
}