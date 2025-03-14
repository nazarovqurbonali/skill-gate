namespace WebUI.Controllers;

[Route("identity")]
public sealed class IdentityController(
    ILogger<IdentityController> logger,
    IIdentityService service) : BaseController
{
    [HttpGet("login")]
    public IActionResult Login() => View(new LoginRequest());

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken token = default)
    {
        logger.LogInformation("Processing login request for {Login}", request.Login);
    
        if (!ModelState.IsValid)
            return View(request);

        Result<LoginResponse> result = await service.LoginAsync(request, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error.Message);
            return View(request);
        }

        TempData["SuccessMessage"] = "You have successfully logged in.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register")]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken token = default)
    {
        if (!ModelState.IsValid)
            return View(request);

        Result<RegisterResponse> result = await service.RegisterAsync(request, token);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error.Message);
            return View(request);
        }

        TempData["SuccessMessage"] = "Registration successful. Please log in.";
        return RedirectToAction("Login");
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        BaseResult result = await service.LogoutAsync();
        if (!result.IsSuccess)
        {
            return result.Error.ErrorType switch
            {
                ErrorType.BadRequest => BadRequest(result.Error.Message),
                _ => StatusCode(500, result.Error.Message)
            };
        }

        return RedirectToAction("Index", "Home");
    }
}