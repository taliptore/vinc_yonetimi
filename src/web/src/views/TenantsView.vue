<script setup>
import { ref } from 'vue'
import api from '../services/api'

const name = ref('')
const adminEmail = ref('')
const adminPassword = ref('')
const loading = ref(false)
const message = ref('')
const error = ref('')

async function submit() {
  if (!name.value?.trim() || !adminEmail.value?.trim() || !adminPassword.value) {
    error.value = 'Firma adı, e-posta ve şifre zorunludur.'
    return
  }
  loading.value = true
  error.value = ''
  message.value = ''
  try {
    await api.post('/tenants', {
      name: name.value.trim(),
      adminEmail: adminEmail.value.trim(),
      adminPassword: adminPassword.value,
    })
    message.value = 'Yeni firma ve admin kullanıcı oluşturuldu.'
    name.value = ''
    adminEmail.value = ''
    adminPassword.value = ''
  } catch (e) {
    if (e.response?.status === 403) {
      error.value = 'Sadece kendi firmasının sahibi (Owner Admin) yeni firma ekleyebilir.'
    } else {
      error.value = e.response?.data?.message || e.message || 'İşlem başarısız.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="tenants">
    <h2>Yeni Firma (Tenant) Ekle</h2>
    <p class="hint">Sadece kendi firmasının sahibi (Owner Admin) yeni tenant ve ilk admin kullanıcı oluşturabilir.</p>
    <form class="form" @submit.prevent="submit">
      <div class="field">
        <label>Firma adı</label>
        <input v-model="name" type="text" placeholder="Yeni firma adı" required />
      </div>
      <div class="field">
        <label>Admin e-posta</label>
        <input v-model="adminEmail" type="email" placeholder="admin@firma.com" required />
      </div>
      <div class="field">
        <label>Admin şifre</label>
        <input v-model="adminPassword" type="password" placeholder="••••••••" required />
      </div>
      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="message" class="success">{{ message }}</p>
      <button type="submit" class="btn primary" :disabled="loading">
        {{ loading ? 'Oluşturuluyor...' : 'Oluştur' }}
      </button>
    </form>
  </div>
</template>

<style scoped>
.tenants { max-width: 420px; }
.tenants h2 { margin: 0 0 0.5rem; font-size: 1.25rem; }
.hint { font-size: 0.9rem; color: #666; margin-bottom: 1.25rem; }
.form { background: #fff; padding: 1.25rem; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); }
.field { margin-bottom: 1rem; }
.field label { display: block; margin-bottom: 0.35rem; font-size: 0.9rem; font-weight: 500; }
.field input { width: 100%; padding: 0.5rem; border: 1px solid #ccc; border-radius: 6px; box-sizing: border-box; }
.error { color: #c00; font-size: 0.9rem; margin: 0.5rem 0; }
.success { color: #0a0; font-size: 0.9rem; margin: 0.5rem 0; }
.btn { padding: 0.5rem 1rem; border-radius: 6px; cursor: pointer; border: 1px solid #ccc; background: #fff; margin-top: 0.5rem; }
.btn.primary { background: #1e3a5f; color: #fff; border-color: #1e3a5f; }
.btn:disabled { opacity: 0.7; cursor: not-allowed; }
</style>
