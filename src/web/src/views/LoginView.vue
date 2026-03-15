<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()
const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(email.value, password.value)
    const redirect = route.query.redirect || '/'
    router.push(redirect)
  } catch (e) {
    error.value = e.response?.data?.message || 'Giriş başarısız.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-card">
      <h1>Vinç Yönetim</h1>
      <p class="subtitle">Panele giriş yapın</p>
      <form @submit.prevent="submit" class="form">
        <input v-model="email" type="email" placeholder="E-posta" required autocomplete="email" />
        <input v-model="password" type="password" placeholder="Şifre" required autocomplete="current-password" />
        <p v-if="error" class="error">{{ error }}</p>
        <button type="submit" :disabled="loading">{{ loading ? 'Giriş yapılıyor...' : 'Giriş' }}</button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #1e3a5f 0%, #2d5a87 100%);
}
.login-card {
  background: #fff;
  padding: 2rem 2.5rem;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0,0,0,0.2);
  width: 100%;
  max-width: 380px;
}
.login-card h1 { margin: 0 0 0.25rem; font-size: 1.5rem; color: #1e3a5f; }
.subtitle { margin: 0 0 1.5rem; color: #666; font-size: 0.9rem; }
.form input {
  display: block;
  width: 100%;
  padding: 0.6rem 0.75rem;
  margin-bottom: 0.75rem;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-sizing: border-box;
}
.form button {
  width: 100%;
  padding: 0.7rem;
  background: #1e3a5f;
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
}
.form button:disabled { opacity: 0.7; cursor: not-allowed; }
.error { color: #c00; font-size: 0.85rem; margin: 0 0 0.5rem; }
</style>
