namespace WebUI.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}