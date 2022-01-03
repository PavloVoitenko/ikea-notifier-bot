using IkeaNotifier.Services.Repositories;
using IkeaNotifier.Services.TgApi;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IkeaNotifier.Services;

public class PollingService
{
	private readonly UserRepository _userRepository;
	private readonly WebDriver _driver;
	private readonly RequestService _requestService;

	public PollingService(
		UserRepository userRepository,
		WebDriver driver,
		RequestService requestService)
	{
		_userRepository = userRepository;
		_driver = driver;
		_requestService = requestService;
	}

	public async Task PollUserEndpoints()
	{
		var users = await _userRepository.GetUsers();

		foreach (var user in users)
		{
			var avaiableEndpoints = new List<string>();

			foreach (var endpoint in user.PolledPages)
			{
				if (IsEndpointValid(endpoint))
				{
					avaiableEndpoints.Add(endpoint);
				}
			}

			if (avaiableEndpoints.Count != 0)
			{
				await _requestService.SendText(user.ChatId, "Following items are available:\n" + string.Join('\n', avaiableEndpoints));
			}
		}
	}

	private bool IsEndpointValid(string uri)
	{
		_driver.Navigate().GoToUrl(uri);
		var element = _driver.FindElement(By.CssSelector("#content > div > div > div > div.range-revamp-product__subgrid.product-pip.js-product-pip > div.range-revamp-product__buy-module-container > div > div.range-revamp-product-availability > div.js-available-for-delivery > div > span > span > span.range-revamp-status__text.range-revamp-status__text--leading"));

		return element.Text != "Немає в наявності";
	}
}