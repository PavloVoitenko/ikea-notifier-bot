using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace IkeaNotifier.Models.Mongo;

[BsonIgnoreExtraElements]
public class UserDto
{
	public int UserId { get; set; }
	public long ChatId { get; set; }

	public string Username { get; set; }

	public List<string> PolledPages { get; set;}
}