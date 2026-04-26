import type { AuthForm, AuthMode, RoutePath } from '../types'

export function AuthPage({
  mode,
  authForm,
  onAuthFormChange,
  onSubmit,
  onNavigate,
}: {
  mode: AuthMode
  authForm: AuthForm
  onAuthFormChange: (value: AuthForm) => void
  onSubmit: (mode: AuthMode) => void
  onNavigate: (path: RoutePath) => void
}) {
  return (
    <div className="auth-page">
      <div className="auth-page__cloud auth-page__cloud--top" aria-hidden="true">
        <svg viewBox="0 0 220 90" fill="none">
          <path
            d="M34 61c-13 0-24-9-24-20 0-12 12-21 26-20 5-12 18-20 34-20 17 0 31 9 35 23 4-2 8-3 13-3 15 0 27 10 27 22 0 10-9 18-21 18H34Z"
            stroke="currentColor"
            strokeWidth="2.2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>

      <div className="auth-page__cloud auth-page__cloud--middle" aria-hidden="true">
        <svg viewBox="0 0 190 76" fill="none">
          <path
            d="M28 55C15 55 5 47 5 37c0-9 9-17 21-17 5-10 16-17 30-17 14 0 26 7 30 18 4-2 8-2 11-2 13 0 23 8 23 18s-9 18-21 18H28Z"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>

      <div className="auth-page__cloud auth-page__cloud--bottom" aria-hidden="true">
        <svg viewBox="0 0 240 96" fill="none">
          <path
            d="M38 68c-15 0-28-10-28-23 0-12 12-22 28-22 4-13 19-23 37-23 19 0 34 9 40 24 4-2 8-3 14-3 16 0 29 10 29 23 0 13-13 24-29 24H38Z"
            stroke="currentColor"
            strokeWidth="2.2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>

      <div className="auth-card">
        <div className="auth-card__eyebrow">Library access</div>
        <h1 className="auth-card__title">Digital Library</h1>
        <p className="auth-card__description">
          {mode === 'login'
            ? 'Sign in to manage books, borrow, and return items.'
            : 'Create an account to start borrowing books today.'}
        </p>

        <label className="auth-field">
          <span className="auth-field__label">Username</span>
          <input
            type="text"
            value={authForm.username}
            onChange={(e) => onAuthFormChange({ ...authForm, username: e.target.value })}
            className="auth-field__input"
          />
        </label>

        <label className="auth-field auth-field--last">
          <span className="auth-field__label">Password</span>
          <input
            type="password"
            value={authForm.password}
            onChange={(e) => onAuthFormChange({ ...authForm, password: e.target.value })}
            className="auth-field__input"
          />
        </label>

        <button
          onClick={() => onSubmit(mode)}
          className="auth-button auth-button--primary"
        >
          {mode === 'login' ? 'Login' : 'Register'}
        </button>

        <button
          onClick={() => onNavigate(mode === 'login' ? '/register' : '/login')}
          className="auth-button auth-button--secondary"
        >
          {mode === 'login' ? 'Create account' : 'Already have an account?'}
        </button>
      </div>
    </div>
  )
}
