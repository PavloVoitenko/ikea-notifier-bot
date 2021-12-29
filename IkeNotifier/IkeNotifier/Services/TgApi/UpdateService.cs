using System.Threading;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.TgApi
{
	public class UpdateService
	{
		private readonly RequestService _requestService;
		//private readonly UpdateContainer _container;
		private readonly ReplyHandlerService _replyHandler;
		//private readonly UserRepository _userRepository;

		private Timer _replyTimer;
		private Timer _notificationTimer;


		public UpdateService(
			RequestService requestService,
			//UpdateContainer container,
			//UserRepository userRepository,
			ReplyHandlerService replyHandler)
		{
			_requestService = requestService;
			//_container = container;
			_replyHandler = replyHandler;
			//_userRepository = userRepository;
		}

		public void StartUpdates()
		{
			if (_replyTimer == null)
			{
				SetTimer();
			}
		}

		private void SetTimers()
		{
			// Create a timer with a two second interval.
			_replyTimer = new Timer(1000);
			// Hook up the Elapsed event for the timer. 
			_replyTimer.Elapsed += UpdateTimer;
			_replyTimer.Elapsed += ReplyTimer;

			_replyTimer.AutoReset = true;
			_replyTimer.Enabled = true;
		}

		private async void UpdateTimer(object source, ElapsedEventArgs e)
		{
			await SaveUpdates(e);

		}

		private async void ReplyTimer(object source, ElapsedEventArgs e)
		{
			await ProceedReplies(e);
		}

		public async Task SaveUpdates(ElapsedEventArgs updateTime = null)
		{
			var updates = await _requestService.GetUpdates(_container.GetLastUpdateId());

			await _container.AddNewResults(updates);
		}

		private async Task ProceedReplies(ElapsedEventArgs e)
		{
			var newMessages = _container.PullNewMessages();

			foreach (var message in newMessages)
			{
				var command = _replyHandler.ResolveMessage(message);

				if (command == CommandType.None)
				{
					continue;
				}

				var currentUser = await _userRepository.GetUserAsync(message.UserTelegramId(), message.UserChatId());

				if (currentUser == null)
				{
					currentUser = await _userRepository.CreateDefaultUser(
						message.UserTelegramId(),
						message.UserName(),
						message.UserChatId());
				}

				switch (command)
				{
					case CommandType.GetStats:
						await _requestService.ReplyText(message.UserChatId(), message.MessageId(), text: currentUser.Stats.ToString());
						break;
					case CommandType.GetWinrateRating:
						await _requestService.ReplyText(message.UserChatId(), message.MessageId(), text: await _statsService.GetRating(currentUser));
						break;
					case CommandType.GetLengthRating:
						await _requestService.ReplyText(message.UserChatId(), message.MessageId(), text: await _statsService.GetLengthRating(currentUser));
						break;
					case CommandType.UpdateStats:
						var result = await _statsService.UpdateLength(currentUser);

						await _requestService.ReplyText(message.UserChatId(), message.MessageId(), text: result);
						break;
					case CommandType.PerformFight:
						await foreach (var fightResult in _fightingService.PerformRandomFight(currentUser))
						{
							System.Threading.Thread.Sleep(500);
							await _requestService.SendText(message.UserChatId(), fightResult.ToString());
						}
						break;
				}
			}
		}
	}
}
