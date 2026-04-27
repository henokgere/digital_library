import { useEffect, useState } from 'react'
import './App.css'
import { AppShell } from './components/AppShell'
import { AddBookPage } from './pages/AddBookPage'
import { AuthPage } from './pages/AuthPage'
import { BooksPage } from './pages/BooksPage'
import type { AuthForm, AuthInfo, AuthMode, Book, RoutePath } from './types'

const API_BASE = 'http://localhost:5285'

const protectedRoutes: RoutePath[] = ['/books', '/books/new']

const getRouteFromPath = (pathname: string): RoutePath => {
  if (pathname === '/register') return '/register'
  if (pathname === '/books') return '/books'
  if (pathname === '/books/new') return '/books/new'
  return '/login'
}

function App() {
  const [auth, setAuth] = useState<AuthInfo | null>(() => {
    const saved = localStorage.getItem('auth')
    return saved ? (JSON.parse(saved) as AuthInfo) : null
  })
  const [currentPath, setCurrentPath] = useState<RoutePath>(() =>
    getRouteFromPath(window.location.pathname),
  )
  const [authForm, setAuthForm] = useState<AuthForm>({ username: '', password: '' })
  const [books, setBooks] = useState<Book[]>([])
  const [loading, setLoading] = useState(false)
  const [editing, setEditing] = useState<Book | null>(null)
  const [newBook, setNewBook] = useState({ title: '', author: '' })
  const [borrowUser, setBorrowUser] = useState('')

  const authHeaders = auth ? { Authorization: `Bearer ${auth.token}` } : undefined

  const navigate = (path: RoutePath, replace = false) => {
    const method = replace ? 'replaceState' : 'pushState'
    window.history[method](null, '', path)
    setCurrentPath(path)
  }

  const saveAuth = (value: AuthInfo | null) => {
    if (value) {
      localStorage.setItem('auth', JSON.stringify(value))
    } else {
      localStorage.removeItem('auth')
    }
    setAuth(value)
  }

  const fetchBooks = async () => {
    if (!auth) return
    setLoading(true)
    try {
      const response = await fetch(`${API_BASE}/books`, {
        headers: { ...authHeaders },
      })
      if (response.ok) {
        const data = await response.json()
        setBooks(data)
      } else if (response.status === 401) {
        saveAuth(null)
      }
    } catch (error) {
      console.error('Error fetching books:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const syncRoute = () => {
      setCurrentPath(getRouteFromPath(window.location.pathname))
    }

    window.addEventListener('popstate', syncRoute)
    return () => window.removeEventListener('popstate', syncRoute)
  }, [])

  useEffect(() => {
    if (!auth && protectedRoutes.includes(currentPath)) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      navigate('/login', true)
      return
    }

    if (auth && (currentPath === '/login' || currentPath === '/register')) {
      navigate('/books', true)
    }
  }, [auth, currentPath])

  useEffect(() => {
    if (auth) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      fetchBooks()
    }
  }, [auth])

  const requestAuth = async (mode: AuthMode) => {
    if (!authForm.username || !authForm.password) {
      alert('Username and password are required.')
      return
    }

    try {
      const response = await fetch(`${API_BASE}/auth/${mode}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(authForm),
      })

      if (response.ok) {
        const data = await response.json()
        saveAuth(data)
        setAuthForm({ username: '', password: '' })
        navigate('/books', true)
      } else if (response.status === 409) {
        alert('Username already exists.')
      } else {
        alert('Invalid credentials.')
      }
    } catch (error) {
      console.error('Authentication error:', error)
    }
  }

  const logout = () => {
    saveAuth(null)
    setBooks([])
    setEditing(null)
    navigate('/login', true)
  }

  const fetchWithAuth = async (input: RequestInfo, init: RequestInit = {}) => {
    const headers: Record<string, string> = {
      ...((init.headers as Record<string, string>) ?? {}),
      ...(authHeaders ?? {}),
    }
    return fetch(input, { ...init, headers })
  }

  const addBook = async () => {
    if (!newBook.title || !newBook.author) return
    try {
      const response = await fetchWithAuth(`${API_BASE}/books`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newBook),
      })
      if (response.ok) {
        setNewBook({ title: '', author: '' })
        await fetchBooks()
        navigate('/books')
      }
    } catch (error) {
      console.error('Error adding book:', error)
    }
  }

  const updateBook = async (id: string, title: string, author: string) => {
    try {
      const response = await fetchWithAuth(`${API_BASE}/books/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, author }),
      })
      if (response.ok) {
        setEditing(null)
        await fetchBooks()
      }
    } catch (error) {
      console.error('Error updating book:', error)
    }
  }

  const deleteBook = async (id: string) => {
    try {
      const response = await fetchWithAuth(`${API_BASE}/books/${id}`, {
        method: 'DELETE',
      })
      if (response.ok) {
        await fetchBooks()
      }
    } catch (error) {
      console.error('Error deleting book:', error)
    }
  }

  const borrowBook = async (id: string, user: string) => {
    try {
      const borrower = user || auth?.username || ''
      const response = await fetchWithAuth(`${API_BASE}/books/${id}/borrow`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ user: borrower }),
      })
      if (response.ok) {
        setBorrowUser('')
        await fetchBooks()
      } else {
        alert('Failed to borrow book')
      }
    } catch (error) {
      console.error('Error borrowing book:', error)
    }
  }

  const returnBook = async (id: string) => {
    try {
      const response = await fetchWithAuth(`${API_BASE}/books/${id}/return`, {
        method: 'POST',
      })
      if (response.ok) {
        await fetchBooks()
      } else {
        alert('Failed to return book')
      }
    } catch (error) {
      console.error('Error returning book:', error)
    }
  }

  if (!auth) {
    return (
      <AuthPage
        mode={currentPath === '/register' ? 'register' : 'login'}
        authForm={authForm}
        onAuthFormChange={setAuthForm}
        onSubmit={requestAuth}
        onNavigate={navigate}
      />
    )
  }

  if (loading) {
    return <div className="flex justify-center items-center h-screen">Loading...</div>
  }

  return (
    <AppShell
      auth={auth}
      currentPath={currentPath}
      onNavigate={navigate}
      onLogout={logout}
    >
      {currentPath === '/books/new' ? (
        <AddBookPage
          newBook={newBook}
          setNewBook={setNewBook}
          addBook={addBook}
          onNavigate={navigate}
        />
      ) : (
        <BooksPage
          books={books}
          editing={editing}
          borrowUser={borrowUser}
          setBorrowUser={setBorrowUser}
          setEditing={setEditing}
          updateBook={updateBook}
          deleteBook={deleteBook}
          borrowBook={borrowBook}
          returnBook={returnBook}
        />
      )}
    </AppShell>
  )
}

export default App
