using IkeaNotifier.Models.TgApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.TgApi;

public class UpdateContainer
{
	private readonly BotStatsRepository _chatStatsRepository;

	private readonly List<ResultModel> _latestIncomeMessages;

	public UpdateContainer(BotStatsRepository chatStatsRepository)
	{
		_latestIncomeMessages = new List<ResultModel>();
		_chatStatsRepository = chatStatsRepository;

	}

	public async Task AddNewResults(UpdateModel incomingUpdates)
	{
		var lastUpdateId = GetLastUpdateId();
		var orderedUpdates = incomingUpdates.Result.OrderBy(u => u.UpdateId);
		var maxUpdateId = lastUpdateId;

		foreach (var update in orderedUpdates)
		{
			if (update.UpdateId <= lastUpdateId)
			{
				continue;
			}

			if (_latestIncomeMessages.Count > 40)
			{
				_latestIncomeMessages.RemoveAt(_latestIncomeMessages.Count - 1);
			}

			if (update.UpdateId > maxUpdateId)
			{
				maxUpdateId = update.UpdateId;
			}

			_latestIncomeMessages.Add(update);
		}

		await _chatStatsRepository.PushBotLastUpdated(maxUpdateId);
	}

	public long GetLastUpdateId() => _chatStatsRepository.GetBotLastUpdated();

	public List<ResultModel> PullNewMessages()
	{
		var updates = new List<ResultModel>(_latestIncomeMessages);
		_latestIncomeMessages.Clear();

		return updates;
	}
}