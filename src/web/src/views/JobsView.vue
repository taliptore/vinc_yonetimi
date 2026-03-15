<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../services/api'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const canEdit = computed(() => auth.role === 'Admin')

const list = ref([])
const loading = ref(true)
const firms = ref([])
const sites = ref([])
const cranes = ref([])
const operators = ref([])
const showForm = ref(false)
const form = ref({
  id: 0, firmId: null, siteId: null, craneId: null, operatorId: null,
  startDate: '', endDate: '', dailyRentPrice: 0,
})
const saving = ref(false)

async function load() {
  loading.value = true
  try {
    const [r1, r2, r3, r4, r5] = await Promise.all([
      api.get('/jobs'),
      api.get('/firms'),
      api.get('/sites'),
      api.get('/cranes'),
      api.get('/operators'),
    ])
    list.value = r1.data
    firms.value = r2.data
    sites.value = r3.data
    cranes.value = r4.data
    operators.value = r5.data
  } finally {
    loading.value = false
  }
}
onMounted(load)

function openAdd() {
  form.value = {
    id: 0, firmId: null, siteId: null, craneId: null, operatorId: null,
    startDate: '', endDate: '', dailyRentPrice: 0,
  }
  showForm.value = true
}
function openEdit(row) {
  form.value = {
    ...row,
    startDate: row.startDate?.slice(0, 10) ?? '',
    endDate: row.endDate?.slice(0, 10) ?? '',
  }
  showForm.value = true
}
async function save() {
  saving.value = true
  try {
    const payload = { ...form.value }
    if (form.value.id) await api.put(`/jobs/${form.value.id}`, payload)
    else await api.post('/jobs', payload)
    showForm.value = false
    load()
  } finally {
    saving.value = false
  }
}
async function remove(id) {
  if (!confirm('Silmek istediğinize emin misiniz?')) return
  await api.delete(`/jobs/${id}`)
  load()
}
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>İşler</h2>
      <button v-if="canEdit" type="button" class="btn primary" @click="openAdd">+ Ekle</button>
    </div>
    <div class="table-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Firma</th>
            <th>Şantiye</th>
            <th>Vinç</th>
            <th>Operatör</th>
            <th>Başlangıç</th>
            <th>Bitiş</th>
            <th>Günlük kira (₺)</th>
            <th width="100"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in list" :key="row.id">
            <td>{{ row.firm?.name }}</td>
            <td>{{ row.site?.name }}</td>
            <td>{{ row.crane?.plate }}</td>
            <td>{{ row.operator?.fullName || '-' }}</td>
            <td>{{ row.startDate?.slice(0, 10) }}</td>
            <td>{{ row.endDate?.slice(0, 10) }}</td>
            <td>{{ row.dailyRentPrice?.toLocaleString('tr-TR') }}</td>
            <td>
              <button v-if="canEdit" type="button" class="btn small" @click="openEdit(row)">Düzenle</button>
              <button v-if="canEdit" type="button" class="btn small danger" @click="remove(row.id)">Sil</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="showForm && canEdit" class="modal" @click.self="showForm = false">
      <div class="modal-content wide">
        <h3>{{ form.id ? 'Düzenle' : 'Yeni İş' }}</h3>
        <form @submit.prevent="save">
          <label>Firma
            <select v-model.number="form.firmId" required>
              <option v-for="f in firms" :key="f.id" :value="f.id">{{ f.name }}</option>
            </select>
          </label>
          <label>Şantiye
            <select v-model.number="form.siteId" required>
              <option v-for="s in sites" :key="s.id" :value="s.id">{{ s.name }}</option>
            </select>
          </label>
          <label>Vinç
            <select v-model.number="form.craneId" required>
              <option v-for="c in cranes" :key="c.id" :value="c.id">{{ c.plate }}</option>
            </select>
          </label>
          <label>Operatör
            <select v-model.number="form.operatorId">
              <option :value="null">—</option>
              <option v-for="o in operators" :key="o.id" :value="o.id">{{ o.fullName }}</option>
            </select>
          </label>
          <label>Başlangıç tarihi <input v-model="form.startDate" type="date" required /></label>
          <label>Bitiş tarihi <input v-model="form.endDate" type="date" required /></label>
          <label>Günlük kira bedeli (₺) <input v-model.number="form.dailyRentPrice" type="number" step="0.01" required /></label>
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
.modal-content.wide { min-width: 420px; }
.modal-content h3 { margin: 0 0 1rem; }
.modal-content label { display: block; margin-bottom: 0.75rem; }
.modal-content input, .modal-content select { width: 100%; padding: 0.5rem; box-sizing: border-box; }
.form-actions { display: flex; gap: 0.5rem; justify-content: flex-end; margin-top: 1rem; }
</style>
