<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'

const from = ref('')
const to = ref('')
const loading = ref(false)
const error = ref('')
const activeTab = ref('crane')

const craneUsage = ref(null)
const income = ref(null)
const fuel = ref(null)
const operatorPerf = ref(null)
const firmJobs = ref(null)

const dateRange = computed(() => {
  const toDate = to.value || new Date().toISOString().slice(0, 10)
  const fromDate = from.value || (() => {
    const d = new Date()
    d.setMonth(d.getMonth() - 1)
    return d.toISOString().slice(0, 10)
  })()
  return { from: fromDate, to: toDate }
})

async function loadReports() {
  loading.value = true
  error.value = ''
  const { from: f, to: t } = dateRange.value
  const params = { from: f, to: t }
  try {
    const [r1, r2, r3, r4, r5] = await Promise.all([
      api.get('/reports/crane-usage', { params }),
      api.get('/reports/income', { params }),
      api.get('/reports/fuel', { params }),
      api.get('/reports/operator-performance', { params }),
      api.get('/reports/firm-jobs', { params }),
    ])
    craneUsage.value = r1.data
    income.value = r2.data
    fuel.value = r3.data
    operatorPerf.value = r4.data
    firmJobs.value = r5.data
  } catch (e) {
    error.value = e.response?.data?.message || 'Raporlar yüklenemedi'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  const d = new Date()
  to.value = d.toISOString().slice(0, 10)
  d.setMonth(d.getMonth() - 1)
  from.value = d.toISOString().slice(0, 10)
  loadReports()
})
</script>

<template>
  <div class="reports">
    <div class="toolbar">
      <div class="filters">
        <label>Başlangıç <input v-model="from" type="date" /></label>
        <label>Bitiş <input v-model="to" type="date" /></label>
        <button type="button" class="btn primary" :disabled="loading" @click="loadReports">
          {{ loading ? 'Yükleniyor...' : 'Yenile' }}
        </button>
      </div>
    </div>
    <p v-if="error" class="error">{{ error }}</p>
    <div v-else class="tabs">
      <button
        v-for="tab in [
          { id: 'crane', label: 'Vinç Kullanım' },
          { id: 'income', label: 'Gelir' },
          { id: 'fuel', label: 'Yakıt' },
          { id: 'operator', label: 'Operatör Performans' },
          { id: 'firm', label: 'Firma İşleri' },
        ]"
        :key="tab.id"
        type="button"
        :class="{ active: activeTab === tab.id }"
        @click="activeTab = tab.id"
      >
        {{ tab.label }}
      </button>
    </div>
    <div class="panel">
      <!-- Vinç kullanım -->
      <div v-show="activeTab === 'crane'" class="section">
        <h3>Vinç kullanım ({{ dateRange.from }} – {{ dateRange.to }})</h3>
        <table v-if="craneUsage?.items?.length" class="table">
          <thead>
            <tr>
              <th>Vinç (Plaka)</th>
              <th>Çalışma günü</th>
              <th>Kullanım %</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in craneUsage.items" :key="row.craneId">
              <td>{{ row.cranePlate || row.craneId }}</td>
              <td>{{ row.workDays }}</td>
              <td>{{ row.usagePercent }}%</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="muted">Veri yok</p>
      </div>
      <!-- Gelir -->
      <div v-show="activeTab === 'income'" class="section">
        <h3>Gelir özeti</h3>
        <div v-if="income" class="summary">
          <p><strong>Toplam gelir:</strong> {{ income.totalIncome?.toLocaleString('tr-TR') }} ₺</p>
          <p><strong>Hakediş adedi:</strong> {{ income.hakedisCount }}</p>
        </div>
        <p v-else class="muted">Veri yok</p>
      </div>
      <!-- Yakıt -->
      <div v-show="activeTab === 'fuel'" class="section">
        <h3>Yakıt gideri</h3>
        <p v-if="fuel"><strong>Toplam tutar:</strong> {{ fuel.totalAmount?.toLocaleString('tr-TR') }} ₺</p>
        <table v-if="fuel?.items?.length" class="table">
          <thead>
            <tr>
              <th>Vinç ID</th>
              <th>Toplam litre</th>
              <th>Toplam tutar (₺)</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in fuel.items" :key="row.craneId">
              <td>{{ row.craneId }}</td>
              <td>{{ row.totalLiters }}</td>
              <td>{{ row.totalAmount?.toLocaleString('tr-TR') }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else-if="fuel && !fuel.items?.length" class="muted">Kayıt yok</p>
      </div>
      <!-- Operatör performans -->
      <div v-show="activeTab === 'operator'" class="section">
        <h3>Operatör performans</h3>
        <table v-if="operatorPerf?.items?.length" class="table">
          <thead>
            <tr>
              <th>Operatör</th>
              <th>Çalışma günü</th>
              <th>Toplam saat</th>
              <th>Mesai saat</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in operatorPerf.items" :key="row.operatorId">
              <td>{{ row.operatorName || row.operatorId }}</td>
              <td>{{ row.totalDays }}</td>
              <td>{{ row.totalHours }}</td>
              <td>{{ row.overtimeHours }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="muted">Veri yok</p>
      </div>
      <!-- Firma işleri -->
      <div v-show="activeTab === 'firm'" class="section">
        <h3>Firma bazlı iş sayısı</h3>
        <table v-if="firmJobs?.items?.length" class="table">
          <thead>
            <tr>
              <th>Firma</th>
              <th>İş sayısı</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in firmJobs.items" :key="row.firmId">
              <td>{{ row.firmName || row.firmId }}</td>
              <td>{{ row.jobCount }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="muted">Veri yok</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.reports { max-width: 900px; }
.toolbar { margin-bottom: 1rem; }
.filters { display: flex; align-items: center; gap: 1rem; flex-wrap: wrap; }
.filters label { display: flex; align-items: center; gap: 0.35rem; font-size: 0.9rem; }
.filters input { padding: 0.35rem; border: 1px solid #ccc; border-radius: 4px; }
.btn { padding: 0.5rem 1rem; border-radius: 6px; cursor: pointer; border: 1px solid #ccc; background: #fff; }
.btn.primary { background: #1e3a5f; color: #fff; border-color: #1e3a5f; }
.btn:disabled { opacity: 0.7; cursor: not-allowed; }
.error { color: #c00; margin-bottom: 1rem; }
.tabs { display: flex; gap: 0.25rem; margin-bottom: 1rem; flex-wrap: wrap; }
.tabs button { padding: 0.5rem 1rem; border: 1px solid #ccc; background: #fff; border-radius: 6px; cursor: pointer; }
.tabs button.active { background: #1e3a5f; color: #fff; border-color: #1e3a5f; }
.panel { background: #fff; padding: 1.25rem; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); }
.section h3 { margin: 0 0 1rem; font-size: 1.1rem; }
.table { width: 100%; border-collapse: collapse; }
.table th, .table td { padding: 0.5rem 0.75rem; text-align: left; border-bottom: 1px solid #eee; }
.table th { background: #f5f5f5; font-weight: 600; }
.muted { color: #888; }
.summary p { margin: 0.5rem 0; }
</style>
