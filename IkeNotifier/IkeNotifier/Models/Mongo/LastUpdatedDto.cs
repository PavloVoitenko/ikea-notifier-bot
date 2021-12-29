using MongoDB.Bson.Serialization.Attributes;

namespace IkeaNotifier.Models.Mongo;

[BsonIgnoreExtraElements]
public class LastUpdatedDto
{
	public long LastUpdateId { get; set; } = int.MinValue;
}
