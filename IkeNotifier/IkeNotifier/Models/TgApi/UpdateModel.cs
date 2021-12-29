using Newtonsoft.Json;
using System.Collections.Generic;

namespace IkeaNotifier.Models.TgApi
{
	public class UpdateModel
	{
		[JsonProperty(PropertyName = "ok")]
		public string Ok { get; set; }

		[JsonProperty(PropertyName = "result")]
		public List<ResultModel> Result { get; set; }
	}
}
