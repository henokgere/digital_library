export interface Book {
  id: string
  title: string
  author: string
  status: string
  borrowedBy?: string
  borrowedDate?: string
}

export interface AuthInfo {
  token: string
  username: string
}

export interface AuthForm {
  username: string
  password: string
}

export type AuthMode = 'login' | 'register'
export type RoutePath = '/login' | '/register' | '/books' | '/books/new'
