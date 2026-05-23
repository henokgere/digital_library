using Microsoft.AspNetCore.Mvc;
using digital_library.Data;
using digital_library.Models;
using Microsoft.EntityFrameworkCore;

namespace digital_library.Controllers
{
    public class BookController : Controller
    {
        private readonly digital_libraryContext _context;

        public BookController(digital_libraryContext context)
        {
            _context = context;
        }

        // GET: Book
        public IActionResult Index()
        {
            return View(_context.Book.OrderBy(book => book.Title).ToList());
        }

        // GET: Book/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Book.FirstOrDefault(m => m.Id == id);
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
        public IActionResult AddBook([Bind("Title,Author,Genre,PublicationDate,CoverUrl")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.AvailabilityStatus = AvailabilityStatus.Available;
                _context.Book.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // POST: Book/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow(int id, string borrowUser)
        {
            var book = _context.Book.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null || book.AvailabilityStatus != AvailabilityStatus.Available)
            {
                return NotFound();
            }
            book.AvailabilityStatus = AvailabilityStatus.Borrowed;
            book.BorrowedBy = borrowUser;
            book.BorrowedDate = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: Book/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(int id)
        {
            var book = _context.Book.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null || book.AvailabilityStatus != AvailabilityStatus.Borrowed)
            {
                return NotFound();
            }
            book.AvailabilityStatus = AvailabilityStatus.Available;
            book.BorrowedBy = null;
            book.BorrowedDate = null;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Book.FirstOrDefault(candidate => candidate.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Author,Genre,PublicationDate,AvailabilityStatus,CoverUrl")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingBook = _context.Book.FirstOrDefault(candidate => candidate.Id == id);
                if (existingBook == null)
                {
                    return NotFound();
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Genre = book.Genre;
                existingBook.PublicationDate = book.PublicationDate;
                existingBook.AvailabilityStatus = book.AvailabilityStatus;
                existingBook.CoverUrl = book.CoverUrl;
                _context.SaveChanges();

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
            var book = _context.Book.FirstOrDefault(m => m.Id == id);
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
            var book = _context.Book.FirstOrDefault(candidate => candidate.Id == id);
            if (book != null)
            {
                _context.Book.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
