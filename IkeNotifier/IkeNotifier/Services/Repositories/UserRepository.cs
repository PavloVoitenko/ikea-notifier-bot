using IkeaNotifier.Models.Mongo;
using IkeaNotifier.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IkeaNotifier.Services.Repositories;

public class UserRepository
{
	private readonly IMongoCollection<UserDto> _userCollection;

	public UserRepository(IOptions<MongoOptions> options, MongoClient client)
	{
		var mongoOptions = options.Value;

		var database = client.GetDatabase(mongoOptions.DatabaseName);

		_userCollection = database.GetCollection<UserDto>(mongoOptions.UserCollectionName);
	}

	public async Task<IEnumerable<UserDto>> GetUsers() => await _userCollection.Find(Builders<UserDto>.Filter.Empty).ToListAsync();

	public async Task<UserDto> GetUser(int userId, long chatId)
	{
		var userFilter = CreateUserChatFilter(userId, chatId);

		return await _userCollection.Find(userFilter).SingleOrDefaultAsync();
	}

	public async Task<UserDto> CreateDefaultUser(int userId, string username, long chatId) => await PushUserAsync(new UserDto
	{
		UserId = userId,
		ChatId = chatId,
		Username = username,
		PolledPages = new List<string>()
	});

	public async Task<UserDto> PushUserAsync(UserDto user)
	{
		var userFilter = CreateUserChatFilter(user.UserId, user.ChatId);
		var replaceOptions = new ReplaceOptions { IsUpsert = true };

		await _userCollection.ReplaceOneAsync(userFilter, user, replaceOptions);

		return user;
	}

	#region Filters

	private static FilterDefinition<UserDto> CreateUserChatFilter(int userId, long chatId)
	{
		var userFilter = CreateUserFilter(userId);
		var chatFilter = CreateChatFilter(chatId);

		return Builders<UserDto>.Filter.And(userFilter, chatFilter);
	}

	private static FilterDefinition<UserDto> CreateUserFilter(int userId) => Builders<UserDto>.Filter.Eq(u => u.UserId, userId);

	private static FilterDefinition<UserDto> CreateChatFilter(long chatId) => Builders<UserDto>.Filter.Eq(u => u.ChatId, chatId);

	#endregion
}