using digital_library.Models;
using Microsoft.EntityFrameworkCore;

namespace digital_library.Data;

public class digital_libraryContext(DbContextOptions<digital_libraryContext> options) : DbContext(options)
{
    public DbSet<Book> Book { get; set; } = default!;
    public DbSet<User> User { get; set; } = default!;
}
