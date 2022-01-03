using IkeaNotifier.Models.Mongo;
using IkeaNotifier.Models.TgApi;
using IkeaNotifier.Services.Repositories;
using IkeaNotifier.Services.TgApi;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IkeaNotifier.Services;

public class UserEndpointService
{
	private readonly UserRepository _userRepository;
	private readonly RequestService _requestService;

	public UserEndpointService(UserRepository userRepository, RequestService requestService)
	{
		_userRepository = userRepository;
		_requestService = requestService;
	}

	public async Task CreateUser(int userId, long chatId, string userName)
	{
		var existingUser = await GetUser(userId, chatId);
		if (existingUser == null)
		{
			await _userRepository.CreateDefaultUser(userId, userName, chatId);
		}

		await _requestService.SendText(
			chatId,
			$"Hello {userName}!\nYou can send me links to UA Ikea pages and I'll periodically check and notify you, if those items are available.");
	}

	public async Task AddUserEndpoint(int userId, long chatId, int messageId, string text)
	{
		var result = "Message is not a valid http endpoint";

		if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
		{
			var user = await GetUser(userId, chatId);
			user.PolledPages.Add(text);

			await _userRepository.PushUserAsync(user);

			result = "Endpoint was successfully saved!";
		}

		await _requestService.ReplyText(chatId, messageId, result);
	}

	public async Task ViewUserEndpoints(int userId, long chatId)
	{
		var message = "No pages saved!";
		var user = await GetUser(userId, chatId);
		if (user.PolledPages.Count != 0)
		{
			var stringBuilder = new StringBuilder("Saved items:\n");
			for (var i = 0; i < user.PolledPages.Count; i++)
			{
				stringBuilder
					.Append(i + 1)
					.Append(". ")
					.Append(user.PolledPages[i])
					.Append('\n');
			}

			message = stringBuilder.ToString();
		}

		await _requestService.SendText(chatId, message);
	}

	public async Task RemoveEndpoint(int userId, long chatId, int position)
	{
		var message = $"Position {position} is not in the list";
		var user = await GetUser(userId, chatId);

		if (position <= user.PolledPages.Count || position > 0)
		{
			var removedEndpoint = user.PolledPages[position - 1];
			user.PolledPages.RemoveAt(position - 1);

			await _userRepository.PushUserAsync(user);

			message = $"Endpoint {removedEndpoint} was removed.";
		}

		await _requestService.SendText(chatId, message);
	}

	private async Task<UserDto> GetUser(int userId, long chatId) => await _userRepository.GetUser(userId, chatId);
}