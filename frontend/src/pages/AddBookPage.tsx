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
    <section className="library-panel">
      <div className="library-panel__row">
        <div className="library-panel__copy">
          <div className="library-panel__eyebrow">Collection update</div>
          <h2 className="library-panel__title">Create a new book</h2>
          <p className="library-panel__description">
            Add title and author, then save it to the collection.
          </p>
        </div>
        <div className="library-panel__form">
          <input
            value={newBook.title}
            onChange={(e) => setNewBook({ ...newBook, title: e.target.value })}
            placeholder="Book title"
            className="library-input"
          />
          <input
            value={newBook.author}
            onChange={(e) => setNewBook({ ...newBook, author: e.target.value })}
            placeholder="Author"
            className="library-input"
          />
          <button
            onClick={addBook}
            className="library-button library-button--blue"
          >
            Save Book
          </button>
          <button
            onClick={() => onNavigate('/books')}
            className="library-button library-button--ghost"
          >
            View Catalog
          </button>
        </div>
      </div>
    </section>
  )
}
