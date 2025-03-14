namespace WebUI.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.Client, NoStore = true)]
public sealed class ErrorController : BaseController
{
    [Route("error/404")]
    public IActionResult Error404()
    {
        ErrorViewModel model = new ()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 404;
        return View( model);
    }

    [Route("error/500")]
    public IActionResult Error500()
    {
        ErrorViewModel model = new ()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 500;
        return View( model);
    }

    [Route("error/401")]
    public IActionResult Error401()
    {
        ErrorViewModel model = new()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 401;
        return View( model);
    }

    [Route("error/403")]
    public IActionResult Error403()
    {
        ErrorViewModel model = new()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 403;
        return View(model);
    }

    [Route("error/{code}")]
    public IActionResult ErrorCodeHandler(int code)
    {
        ErrorViewModel model = new ()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = code;
        return View("Error", model);
    }
}