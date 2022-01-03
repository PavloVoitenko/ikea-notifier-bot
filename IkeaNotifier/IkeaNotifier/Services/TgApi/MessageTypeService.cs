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
		if (model == null || model.Message == null)
		{
			return CommandType.None;
		}

		var message = model.Message.Text;

		return message switch
		{
			_ when IsCommand(message, "/start") => CommandType.CreateUser,
			_ when IsCommand(message, "/viewitems") => CommandType.ViewEndpoints,
			_ when IsCommand(message.Split(' ')[0], "/removeitem") => CommandType.RemoveEndpoint,
			_ => CommandType.SaveEndpoint
		};
	}

	private bool IsCommand(string text, string command) => text == command || text == $"{command}@{_botName}";
}

public enum CommandType
{
	None,
	CreateUser,
	SaveEndpoint,
	ViewEndpoints,
	RemoveEndpoint
}