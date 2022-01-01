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
			SetTimers();
		}
	}

	private void SetTimers()
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

			switch (command)
			{
				case CommandType.SaveEndpoint:
					var response = await _userEndpointService.AddUserEndpoint(message.UserId, message.UserName, message.ChatId, message.Message.Text);

					await _requestService.ReplyText(message.ChatId, message.MessageId, response);
					break;
			}
		}
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);

		_replyTimer?.Dispose();
	}
}