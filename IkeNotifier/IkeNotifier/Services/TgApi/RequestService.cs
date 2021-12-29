
using IkeaNotifier.Models.TgApi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace IkeaNotifier.Services.TgApi
{
	public class RequestService
	{
		private readonly HttpClient _client = new();
		private string BotCode { get; set; }
		private string ApiAddress { get; set; }

		public RequestService(IConfiguration config)
		{
			ApiAddress = config["Api"];
			BotCode = config["BotCode"];
		}

		public async Task<UpdateModel> GetUpdates(long currentOffset)
		{
			var result = await _client.GetAsync(ApiAddress + BotCode + "/getUpdates?offset=" + currentOffset);
			var content = await result.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<UpdateModel>(content);
		}

		public async Task SendText(long chatId, string text = "", ElapsedEventArgs timerInfo = null)
		{
			if (timerInfo != null)
			{
				text += string.Format(" at {0:HH:mm:ss}", timerInfo.SignalTime);
			}

			await _client.GetAsync(ApiAddress + BotCode + "/sendMessage" + "?chat_id=" + chatId + "&text=" + text);
		}

		public async Task ReplyText(long chatId, int replyChatId, ElapsedEventArgs timerInfo = null, string text = "")
		{
			if (timerInfo != null)
			{
				text += string.Format(" at {0:HH:mm:ss}", timerInfo.SignalTime);
			}

			await _client.GetAsync($"{ApiAddress}{BotCode}/sendMessage?chat_id={chatId}&text={text}&reply_to_message_id={replyChatId}");
		}
	}
}
