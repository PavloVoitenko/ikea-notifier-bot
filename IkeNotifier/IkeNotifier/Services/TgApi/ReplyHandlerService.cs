using IkeaNotifier.Models.TgApi;
using Microsoft.Extensions.Configuration;

namespace IkeaNotifier.Services.TgApi;

public class ReplyHandlerService
{
	private readonly string _botName;

	public ReplyHandlerService(IConfiguration config)
	{
		_botName = config["BotName"];
	}

	public CommandType ResolveMessage(ResultModel model)
	{
		return CommandType.None;
	}

	//private bool IsStatsUpdateCommand(string text) => CheckTextIsMessage(text, "/dick");

	private bool CheckTextIsMessage(string text, string message)
	{
		return text == message || text == $"{message}@{_botName}";
	}
}

public enum CommandType
{
	None
}