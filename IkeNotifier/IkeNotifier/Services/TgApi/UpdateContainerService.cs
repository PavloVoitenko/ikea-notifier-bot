using IkeaNotifier.Models.TgApi;
using IkeaNotifier.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.TgApi;

public class UpdateContainerService
{
	private readonly BotStatsRepository _botStatsRepository;
	private readonly List<ResultModel> _latestIncomingMessages = new();

	public UpdateContainerService(BotStatsRepository chatStatsRepository)
	{
		_botStatsRepository = chatStatsRepository;
	}

	public async Task AddNewResults(UpdateModel incomingUpdates)
	{
		var lastUpdateId = await GetLastUpdateId();
		var orderedUpdates = incomingUpdates.Result.OrderBy(u => u.UpdateId);
		var maxUpdateId = lastUpdateId;

		foreach (var update in orderedUpdates.Where(u => u.UpdateId > lastUpdateId).OrderBy(u => u.UpdateId))
		{
			maxUpdateId = update.UpdateId;

			_latestIncomingMessages.Add(update);
		}

		await _botStatsRepository.PushBotLastUpdated(maxUpdateId);
	}

	public async Task<long> GetLastUpdateId() => await _botStatsRepository.GetBotLastUpdated();

	public List<ResultModel> PullNewMessages()
	{
		var updates = new List<ResultModel>(_latestIncomingMessages);
		_latestIncomingMessages.Clear();

		return updates;
	}
}