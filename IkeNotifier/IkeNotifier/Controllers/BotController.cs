using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IkeaNotifier.Controllers;

public class BotController : Controller
{
	private readonly IConfiguration _config;


	public IActionResult Index()
	{
		return View();
	}
}