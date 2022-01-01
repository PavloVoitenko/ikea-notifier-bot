using IkeaNotifier.Services.Repositories;
using IkeaNotifier.Services.TgApi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IkeaNotifier.Services;

public class PollingService
{
	private readonly UserRepository _userRepository;
	private readonly HttpClient _httpClient;
	private readonly RequestService _requestService;

	public PollingService(UserRepository userRepository, HttpClient client, RequestService requestService)
	{
		_userRepository = userRepository;
		_httpClient = client;
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
				if (await IsEndpointValid(endpoint))
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

	private async Task<bool> IsEndpointValid(string uri)
	{
		var response = await _httpClient.GetAsync(uri);
		var htmlText = await response.Content.ReadAsStringAsync();

		var driverOptions = new ChromeOptions();
		driverOptions.AddArgument("headless");

		using (var chromeDriver = new ChromeDriver(driverOptions))
		{
			chromeDriver.Navigate().GoToUrl(uri);
			var element = chromeDriver.FindElement(By.CssSelector("#content > div > div > div > div.range-revamp-product__subgrid.product-pip.js-product-pip > div.range-revamp-product__buy-module-container > div > div.range-revamp-product-availability > div.js-available-for-delivery > div > span > span > span.range-revamp-status__text.range-revamp-status__text--leading"));
			if (element.Text == "Немає в наявності")
			{
				return false;
			}
		}

		return true;
	}
}