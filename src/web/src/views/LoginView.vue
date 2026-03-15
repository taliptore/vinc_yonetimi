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
    const redirect = route.query.redirect || '/app'
    router.push(redirect)
  } catch (e) {
    error.value = e.response?.data?.message || 'Giriş başarısız.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 px-4">
    <div class="w-full max-w-md bg-white rounded-2xl shadow-xl p-8">
      <div class="text-center mb-8">
        <router-link to="/" class="text-xl font-bold text-slate-800">Vinç Yönetim</router-link>
        <p class="text-slate-500 mt-1">Panele giriş yapın</p>
      </div>
      <form @submit.prevent="submit" class="space-y-5">
        <div>
          <label for="email" class="block text-sm font-medium text-slate-700 mb-1">E-posta</label>
          <input
            id="email"
            v-model="email"
            type="email"
            required
            autocomplete="email"
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="ornek@firma.com"
          />
        </div>
        <div>
          <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Şifre</label>
          <input
            id="password"
            v-model="password"
            type="password"
            required
            autocomplete="current-password"
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="••••••••"
          />
        </div>
        <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
        <button
          type="submit"
          :disabled="loading"
          class="w-full py-3 rounded-xl bg-slate-800 text-white font-medium hover:bg-slate-700 disabled:opacity-70 disabled:cursor-not-allowed transition-colors"
        >
          {{ loading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
        </button>
      </form>
      <p class="mt-6 text-center text-slate-500 text-sm">
        Hesabınız yok mu?
        <router-link to="/register" class="text-slate-800 font-medium hover:underline">Hesap oluştur</router-link>
      </p>
      <p class="mt-2 text-center">
        <router-link to="/" class="text-slate-500 text-sm hover:underline">← Ana sayfaya dön</router-link>
      </p>
    </div>
  </div>
</template>
