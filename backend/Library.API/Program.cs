var builder = WebApplication.CreateBuilder(args);

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

var users = new List<User>
{
    new User { Username = "admin", Password = "password" }
};
var tokens = new Dictionary<string, string>();

var books = new List<Book>
{
    new Book { Id = 1, Title = "The Hobbit", Author = "J.R.R. Tolkien", Status = "available" },
    new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", Status = "available" }
};
var nextId = 3;

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

app.MapPost("/auth/register", (UserInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
        return Results.BadRequest("Username and password are required.");

    if (users.Any(u => u.Username.Equals(input.Username, StringComparison.OrdinalIgnoreCase)))
        return Results.Conflict("Username already exists.");

    var user = new User { Username = input.Username!, Password = input.Password! };
    users.Add(user);

    var token = Guid.NewGuid().ToString();
    tokens[token] = user.Username;

    return Results.Ok(new AuthResponse { Token = token, Username = user.Username });
});

app.MapPost("/auth/login", (UserInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
        return Results.BadRequest("Username and password are required.");

    var user = users.FirstOrDefault(u => u.Username.Equals(input.Username, StringComparison.OrdinalIgnoreCase) && u.Password == input.Password);
    if (user is null)
        return Results.Unauthorized();

    var token = Guid.NewGuid().ToString();
    tokens[token] = user.Username;

    return Results.Ok(new AuthResponse { Token = token, Username = user.Username });
});

app.MapGet("/auth/me", (HttpRequest request) =>
{
    if (!TryGetUsername(request, out var username))
        return Results.Unauthorized();

    return Results.Ok(new { Username = username });
});

app.MapGet("/books", (HttpRequest request) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    return Results.Ok(books);
});

app.MapGet("/books/{id:int}", (HttpRequest request, int id) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    var book = books.FirstOrDefault(b => b.Id == id);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapPost("/books", (HttpRequest request, BookInput input) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Author))
        return Results.BadRequest("Title and Author are required.");

    var book = new Book { Id = nextId++, Title = input.Title, Author = input.Author, Status = "available" };
    books.Add(book);
    return Results.Created($"/books/{book.Id}", book);
});

app.MapPut("/books/{id:int}", (HttpRequest request, int id, BookInput input) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book is null)
        return Results.NotFound();

    if (!string.IsNullOrWhiteSpace(input.Title))
        book.Title = input.Title;
    if (!string.IsNullOrWhiteSpace(input.Author))
        book.Author = input.Author;

    return Results.Ok(book);
});

app.MapDelete("/books/{id:int}", (HttpRequest request, int id) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book is null)
        return Results.NotFound();

    books.Remove(book);
    return Results.NoContent();
});

app.MapPost("/books/{id:int}/borrow", (HttpRequest request, int id, BorrowInput input) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book is null)
        return Results.NotFound();

    if (book.Status != "available")
        return Results.BadRequest("Book is not available for borrowing.");

    if (string.IsNullOrWhiteSpace(input.User))
        return Results.BadRequest("User is required.");

    book.Status = "borrowed";
    book.BorrowedBy = input.User;
    book.BorrowedDate = DateTime.UtcNow;

    return Results.Ok(book);
});

app.MapPost("/books/{id:int}/return", (HttpRequest request, int id) =>
{
    if (!TryGetUsername(request, out _))
        return Results.Unauthorized();

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book is null)
        return Results.NotFound();

    if (book.Status != "borrowed")
        return Results.BadRequest("Book is not borrowed.");

    book.Status = "available";
    book.BorrowedBy = null;
    book.BorrowedDate = null;

    return Results.Ok(book);
});

app.Run();

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Status { get; set; } = "available";
    public string? BorrowedBy { get; set; }
    public DateTime? BorrowedDate { get; set; }
}

public class BookInput
{
    public string? Title { get; set; }
    public string? Author { get; set; }
}

public class BorrowInput
{
    public string? User { get; set; }
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserInput
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
