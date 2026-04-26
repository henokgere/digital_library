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
    <div className="library-page">
      <div className="library-page__inner">
        <header className="library-header">
          <div className="library-header__row">
            <div>
              {/* <div className="library-header__eyebrow">Reading room</div> */}
              <h1 className="library-header__title">Digital Library</h1>
              <p className="library-header__description">
                Welcome back, {auth.username}. Move between pages without losing your place.
              </p>
            </div>

            <div className="library-header__actions">
              {navItems.map((item) => {
                const isActive = currentPath === item.path

                return (
                  <button
                    key={item.path}
                    onClick={() => onNavigate(item.path)}
                    className={`library-nav-button ${isActive ? 'library-nav-button--active' : ''}`}
                  >
                    {item.label}
                  </button>
                )
              })}
              <button
                onClick={onLogout}
                className="library-nav-button library-nav-button--logout"
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
