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
    <div className="grid gap-4">
      {books.map((book) => (
        <article key={book.id} className="rounded-3xl bg-white p-6 shadow-sm">
          {editing?.id === book.id ? (
            <div className="grid gap-3 md:grid-cols-[1fr_1fr_auto]">
              <input
                type="text"
                value={editing.title}
                onChange={(e) => setEditing({ ...editing, title: e.target.value })}
                className="rounded-2xl border border-slate-200 px-4 py-3 focus:border-blue-500 focus:outline-none"
              />
              <input
                type="text"
                value={editing.author}
                onChange={(e) => setEditing({ ...editing, author: e.target.value })}
                className="rounded-2xl border border-slate-200 px-4 py-3 focus:border-blue-500 focus:outline-none"
              />
              <div className="flex flex-wrap gap-3">
                <button
                  onClick={() => updateBook(book.id, editing.title, editing.author)}
                  className="rounded-2xl bg-green-600 px-4 py-2 text-white transition hover:bg-green-700"
                >
                  Save
                </button>
                <button
                  onClick={() => setEditing(null)}
                  className="rounded-2xl border border-slate-200 px-4 py-2 text-slate-700 transition hover:bg-slate-100"
                >
                  Cancel
                </button>
              </div>
            </div>
          ) : (
            <div className="grid gap-4 lg:grid-cols-[1fr_auto] lg:items-start">
              <div>
                <h2 className="text-xl font-semibold text-slate-900">{book.title}</h2>
                <p className="mt-1 text-slate-600">by {book.author}</p>
                <p
                  className={`mt-3 text-sm font-semibold ${
                    book.status === 'available' ? 'text-green-600' : 'text-red-600'
                  }`}
                >
                  {book.status === 'available' ? 'Available' : 'Borrowed'}
                </p>
                {book.status === 'borrowed' && book.borrowedBy && (
                  <p className="mt-1 text-sm text-slate-500">
                    Borrowed by {book.borrowedBy} on{' '}
                    {new Date(book.borrowedDate ?? '').toLocaleDateString()}
                  </p>
                )}
              </div>

              <div className="grid gap-3 sm:grid-cols-[1fr_auto]">
                {book.status === 'available' ? (
                  <>
                    <input
                      type="text"
                      placeholder="Borrower name"
                      value={borrowUser}
                      onChange={(e) => setBorrowUser(e.target.value)}
                      className="rounded-2xl border border-slate-200 px-4 py-3 focus:border-blue-500 focus:outline-none"
                    />
                    <button
                      onClick={() => borrowBook(book.id, borrowUser)}
                      className="rounded-2xl bg-blue-600 px-4 py-3 text-white transition hover:bg-blue-700"
                    >
                      Borrow
                    </button>
                  </>
                ) : (
                  <button
                    onClick={() => returnBook(book.id)}
                    className="rounded-2xl bg-green-600 px-4 py-3 text-white transition hover:bg-green-700"
                  >
                    Return
                  </button>
                )}
                <div className="flex flex-wrap gap-3">
                  <button
                    onClick={() => setEditing(book)}
                    className="rounded-2xl bg-yellow-500 px-4 py-3 text-white transition hover:bg-yellow-600"
                    disabled={book.status === 'borrowed'}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => deleteBook(book.id)}
                    className="rounded-2xl bg-red-500 px-4 py-3 text-white transition hover:bg-red-600"
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
