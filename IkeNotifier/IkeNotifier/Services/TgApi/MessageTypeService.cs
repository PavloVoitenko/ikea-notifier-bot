using IkeaNotifier.Models.TgApi;
using IkeaNotifier.Options;

using Microsoft.Extensions.Options;

namespace IkeaNotifier.Services.TgApi;

public class MessageTypeService
{
	private readonly string _botName;

	public MessageTypeService(IOptions<BotOptions> botOptions)
	{
		_botName = botOptions.Value.BotName;
	}

	public CommandType ResolveMessage(ResultModel model)
	{
		return CommandType.SaveEndpoint;
	}

	//private bool IsStatsUpdateCommand(string text) => CheckTextIsMessage(text, "/dick");

	private bool CheckTextIsMessage(string text, string message)
	{
		return text == message || text == $"{message}@{_botName}";
	}
}

public enum CommandType
{
	None,
	SaveEndpoint
}