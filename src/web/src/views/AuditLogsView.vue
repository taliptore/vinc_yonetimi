<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const list = ref([])
const loading = ref(true)
const entityType = ref('')
const from = ref('')
const to = ref('')

async function load() {
  loading.value = true
  try {
    const params = { take: 100 }
    if (entityType.value) params.entityType = entityType.value
    if (from.value) params.from = from.value
    if (to.value) params.to = to.value
    const { data } = await api.get('/audit-logs', { params })
    list.value = data || []
  } finally {
    loading.value = false
  }
}
onMounted(load)
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>Denetim kayıtları</h2>
      <div class="filters">
        <input v-model="entityType" type="text" placeholder="Entity (örn. Firm)" />
        <input v-model="from" type="date" placeholder="Başlangıç" />
        <input v-model="to" type="date" placeholder="Bitiş" />
        <button type="button" class="btn primary" @click="load">Filtrele</button>
      </div>
    </div>
    <div class="table-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Tarih</th>
            <th>İşlem</th>
            <th>Entity</th>
            <th>Id</th>
            <th>Kullanıcı</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in list" :key="row.id">
            <td>{{ row.createdAt?.slice(0, 19)?.replace('T', ' ') }}</td>
            <td>{{ row.action }}</td>
            <td>{{ row.entityType }}</td>
            <td>{{ row.entityId }}</td>
            <td>{{ row.userId }}</td>
          </tr>
        </tbody>
      </table>
      <p v-if="!loading && list.length === 0" class="muted">Kayıt yok.</p>
    </div>
  </div>
</template>

<style scoped>
.page { max-width: 100%; }
.toolbar { margin-bottom: 1rem; }
.toolbar h2 { margin: 0 0 0.5rem; font-size: 1.25rem; }
.filters { display: flex; gap: 0.5rem; align-items: center; flex-wrap: wrap; }
.filters input { padding: 0.35rem 0.5rem; border: 1px solid #ccc; border-radius: 4px; }
.table-wrap { background: #fff; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); overflow: auto; }
.table { width: 100%; border-collapse: collapse; }
.table th, .table td { padding: 0.5rem 0.75rem; text-align: left; border-bottom: 1px solid #eee; }
.table th { background: #f8f9fa; font-weight: 600; }
.btn.primary { padding: 0.4rem 0.75rem; background: #1e3a5f; color: #fff; border: none; border-radius: 4px; cursor: pointer; }
.muted { padding: 1rem; color: #888; }
</style>
