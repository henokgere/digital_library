using Library.API.Models;
using Library.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Library.API.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UsersCollectionName);
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _usersCollection
            .Find(Builders<User>.Filter.Regex(
                user => user.Username,
                new MongoDB.Bson.BsonRegularExpression($"^{Regex.Escape(username.Trim())}$", "i")))
            .FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task<bool> AnyAsync() =>
        await _usersCollection.Find(_ => true).AnyAsync();
}
