using IkeaNotifier.Services.Repositories;
using System;
using System.Threading.Tasks;

namespace IkeaNotifier.Services;

public class UserEndpointService
{
	private readonly UserRepository _userRepository;

	public UserEndpointService(UserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<string> AddUserEndpoint(int userId, string userName, long chatId, string message)
	{
		var result = "Message is not a valid http endpoint";

		if (Uri.IsWellFormedUriString(message, UriKind.Absolute))
		{
			var user = await _userRepository.GetUser(userId, chatId) ?? await _userRepository.CreateDefaultUser(userId, userName, chatId);
			user.PolledPages.Add(message);

			await _userRepository.PushUserAsync(user);

			result = "Endpoint was successfully saved!";
		}

		return result;

	}
}