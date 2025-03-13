namespace WebUI.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.Client, NoStore = true)]
public sealed class ErrorController : BaseController
{
    [Route("error/404")]
    public IActionResult Error404()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 404;
        return View("Error", model);
    }

    [Route("error/500")]
    public IActionResult Error500()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 500;
        return View("Error", model);
    }

    [Route("error/401")]
    public IActionResult Error401()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 401;
        return View("Error", model);
    }

    [Route("error/403")]
    public IActionResult Error403()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = 403;
        return View("Error", model);
    }

    [Route("error/{code}")]
    public IActionResult ErrorCodeHandler(int code)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        Response.StatusCode = code;
        return View("Error", model);
    }
}