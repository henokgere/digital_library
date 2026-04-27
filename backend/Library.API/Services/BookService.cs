using Library.API.Models;
using Library.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Library.API.Services;

public class BookService
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BookService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _booksCollection = mongoDatabase.GetCollection<Book>(mongoDbSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(book => book.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(book => book.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(book => book.Id == id);

    public async Task<bool> AnyAsync() =>
        await _booksCollection.Find(_ => true).AnyAsync();
}
