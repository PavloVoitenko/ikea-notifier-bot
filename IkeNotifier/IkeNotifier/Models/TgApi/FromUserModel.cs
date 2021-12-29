using Newtonsoft.Json;

namespace IkeaNotifier.Models.TgApi
{
	public class FromUserModel
	{
		[JsonProperty(PropertyName = "id")]
		public int UserId { get; set; }

		[JsonProperty(PropertyName = "is_bot")]
		public bool IsBot { get; set; }

		[JsonProperty(PropertyName = "first_name")]
		public string FirstName { get; set; }

		[JsonProperty(PropertyName = "username")]
		public string UserName { get; set; }

		[JsonProperty(PropertyName = "language_code")]
		public string LanguageCode { get; set; }
	}
}
