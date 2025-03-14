namespace WebUI.Controllers;

[Authorize]
[Route("users")]
public sealed class UserController(
    IUserService userService,
    ILogger<UserController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UserFilter filter, CancellationToken token)
    {
        logger.LogInformation("Fetching users with filter: {@Filter}", filter);
        Result<PagedResponse<IEnumerable<User>>> result = await userService.GetUsersAsync(filter, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(PagedResponse<IEnumerable<User>>.Create(filter.PageSize, filter.PageNumber, 0, []));
        }

        return View(result.Value);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        logger.LogInformation("Fetching details for user {Id}", id);
        Result<User> result = await userService.GetByIdForUser(id, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [HttpGet("edit")]
    public async Task<IActionResult> Edit(CancellationToken token)
    {
        logger.LogInformation("Fetching user for edit ");
        Result<User> result = await userService.GetByIdForSelf(token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [HttpPost("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Updating user");
        Result<Guid> result = await userService.UpdateProfileAsync(model, token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "User updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> ProfileAsync(CancellationToken token = default)
    {
        Result<User> result = await userService.GetByIdForSelf(token);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }
}