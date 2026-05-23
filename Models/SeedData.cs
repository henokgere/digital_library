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

        // Idempotent seed: every startup, ensure each of the 15 catalog books
        // exists in the DB (matched by Title). Missing ones are inserted; existing
        // ones — including books the user has borrowed/edited — are left alone.
        // This guarantees that a DB wipe (Docker container restart with no volume,
        // a manual delete of digital_library.db, etc.) leaves the catalog populated.
        var existingTitles = context.Book.Select(b => b.Title).ToHashSet();

        var toAdd = Catalog.Where(b => !existingTitles.Contains(b.Title)).ToList();
        if (toAdd.Count > 0)
        {
            context.Book.AddRange(toAdd);
            context.SaveChanges();
        }
    }

    // Cover URLs use Open Library's public cover service:
    //   https://covers.openlibrary.org/b/isbn/{ISBN}-L.jpg
    // No API key required, served over HTTPS, stable as long as the ISBN exists.
    private static readonly Book[] Catalog =
    [
        new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt and David Thomas",
            Genre = "Software Engineering",
            PublicationDate = new DateTime(1999, 10, 30),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201616224-L.jpg"
        },
        new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Genre = "Programming",
            PublicationDate = new DateTime(2008, 8, 1),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg"
        },
        new Book
        {
            Title = "The C Programming Language",
            Author = "Brian W. Kernighan and Dennis M. Ritchie",
            Genre = "Programming",
            PublicationDate = new DateTime(1988, 4, 1),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780131103627-L.jpg"
        },
        new Book
        {
            Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
            Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
            Genre = "Software Architecture",
            PublicationDate = new DateTime(1994, 10, 21),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201633610-L.jpg"
        },
        new Book
        {
            Title = "Introduction to Algorithms",
            Author = "Thomas H. Cormen, Charles E. Leiserson, Ronald L. Rivest, Clifford Stein",
            Genre = "Computer Science",
            PublicationDate = new DateTime(2009, 7, 31),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780262033848-L.jpg"
        },
        new Book
        {
            Title = "Code Complete",
            Author = "Steve McConnell",
            Genre = "Software Engineering",
            PublicationDate = new DateTime(2004, 6, 9),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780735619678-L.jpg"
        },
        new Book
        {
            Title = "Refactoring: Improving the Design of Existing Code",
            Author = "Martin Fowler",
            Genre = "Programming",
            PublicationDate = new DateTime(1999, 7, 8),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201485677-L.jpg"
        },
        new Book
        {
            Title = "The Mythical Man-Month",
            Author = "Frederick P. Brooks Jr.",
            Genre = "Software Engineering",
            PublicationDate = new DateTime(1995, 8, 12),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201835953-L.jpg"
        },
        new Book
        {
            Title = "Cracking the Coding Interview",
            Author = "Gayle Laakmann McDowell",
            Genre = "Career",
            PublicationDate = new DateTime(2015, 7, 1),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780984782857-L.jpg"
        },
        new Book
        {
            Title = "Structure and Interpretation of Computer Programs",
            Author = "Harold Abelson and Gerald Jay Sussman",
            Genre = "Computer Science",
            PublicationDate = new DateTime(1996, 7, 25),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780262510875-L.jpg"
        },
        new Book
        {
            Title = "You Don't Know JS: Up & Going",
            Author = "Kyle Simpson",
            Genre = "JavaScript",
            PublicationDate = new DateTime(2015, 3, 27),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9781491924464-L.jpg"
        },
        new Book
        {
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Genre = "Software Architecture",
            PublicationDate = new DateTime(2003, 8, 22),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780321125217-L.jpg"
        },
        new Book
        {
            Title = "The Art of Computer Programming, Volume 1",
            Author = "Donald E. Knuth",
            Genre = "Computer Science",
            PublicationDate = new DateTime(1997, 7, 17),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201896831-L.jpg"
        },
        new Book
        {
            Title = "Eloquent JavaScript",
            Author = "Marijn Haverbeke",
            Genre = "JavaScript",
            PublicationDate = new DateTime(2018, 12, 4),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9781593279509-L.jpg"
        },
        new Book
        {
            Title = "Head First Design Patterns",
            Author = "Eric Freeman and Elisabeth Robson",
            Genre = "Software Architecture",
            PublicationDate = new DateTime(2004, 10, 25),
            AvailabilityStatus = AvailabilityStatus.Available,
            CoverUrl = "https://covers.openlibrary.org/b/isbn/9780596007126-L.jpg"
        }
    ];
}
