using IkeaNotifier.Models.Mongo;
using IkeaNotifier.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.Repositories;

public class BotStatsRepository
{
	private readonly IMongoCollection<BotStatsDto> _lastUpdatedCollection;

	public BotStatsRepository(IOptions<MongoOptions> options, MongoClient client)
	{
		var mongoOptions = options.Value;
		var database = client.GetDatabase(mongoOptions.DatabaseName);

		_lastUpdatedCollection = database.GetCollection<BotStatsDto>(mongoOptions.LastUpdatedCollectionName);
	}

	public async Task<long> GetBotLastUpdated() => (await GetBotStats())?.LastUpdateId ?? long.MinValue;

	public async Task PushBotLastUpdated(long updateId)
	{
		var botStats = await GetBotStats() ?? new BotStatsDto();
		botStats.LastUpdateId = updateId;

		var replaceOptions = new ReplaceOptions { IsUpsert = true };

		await _lastUpdatedCollection.ReplaceOneAsync(Builders<BotStatsDto>.Filter.Empty, botStats, replaceOptions);
	}

	private async Task<BotStatsDto> GetBotStats() =>
		await _lastUpdatedCollection
			.Find(Builders<BotStatsDto>.Filter.Empty)
			.FirstOrDefaultAsync();
}