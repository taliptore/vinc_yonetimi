<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const list = ref([])
const loading = ref(true)
const showForm = ref(false)
const form = ref({ id: 0, plate: '', brand: '', model: '', tonnage: null, year: null, status: 'Aktif' })
const saving = ref(false)

async function load() {
  loading.value = true
  try {
    const { data } = await api.get('/cranes')
    list.value = data
  } finally {
    loading.value = false
  }
}
onMounted(load)

function openAdd() {
  form.value = { id: 0, plate: '', brand: '', model: '', tonnage: null, year: null, status: 'Aktif' }
  showForm.value = true
}
function openEdit(row) {
  form.value = { ...row }
  showForm.value = true
}
async function save() {
  saving.value = true
  try {
    if (form.value.id) await api.put(`/cranes/${form.value.id}`, form.value)
    else await api.post('/cranes', form.value)
    showForm.value = false
    load()
  } finally {
    saving.value = false
  }
}
async function remove(id) {
  if (!confirm('Silmek istediğinize emin misiniz?')) return
  await api.delete(`/cranes/${id}`)
  load()
}
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>Vinçler</h2>
      <button type="button" class="btn primary" @click="openAdd">+ Ekle</button>
    </div>
    <div class="table-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Plaka</th>
            <th>Marka</th>
            <th>Model</th>
            <th>Tonaj</th>
            <th>Yıl</th>
            <th>Durum</th>
            <th width="100"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in list" :key="row.id">
            <td>{{ row.plate }}</td>
            <td>{{ row.brand }}</td>
            <td>{{ row.model }}</td>
            <td>{{ row.tonnage }}</td>
            <td>{{ row.year }}</td>
            <td>{{ row.status }}</td>
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
        <h3>{{ form.id ? 'Düzenle' : 'Yeni Vinç' }}</h3>
        <form @submit.prevent="save">
          <label>Plaka <input v-model="form.plate" required /></label>
          <label>Marka <input v-model="form.brand" /></label>
          <label>Model <input v-model="form.model" /></label>
          <label>Tonaj <input v-model.number="form.tonnage" type="number" step="any" /></label>
          <label>Yıl <input v-model.number="form.year" type="number" /></label>
          <label>Durum
            <select v-model="form.status">
              <option value="Aktif">Aktif</option>
              <option value="Pasif">Pasif</option>
              <option value="Bakımda">Bakımda</option>
            </select>
          </label>
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
