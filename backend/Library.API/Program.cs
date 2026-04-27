using Library.API.Models;
using Library.API.Services;
using Library.API.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<LibrarySeeder>();
builder.Services.AddSingleton<PasswordService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();

var tokens = new Dictionary<string, string>(StringComparer.Ordinal);

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<LibrarySeeder>();
    await seeder.SeedAsync();
}

bool TryGetUsername(HttpRequest request, out string? username)
{
    username = null;
    if (!request.Headers.TryGetValue("Authorization", out var headerValues))
    {
        return false;
    }

    var header = headerValues.ToString();
    if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    var token = header[7..].Trim();
    return tokens.TryGetValue(token, out username);
}

app.MapGet("/", () => "Library API is running. Use /auth/login or /auth/register and /books endpoint.");

app.MapPost("/auth/register", async (UserInput input, UserService userService, PasswordService passwordService) =>
{
    if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
    {
        return Results.BadRequest("Username and password are required.");
    }

    var existingUser = await userService.GetByUsernameAsync(input.Username);
    if (existingUser is not null)
    {
        return Results.Conflict("Username already exists.");
    }

    var user = new User
    {
        Username = input.Username.Trim(),
        PasswordHash = passwordService.HashPassword(input.Password)
    };

    await userService.CreateAsync(user);

    var token = Guid.NewGuid().ToString();
    tokens[token] = user.Username;

    return Results.Ok(new AuthResponse { Token = token, Username = user.Username });
});

app.MapPost("/auth/login", async (UserInput input, UserService userService, PasswordService passwordService) =>
{
    if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
    {
        return Results.BadRequest("Username and password are required.");
    }

    var user = await userService.GetByUsernameAsync(input.Username);
    if (user is null || !passwordService.VerifyPassword(user, input.Password))
    {
        return Results.Unauthorized();
    }

    var token = Guid.NewGuid().ToString();
    tokens[token] = user.Username;

    return Results.Ok(new AuthResponse { Token = token, Username = user.Username });
});

app.MapGet("/auth/me", async (HttpRequest request, UserService userService) =>
{
    if (!TryGetUsername(request, out var username) || string.IsNullOrWhiteSpace(username))
    {
        return Results.Unauthorized();
    }

    var user = await userService.GetByUsernameAsync(username);
    return user is null ? Results.Unauthorized() : Results.Ok(new { Username = user.Username });
});

app.MapGet("/books", async (HttpRequest request, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var books = await bookService.GetAsync();
    return Results.Ok(books);
});

app.MapGet("/books/{id}", async (HttpRequest request, string id, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var book = await bookService.GetAsync(id);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapPost("/books", async (HttpRequest request, BookInput input, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Author))
    {
        return Results.BadRequest("Title and Author are required.");
    }

    var book = new Book
    {
        Title = input.Title.Trim(),
        Author = input.Author.Trim(),
        Status = "available"
    };

    await bookService.CreateAsync(book);
    return Results.Created($"/books/{book.Id}", book);
});

app.MapPut("/books/{id}", async (HttpRequest request, string id, BookInput input, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var book = await bookService.GetAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrWhiteSpace(input.Title))
    {
        book.Title = input.Title.Trim();
    }

    if (!string.IsNullOrWhiteSpace(input.Author))
    {
        book.Author = input.Author.Trim();
    }

    await bookService.UpdateAsync(id, book);
    return Results.Ok(book);
});

app.MapDelete("/books/{id}", async (HttpRequest request, string id, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var book = await bookService.GetAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    await bookService.RemoveAsync(id);
    return Results.NoContent();
});

app.MapPost("/books/{id}/borrow", async (HttpRequest request, string id, BorrowInput input, BookService bookService, UserService userService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var book = await bookService.GetAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    if (book.Status != "available")
    {
        return Results.BadRequest("Book is not available for borrowing.");
    }

    if (string.IsNullOrWhiteSpace(input.User))
    {
        return Results.BadRequest("User is required.");
    }

    var borrower = await userService.GetByUsernameAsync(input.User);
    if (borrower is null)
    {
        return Results.BadRequest("Borrowing user does not exist.");
    }

    book.Status = "borrowed";
    book.BorrowedBy = borrower.Username;
    book.BorrowedDate = DateTime.UtcNow;

    await bookService.UpdateAsync(id, book);
    return Results.Ok(book);
});

app.MapPost("/books/{id}/return", async (HttpRequest request, string id, BookService bookService) =>
{
    if (!TryGetUsername(request, out _))
    {
        return Results.Unauthorized();
    }

    var book = await bookService.GetAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    if (book.Status != "borrowed")
    {
        return Results.BadRequest("Book is not borrowed.");
    }

    book.Status = "available";
    book.BorrowedBy = null;
    book.BorrowedDate = null;

    await bookService.UpdateAsync(id, book);
    return Results.Ok(book);
});

app.Run();
