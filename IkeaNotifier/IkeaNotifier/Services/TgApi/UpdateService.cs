using System.Threading.Tasks;
using System;

namespace IkeaNotifier.Services.TgApi;

public class UpdateService : IDisposable
{
	private readonly RequestService _requestService;
	private readonly UpdateContainerService _container;
	private readonly MessageTypeService _messageTypeService;
	private readonly UserEndpointService _userEndpointService;

	private System.Timers.Timer _replyTimer;

	public UpdateService(
		RequestService requestService,
		UpdateContainerService container,
		MessageTypeService messageTypeService,
		UserEndpointService userEndpointService)
	{
		_requestService = requestService;
		_container = container;
		_messageTypeService = messageTypeService;
		_userEndpointService = userEndpointService;
	}

	public void StartUpdates()
	{
		if (_replyTimer == null)
		{
			SetTimer();
		}
	}

	private void SetTimer()
	{
		_replyTimer = new System.Timers.Timer(2000)
		{
			AutoReset = true,
			Enabled = true
		};
		_replyTimer.Elapsed += async (_, _) => await SaveUpdates();
		_replyTimer.Elapsed += async (_, _) => await ProceedReplies();
	}

	public async Task SaveUpdates()
	{
		var updates = await _requestService.GetUpdates(await _container.GetLastUpdateId());

		await _container.AddNewResults(updates);
	}

	private async Task ProceedReplies()
	{
		foreach (var message in _container.PullNewMessages())
		{
			var command = _messageTypeService.ResolveMessage(message);

			if (command == CommandType.None)
			{
				continue;
			}

			var responseTask = command switch
			{
				CommandType.CreateUser => _userEndpointService.CreateUser(message.UserId, message.ChatId, message.UserName),
				CommandType.SaveEndpoint => _userEndpointService.AddUserEndpoint(message.UserId, message.ChatId, message.MessageId, message.Message.Text),
				CommandType.ViewEndpoints => _userEndpointService.ViewUserEndpoints(message.UserId, message.ChatId),
				CommandType.RemoveEndpoint => GetRemoveItemTask(message.UserId, message.ChatId, message.MessageId, message.Message.Text),
				_ => Task.CompletedTask,
			};

			await responseTask;
		}
	}

	public Task GetRemoveItemTask(int userId, long chatId, int messageId, string text)
	{
		var parts = text.Split(' ');
		return parts.Length != 2 || !int.TryParse(parts[1], out var position) ?
			_requestService.ReplyText(chatId, messageId, "/removeitem command requires a single number like: \"/removeitem 1\"") :
			_userEndpointService.RemoveEndpoint(userId, chatId, position);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);

		_replyTimer?.Dispose();
	}
}