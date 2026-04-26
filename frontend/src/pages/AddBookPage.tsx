import type { RoutePath } from '../types'

export function AddBookPage({
  newBook,
  setNewBook,
  addBook,
  onNavigate,
}: {
  newBook: { title: string; author: string }
  setNewBook: (value: { title: string; author: string }) => void
  addBook: () => Promise<void>
  onNavigate: (path: RoutePath) => void
}) {
  return (
    <section className="rounded-3xl bg-white p-6 shadow-sm">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end">
        <div className="flex-1">
          <h2 className="text-xl font-semibold text-slate-900">Create a new book</h2>
          <p className="mt-1 text-sm text-slate-600">
            Add title and author, then save it to the collection.
          </p>
        </div>
        <div className="grid w-full gap-3 sm:grid-cols-2 lg:grid-cols-[1fr_1fr_auto_auto] lg:w-auto">
          <input
            value={newBook.title}
            onChange={(e) => setNewBook({ ...newBook, title: e.target.value })}
            placeholder="Book title"
            className="rounded-2xl border border-slate-200 px-4 py-3 focus:border-blue-500 focus:outline-none"
          />
          <input
            value={newBook.author}
            onChange={(e) => setNewBook({ ...newBook, author: e.target.value })}
            placeholder="Author"
            className="rounded-2xl border border-slate-200 px-4 py-3 focus:border-blue-500 focus:outline-none"
          />
          <button
            onClick={addBook}
            className="rounded-2xl bg-blue-600 px-5 py-3 text-white transition hover:bg-blue-700"
          >
            Save Book
          </button>
          <button
            onClick={() => onNavigate('/books')}
            className="rounded-2xl border border-slate-200 px-5 py-3 text-slate-700 transition hover:bg-slate-100"
          >
            View Catalog
          </button>
        </div>
      </div>
    </section>
  )
}
