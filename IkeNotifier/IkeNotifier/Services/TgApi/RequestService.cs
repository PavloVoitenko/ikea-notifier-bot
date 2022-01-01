using IkeaNotifier.Models.TgApi;
using IkeaNotifier.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.TgApi;

public class RequestService
{
	private readonly HttpClient _httpClient;

	private readonly string _botCode;
	private readonly string _apiAddress;

	public RequestService(IOptions<BotOptions> botOptions, HttpClient client)
	{
		var options = botOptions.Value;

		_botCode = options.BotCode;
		_apiAddress = options.Api;
		_httpClient = client;
	}

	public async Task<UpdateModel> GetUpdates(long currentOffset)
	{
		var result = await _httpClient.GetAsync(_apiAddress + _botCode + "/getUpdates?offset=" + currentOffset);
		var content = await result.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<UpdateModel>(content);
	}

	public async Task SendText(long chatId, string text = "")
	{
		await _httpClient.GetAsync(_apiAddress + _botCode + "/sendMessage?chat_id=" + chatId + "&text=" + text);
	}

	public async Task ReplyText(long chatId, int messageId, string text = "")
	{
		await _httpClient.GetAsync($"{_apiAddress}{_botCode}/sendMessage?chat_id={chatId}&text={text}&reply_to_message_id={messageId}");
	}
}