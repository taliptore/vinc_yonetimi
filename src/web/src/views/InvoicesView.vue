<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const list = ref([])
const firms = ref([])
const loading = ref(true)
const showForm = ref(false)
const form = ref({
  id: 0, firmId: null, hakedisId: null, invoiceNo: '', amount: '', issueDate: '', dueDate: '', status: 'Beklemede', pdfPath: ''
})
const saving = ref(false)

async function load() {
  loading.value = true
  try {
    const [r1, r2] = await Promise.all([api.get('/invoices'), api.get('/firms')])
    list.value = r1.data
    firms.value = r2.data
  } finally {
    loading.value = false
  }
}
onMounted(load)

function openAdd() {
  form.value = {
    id: 0, firmId: firms.value[0]?.id ?? null, hakedisId: null, invoiceNo: '',
    amount: '', issueDate: new Date().toISOString().slice(0, 10), dueDate: '', status: 'Beklemede', pdfPath: ''
  }
  showForm.value = true
}
function openEdit(row) {
  form.value = {
    id: row.id, firmId: row.firmId, hakedisId: row.hakedisId ?? null, invoiceNo: row.invoiceNo,
    amount: row.amount, issueDate: row.issueDate?.slice(0, 10) ?? '', dueDate: row.dueDate?.slice(0, 10) ?? '',
    status: row.status ?? 'Beklemede', pdfPath: row.pdfPath ?? ''
  }
  showForm.value = true
}
async function save() {
  saving.value = true
  try {
    const payload = {
      firmId: form.value.firmId,
      hakedisId: form.value.hakedisId || null,
      invoiceNo: form.value.invoiceNo,
      amount: Number(form.value.amount) || 0,
      issueDate: form.value.issueDate,
      dueDate: form.value.dueDate || null,
      status: form.value.status,
      pdfPath: form.value.pdfPath || null,
    }
    if (form.value.id) await api.put(`/invoices/${form.value.id}`, payload)
    else await api.post('/invoices', payload)
    showForm.value = false
    load()
  } finally {
    saving.value = false
  }
}
async function remove(id) {
  if (!confirm('Silmek istediğinize emin misiniz?')) return
  await api.delete(`/invoices/${id}`)
  load()
}

function firmName(id) {
  return firms.value.find(f => f.id === id)?.name ?? id
}
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>Faturalar</h2>
      <button type="button" class="btn primary" @click="openAdd">+ Ekle</button>
    </div>
    <div class="table-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Fatura No</th>
            <th>Firma</th>
            <th>Tutar (₺)</th>
            <th>Düzenleme</th>
            <th>Vade</th>
            <th>Durum</th>
            <th width="100"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in list" :key="row.id">
            <td>{{ row.invoiceNo }}</td>
            <td>{{ firmName(row.firmId) }}</td>
            <td>{{ Number(row.amount).toLocaleString('tr-TR') }}</td>
            <td>{{ row.issueDate?.slice(0, 10) }}</td>
            <td>{{ row.dueDate?.slice(0, 10) || '—' }}</td>
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
        <h3>{{ form.id ? 'Düzenle' : 'Yeni Fatura' }}</h3>
        <form @submit.prevent="save">
          <label>Firma <select v-model.number="form.firmId" required><option v-for="f in firms" :key="f.id" :value="f.id">{{ f.name }}</option></select></label>
          <label>Fatura no <input v-model="form.invoiceNo" required /></label>
          <label>Tutar (₺) <input v-model="form.amount" type="number" step="0.01" required /></label>
          <label>Düzenleme tarihi <input v-model="form.issueDate" type="date" required /></label>
          <label>Vade tarihi <input v-model="form.dueDate" type="date" /></label>
          <label>Durum <select v-model="form.status"><option>Beklemede</option><option>Ödendi</option><option>İptal</option></select></label>
          <label>PDF yolu <input v-model="form.pdfPath" placeholder="/uploads/..." /></label>
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
.modal-content { background: #fff; padding: 1.5rem; border-radius: 8px; min-width: 380px; max-height: 90vh; overflow: auto; }
.modal-content h3 { margin: 0 0 1rem; }
.modal-content label { display: block; margin-bottom: 0.75rem; }
.modal-content input, .modal-content select { width: 100%; padding: 0.5rem; box-sizing: border-box; }
.form-actions { display: flex; gap: 0.5rem; justify-content: flex-end; margin-top: 1rem; }
</style>
