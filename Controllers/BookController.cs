using Microsoft.AspNetCore.Mvc;
using digital_library.Models;

namespace digital_library.Controllers
{
    public class BookController : Controller
    {
        private static readonly List<Book> Books =
        [
            new Book
            {
                Id = 1,
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt and David Thomas",
                Genre = "Software Engineering",
                PublicationDate = new DateTime(1999, 10, 30),
                AvailabilityStatus = AvailabilityStatus.Available
            },
            new Book
            {
                Id = 2,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Genre = "Programming",
                PublicationDate = new DateTime(2008, 8, 1),
                AvailabilityStatus = AvailabilityStatus.Borrowed,
                BorrowedBy = "Alem",
                BorrowedDate = DateTime.Today.AddDays(-3)
            }
        ];

        private static int _nextId = Books.Max(book => book.Id) + 1;

        // GET: Book
        public IActionResult Index()
        {
            return View(Books.OrderBy(book => book.Title).ToList());
        }

        // GET: Book/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = Books.FirstOrDefault(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // GET: Book/AddBook
        public IActionResult AddBook()
        {
            return View();
        }

        // POST: Book/AddBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBook([Bind("Title,Author,Genre,PublicationDate")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.Id = _nextId++;
                book.AvailabilityStatus = AvailabilityStatus.Available;
                Books.Add(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // POST: Book/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow(int id, string borrowUser)
        {
            var book = Books.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null || book.AvailabilityStatus != AvailabilityStatus.Available)
            {
                return NotFound();
            }
            book.AvailabilityStatus = AvailabilityStatus.Borrowed;
            book.BorrowedBy = borrowUser;
            book.BorrowedDate = DateTime.Now;
            return RedirectToAction(nameof(Index));
        }

        // POST: Book/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(int id)
        {
            var book = Books.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null || book.AvailabilityStatus != AvailabilityStatus.Borrowed)
            {
                return NotFound();
            }
            book.AvailabilityStatus = AvailabilityStatus.Available;
            book.BorrowedBy = null;
            book.BorrowedDate = null;
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = Books.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Author,Genre,PublicationDate,AvailabilityStatus")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingBook = Books.FirstOrDefault(candidate => candidate.Id == id);
                if (existingBook == null)
                {
                    return NotFound();
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Genre = book.Genre;
                existingBook.PublicationDate = book.PublicationDate;
                existingBook.AvailabilityStatus = book.AvailabilityStatus;

                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = Books.FirstOrDefault(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = Books.FirstOrDefault(candidate => candidate.Id == id);
            if (book != null)
            {
                Books.Remove(book);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return Books.Any(e => e.Id == id);
        }
    }
}
