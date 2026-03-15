import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token'),
    user: JSON.parse(localStorage.getItem('user') || 'null'),
  }),
  getters: {
    isAuthenticated: (s) => !!s.token,
    role: (s) => s.user?.role ?? '',
    tenantId: (s) => s.user?.tenantId ?? null,
  },
  actions: {
    async login(email, password) {
      const { data } = await api.post('/auth/login', { email, password })
      this.token = data.token
      this.user = {
        email: data.email,
        role: data.role,
        tenantId: data.tenantId,
        userId: data.userId,
      }
      localStorage.setItem('token', data.token)
      localStorage.setItem('user', JSON.stringify(this.user))
      return data
    },
    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },
    initFromStorage() {
      this.token = localStorage.getItem('token')
      this.user = JSON.parse(localStorage.getItem('user') || 'null')
    },
  },
})
