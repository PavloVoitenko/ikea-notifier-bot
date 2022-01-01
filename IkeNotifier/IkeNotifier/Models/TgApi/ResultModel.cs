using Newtonsoft.Json;

namespace IkeaNotifier.Models.TgApi;

public class ResultModel
{
	[JsonProperty(PropertyName = "update_id")]
	public int UpdateId { get; set; }

	[JsonProperty(PropertyName = "message")]
	public MessageModel Message { get; set; }

	public string UserName => Message.User.UserName;
	public int UserId => Message.User.UserId;
	public long ChatId => Message.Chat.ChatId;
	public int MessageId => Message.MessageId;
}