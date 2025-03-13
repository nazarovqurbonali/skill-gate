namespace WebUI.Controllers;

public sealed class ErrorController : Controller
{
    [Route("error/404")]
    public IActionResult Error404()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }

    [Route("error/500")]
    public IActionResult Error500()
    {
        Response.StatusCode = 500;
        return View("ServerError");
    }

    [Route("error/401")]
    public IActionResult Error401()
    {
        Response.StatusCode = 401;
        return View("Unauthorized");
    }

    [Route("error/403")]
    public IActionResult Error403()
    {
        Response.StatusCode = 403;
        return View("Forbidden");
    }

    [Route("error/{code}")]
    public IActionResult ErrorCodeHandler(int code)
    {
        Response.StatusCode = code;
        return View("Error");
    }
}