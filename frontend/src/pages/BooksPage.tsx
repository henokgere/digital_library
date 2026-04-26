import type { Book } from '../types'

export function BooksPage({
  books,
  editing,
  borrowUser,
  setBorrowUser,
  setEditing,
  updateBook,
  deleteBook,
  borrowBook,
  returnBook,
}: {
  books: Book[]
  editing: Book | null
  borrowUser: string
  setBorrowUser: (value: string) => void
  setEditing: (value: Book | null) => void
  updateBook: (id: number, title: string, author: string) => Promise<void>
  deleteBook: (id: number) => Promise<void>
  borrowBook: (id: number, user: string) => Promise<void>
  returnBook: (id: number) => Promise<void>
}) {
  return (
    <div className="library-grid">
      {books.length === 0 && (
        <section className="library-empty-state">
          <h2 className="library-empty-state__title">No books yet</h2>
          <p className="library-empty-state__text">
            Start the collection from the add book page and the catalog will fill in here.
          </p>
        </section>
      )}

      {books.map((book) => (
        <article key={book.id} className="book-card">
          {editing?.id === book.id ? (
            <div className="book-card__edit-grid">
              <input
                type="text"
                value={editing.title}
                onChange={(e) => setEditing({ ...editing, title: e.target.value })}
                className="library-input"
              />
              <input
                type="text"
                value={editing.author}
                onChange={(e) => setEditing({ ...editing, author: e.target.value })}
                className="library-input"
              />
              <div className="book-card__button-row">
                <button
                  onClick={() => updateBook(book.id, editing.title, editing.author)}
                  className="library-button library-button--blue"
                >
                  Save
                </button>
                <button
                  onClick={() => setEditing(null)}
                  className="library-button library-button--ghost"
                >
                  Cancel
                </button>
              </div>
            </div>
          ) : (
            <div className="book-card__content">
              <div className="book-card__meta">
                <div className="book-card__chip">Book</div>
                <h2 className="book-card__title">{book.title}</h2>
                <p className="book-card__author">by {book.author}</p>
                <p
                  className={`book-card__status ${
                    book.status === 'available'
                      ? 'book-card__status--available'
                      : 'book-card__status--borrowed'
                  }`}
                >
                  {book.status === 'available' ? 'Available' : 'Borrowed'}
                </p>
                {book.status === 'borrowed' && book.borrowedBy && (
                  <p className="book-card__borrowed-note">
                    Borrowed by {book.borrowedBy} on{' '}
                    {new Date(book.borrowedDate ?? '').toLocaleDateString()}
                  </p>
                )}
              </div>

              <div className="book-card__controls">
                {book.status === 'available' ? (
                  <>
                    <input
                      type="text"
                      placeholder="Borrower name"
                      value={borrowUser}
                      onChange={(e) => setBorrowUser(e.target.value)}
                      className="library-input"
                    />
                    <button
                      onClick={() => borrowBook(book.id, borrowUser)}
                      className="library-button library-button--blue"
                    >
                      Borrow
                    </button>
                  </>
                ) : (
                  <button
                    onClick={() => returnBook(book.id)}
                    className="library-button library-button--blue"
                  >
                    Return
                  </button>
                )}
                <div className="book-card__button-row">
                  <button
                    onClick={() => setEditing(book)}
                    className="library-button library-button--cream"
                    disabled={book.status === 'borrowed'}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => deleteBook(book.id)}
                    className="library-button library-button--ink"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
          )}
        </article>
      ))}
    </div>
  )
}
