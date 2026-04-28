using digital_library.Data;
using Microsoft.EntityFrameworkCore;

namespace digital_library.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new digital_libraryContext(
            serviceProvider.GetRequiredService<DbContextOptions<digital_libraryContext>>());

        context.Database.Migrate();

        if (context.Book.Any())
        {
            return;
        }

        context.Book.AddRange(
            new Book
            {
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt and David Thomas",
                Genre = "Software Engineering",
                PublicationDate = new DateTime(1999, 10, 30),
                AvailabilityStatus = AvailabilityStatus.Available
            },
            new Book
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Genre = "Programming",
                PublicationDate = new DateTime(2008, 8, 1),
                AvailabilityStatus = AvailabilityStatus.Borrowed,
                BorrowedBy = "Alem",
                BorrowedDate = DateTime.Today.AddDays(-3)
            });

        context.SaveChanges();
    }
}
