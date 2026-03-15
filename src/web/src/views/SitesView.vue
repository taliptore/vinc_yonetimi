<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const list = ref([])
const loading = ref(true)
const firms = ref([])
const showForm = ref(false)
const form = ref({ id: 0, firmId: null, name: '', address: '', city: '', gpsLat: null, gpsLng: null })
const saving = ref(false)

async function load() {
  loading.value = true
  try {
    const [r1, r2] = await Promise.all([api.get('/sites'), api.get('/firms')])
    list.value = r1.data
    firms.value = r2.data
  } finally {
    loading.value = false
  }
}
onMounted(load)

function openAdd() {
  form.value = { id: 0, firmId: null, name: '', address: '', city: '', gpsLat: null, gpsLng: null }
  showForm.value = true
}
function openEdit(row) {
  form.value = { ...row }
  showForm.value = true
}
async function save() {
  saving.value = true
  try {
    if (form.value.id) await api.put(`/sites/${form.value.id}`, form.value)
    else await api.post('/sites', form.value)
    showForm.value = false
    load()
  } finally {
    saving.value = false
  }
}
async function remove(id) {
  if (!confirm('Silmek istediğinize emin misiniz?')) return
  await api.delete(`/sites/${id}`)
  load()
}
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>Şantiyeler</h2>
      <button type="button" class="btn primary" @click="openAdd">+ Ekle</button>
    </div>
    <div class="table-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Şantiye</th>
            <th>Firma</th>
            <th>Şehir</th>
            <th>Adres</th>
            <th width="100"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in list" :key="row.id">
            <td>{{ row.name }}</td>
            <td>{{ row.firm?.name }}</td>
            <td>{{ row.city }}</td>
            <td>{{ row.address }}</td>
            <td>
              <button type="button" class="btn small" @click="openEdit(row)">Düzenle</button>
              <button type="button" class="btn small danger" @click="remove(row.id)">Sil</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="showForm" class="modal" @click.self="showForm = false">
      <div class="modal-content">
        <h3>{{ form.id ? 'Düzenle' : 'Yeni Şantiye' }}</h3>
        <form @submit.prevent="save">
          <label>Firma
            <select v-model.number="form.firmId" required>
              <option v-for="f in firms" :key="f.id" :value="f.id">{{ f.name }}</option>
            </select>
          </label>
          <label>Şantiye adı <input v-model="form.name" required /></label>
          <label>Adres <input v-model="form.address" /></label>
          <label>Şehir <input v-model="form.city" /></label>
          <label>GPS Enlem <input v-model.number="form.gpsLat" type="number" step="any" /></label>
          <label>GPS Boylam <input v-model.number="form.gpsLng" type="number" step="any" /></label>
          <div class="form-actions">
            <button type="button" class="btn" @click="showForm = false">İptal</button>
            <button type="submit" class="btn primary" :disabled="saving">{{ saving ? 'Kaydediliyor...' : 'Kaydet' }}</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page { max-width: 100%; }
.toolbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
.toolbar h2 { margin: 0; font-size: 1.25rem; }
.table-wrap { background: #fff; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); overflow: auto; }
.table { width: 100%; border-collapse: collapse; }
.table th, .table td { padding: 0.6rem 0.75rem; text-align: left; border-bottom: 1px solid #eee; }
.table th { background: #f8f9fa; font-weight: 600; }
.btn { padding: 0.4rem 0.75rem; border-radius: 4px; border: 1px solid #ddd; cursor: pointer; font-size: 0.9rem; }
.btn.primary { background: #1e3a5f; color: #fff; border-color: #1e3a5f; }
.btn.small { padding: 0.25rem 0.5rem; font-size: 0.8rem; }
.btn.danger { color: #c00; border-color: #c00; }
.modal { position: fixed; inset: 0; background: rgba(0,0,0,0.4); display: flex; align-items: center; justify-content: center; z-index: 100; }
.modal-content { background: #fff; padding: 1.5rem; border-radius: 8px; min-width: 360px; }
.modal-content h3 { margin: 0 0 1rem; }
.modal-content label { display: block; margin-bottom: 0.75rem; }
.modal-content input, .modal-content select { width: 100%; padding: 0.5rem; box-sizing: border-box; }
.form-actions { display: flex; gap: 0.5rem; justify-content: flex-end; margin-top: 1rem; }
</style>
