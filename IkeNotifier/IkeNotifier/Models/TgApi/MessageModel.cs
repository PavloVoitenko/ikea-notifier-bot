using Newtonsoft.Json;

namespace IkeaNotifier.Models.TgApi;

public class MessageModel
{
	[JsonProperty(PropertyName = "message_id")]
	public int MessageId { get; set; }

	[JsonProperty(PropertyName = "date")]
	public int Date { get; set; }

	[JsonProperty(PropertyName = "text")]
	public string Text { get; set; }

	[JsonProperty(PropertyName = "from")]
	public FromUserModel User { get; set; }

	[JsonProperty(PropertyName = "chat")]
	public ChatModel Chat { get; set; }
}