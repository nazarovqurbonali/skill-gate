namespace WebUI.Controllers;

[Authorize]
[Route("roles")]
public sealed class RoleController(IRoleService roleService, ILogger<RoleController> logger) : BaseController
{
    
}