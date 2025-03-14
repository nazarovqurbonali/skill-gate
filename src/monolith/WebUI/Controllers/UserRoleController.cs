namespace WebUI.Controllers;

[Authorize]
[Route("user-roles")]
public sealed class UserRoleController(
    IUserRoleService userRoleService,
    ILogger<UserRoleController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UserRoleFilter filter, CancellationToken token)
    {
        logger.LogInformation("Fetching user roles with filter: {@Filter}", filter);
        Result<PagedResponse<IEnumerable<UserRole>>> result = await userRoleService.GetUserRolesAsync(filter, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(PagedResponse<IEnumerable<UserRole>>.Create(filter.PageSize, filter.PageNumber, 0, []));
        }

        return View(result.Value);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching details for user role {Id}", id);
        Result<UserRole> result = await userRoleService.GetUserRoleDetailAsync(id, token);
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
    public async Task<IActionResult> Create(UserRole model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Creating new user role: {UserId} - {RoleId}", model.UserId, model.RoleId);
        Result<Guid> result = await userRoleService.CreateUserRoleAsync(model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "User role created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching user role for edit with id: {Id}", id);
        Result<UserRole> result = await userRoleService.GetUserRoleDetailAsync(id, token);
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
    public async Task<IActionResult> Edit(Guid id, UserRole model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Updating user role with id: {Id}", id);
        Result<Guid> result = await userRoleService.UpdateUserRoleAsync(id, model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "User role updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        logger.LogWarning("Deleting user role with id: {Id}", id);
        BaseResult result = await userRoleService.DeleteUserRoleAsync(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "User role deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }
}