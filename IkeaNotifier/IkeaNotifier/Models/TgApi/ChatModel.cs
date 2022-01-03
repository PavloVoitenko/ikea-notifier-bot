using Newtonsoft.Json;

namespace IkeaNotifier.Models.TgApi;

public class ChatModel
{
	[JsonProperty(PropertyName = "id")]
	public long ChatId { get; set; }

	[JsonProperty(PropertyName = "first_name")]
	public string FirstName { get; set; }

	[JsonProperty(PropertyName = "username")]
	public string UserName { get; set; }

	[JsonProperty(PropertyName = "type")]
	public ChatTypeEnum Type { get; set; }
}

public enum ChatTypeEnum
{
	Private,
	Group,
	Supergroup,
	Channel
}