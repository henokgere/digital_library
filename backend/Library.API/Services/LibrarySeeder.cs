using Library.API.Models;

namespace Library.API.Services;

public class LibrarySeeder
{
    private readonly BookService _bookService;
    private readonly UserService _userService;
    private readonly PasswordService _passwordService;

    public LibrarySeeder(BookService bookService, UserService userService, PasswordService passwordService)
    {
        _bookService = bookService;
        _userService = userService;
        _passwordService = passwordService;
    }

    public async Task SeedAsync()
    {
        if (!await _userService.AnyAsync())
        {
            await _userService.CreateAsync(new User
            {
                Username = "admin",
                PasswordHash = _passwordService.HashPassword("password")
            });
        }

        if (!await _bookService.AnyAsync())
        {
            await _bookService.CreateAsync(new Book
            {
                Title = "The Hobbit",
                Author = "J.R.R. Tolkien",
                Status = "available"
            });

            await _bookService.CreateAsync(new Book
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Status = "available"
            });
        }
    }
}
