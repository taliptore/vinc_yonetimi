<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const companyName = ref('')
const adminName = ref('')
const email = ref('')
const phone = ref('')
const password = ref('')
const error = ref('')
const success = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  success.value = ''
  loading.value = true
  try {
    await api.post('/auth/register', {
      companyName: companyName.value.trim(),
      adminName: adminName.value.trim(),
      email: email.value.trim().toLowerCase(),
      phone: phone.value.trim() || undefined,
      password: password.value,
    })
    success.value = 'Hesabınız oluşturuldu. Giriş yapabilirsiniz.'
    setTimeout(() => router.push('/login'), 2000)
  } catch (e) {
    error.value = e.response?.data?.message || 'Kayıt başarısız.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 px-4 py-12">
    <div class="w-full max-w-md bg-white rounded-2xl shadow-xl p-8">
      <div class="text-center mb-8">
        <router-link to="/" class="text-xl font-bold text-slate-800">Vinç Yönetim</router-link>
        <p class="text-slate-500 mt-1">Yeni hesap oluştur</p>
      </div>
      <form @submit.prevent="submit" class="space-y-4">
        <div>
          <label for="companyName" class="block text-sm font-medium text-slate-700 mb-1">Firma adı</label>
          <input
            id="companyName"
            v-model="companyName"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="Firma Adı A.Ş."
          />
        </div>
        <div>
          <label for="adminName" class="block text-sm font-medium text-slate-700 mb-1">Admin adı</label>
          <input
            id="adminName"
            v-model="adminName"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="Ad Soyad"
          />
        </div>
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
          <label for="phone" class="block text-sm font-medium text-slate-700 mb-1">Telefon</label>
          <input
            id="phone"
            v-model="phone"
            type="tel"
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="05XX XXX XX XX"
          />
        </div>
        <div>
          <label for="password" class="block text-sm font-medium text-slate-700 mb-1">Şifre</label>
          <input
            id="password"
            v-model="password"
            type="password"
            required
            minlength="6"
            autocomplete="new-password"
            class="w-full px-4 py-3 rounded-xl border border-slate-300 focus:ring-2 focus:ring-slate-500 focus:border-slate-500"
            placeholder="••••••••"
          />
        </div>
        <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
        <p v-if="success" class="text-sm text-green-600">{{ success }}</p>
        <button
          type="submit"
          :disabled="loading"
          class="w-full py-3 rounded-xl bg-slate-800 text-white font-medium hover:bg-slate-700 disabled:opacity-70 disabled:cursor-not-allowed transition-colors"
        >
          {{ loading ? 'Oluşturuluyor...' : 'Hesap Oluştur' }}
        </button>
      </form>
      <p class="mt-6 text-center text-slate-500 text-sm">
        Zaten hesabınız var mı?
        <router-link to="/login" class="text-slate-800 font-medium hover:underline">Giriş yap</router-link>
      </p>
      <p class="mt-2 text-center">
        <router-link to="/" class="text-slate-500 text-sm hover:underline">← Ana sayfaya dön</router-link>
      </p>
    </div>
  </div>
</template>
