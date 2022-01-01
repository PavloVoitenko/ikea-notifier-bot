using IkeaNotifier.Services;
using IkeaNotifier.Services.TgApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IkeaNotifier.Controllers;

[Route("api")]
public class BotController : Controller
{
	private readonly UpdateService _updateService;
	private readonly PollingService _pollingService;

	public BotController(UpdateService updateService, PollingService pollingService)
	{
		_updateService = updateService;
		_pollingService = pollingService;
	}


	[HttpGet("start")]
	public IActionResult Start()
	{
		_updateService.StartUpdates();

		return Ok();
	}

	[HttpGet("poll")]
	public async Task<IActionResult> Poll()
	{
		await _pollingService.PollUserEndpoints();

		return Ok();
	}
}