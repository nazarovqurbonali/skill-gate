namespace WebUI.Controllers;

[Authorize]
[Route("roles")]
public sealed class RoleController(
    IRoleService roleService,
    ILogger<RoleController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] RoleFilter filter, CancellationToken token)
    {
        logger.LogInformation("Fetching roles with filter: {@Filter}", filter);
        Result<PagedResponse<IEnumerable<Role>>> result = await roleService.GetRolesAsync(filter, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(PagedResponse<IEnumerable<Role>>.Create(filter.PageSize, filter.PageNumber, 0,
                []));
        }

        return View(result.Value);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching details for role {Id}", id);
        Result<Role> result = await roleService.GetRoleDetailAsync(id, token);
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
    public async Task<IActionResult> Create(Role model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Creating new role: {RoleName}", model.Name);
        Result<Guid> result = await roleService.CreateRoleAsync(model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "Role created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching role for edit with id: {Id}", id);
        Result<Role> result = await roleService.GetRoleDetailAsync(id, token);
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
    public async Task<IActionResult> Edit(Guid id, Role model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Updating role with id: {Id}", id);
        Result<Guid> result = await roleService.UpdateRoleAsync(id, model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "Role updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        logger.LogWarning("Deleting role with id: {Id}", id);
        BaseResult result = await roleService.DeleteRoleAsync(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Role deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }
}