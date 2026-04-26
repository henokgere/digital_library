import type { ReactNode } from 'react'
import type { AuthInfo, RoutePath } from '../types'

export function AppShell({
  auth,
  currentPath,
  onNavigate,
  onLogout,
  children,
}: {
  auth: AuthInfo
  currentPath: RoutePath
  onNavigate: (path: RoutePath) => void
  onLogout: () => void
  children: ReactNode
}) {
  const navItems: Array<{ path: RoutePath; label: string }> = [
    { path: '/books', label: 'Catalog' },
    { path: '/books/new', label: 'Add Book' },
  ]

  return (
    <div className="min-h-screen bg-slate-50 py-8 px-4">
      <div className="mx-auto max-w-6xl">
        <header className="mb-8 rounded-3xl bg-white p-6 shadow-sm">
          <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
            <div>
              <h1 className="text-3xl font-bold text-slate-900">Digital Library</h1>
              <p className="mt-2 text-sm text-slate-600">
                Welcome back, {auth.username}. Move between pages without losing your place.
              </p>
            </div>

            <div className="flex flex-wrap items-center gap-3">
              {navItems.map((item) => {
                const isActive = currentPath === item.path

                return (
                  <button
                    key={item.path}
                    onClick={() => onNavigate(item.path)}
                    className={`rounded-2xl px-4 py-3 text-sm font-semibold transition ${
                      isActive
                        ? 'bg-slate-900 text-white'
                        : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-100'
                    }`}
                  >
                    {item.label}
                  </button>
                )
              })}
              <button
                onClick={onLogout}
                className="rounded-2xl bg-red-500 px-5 py-3 text-sm font-semibold text-white transition hover:bg-red-600"
              >
                Logout
              </button>
            </div>
          </div>
        </header>

        {children}
      </div>
    </div>
  )
}
