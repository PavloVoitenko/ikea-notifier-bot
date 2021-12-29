using Newtonsoft.Json;

namespace IkeaNotifier.Models.TgApi
{
	public class ResultModel
	{
		[JsonProperty(PropertyName = "update_id")]
		public int UpdateId { get; set; }

		[JsonProperty(PropertyName = "message")]
		public MessageModel Message { get; set; }
	}
}
