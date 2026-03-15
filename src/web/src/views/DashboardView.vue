<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'

const data = ref(null)
const loading = ref(true)
const error = ref('')

const chartMax = computed(() => {
  const arr = data.value?.monthlyIncomeBreakdown
  if (!arr?.length) return 1
  const max = Math.max(...arr.map((x) => Number(x.total)))
  return max > 0 ? max : 1
})

onMounted(async () => {
  try {
    const { data: res } = await api.get('/dashboard')
    data.value = res
  } catch (e) {
    error.value = e.response?.data?.message || 'Yüklenemedi'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="dashboard">
    <h2>Dashboard</h2>
    <p v-if="loading">Yükleniyor...</p>
    <p v-else-if="error" class="error">{{ error }}</p>
    <div v-else-if="data" class="cards">
      <div v-if="data.totalCranes !== undefined" class="card">
        <span class="label">Toplam Vinç</span>
        <span class="value">{{ data.totalCranes }}</span>
      </div>
      <div v-if="data.totalFirms !== undefined" class="card">
        <span class="label">Toplam Firma</span>
        <span class="value">{{ data.totalFirms }}</span>
      </div>
      <div v-if="data.activeJobs !== undefined" class="card">
        <span class="label">Aktif İş</span>
        <span class="value">{{ data.activeJobs }}</span>
      </div>
      <div v-if="data.monthlyIncome !== undefined" class="card">
        <span class="label">Aylık Gelir (₺)</span>
        <span class="value">{{ data.monthlyIncome?.toLocaleString('tr-TR') }}</span>
      </div>
      <div v-if="data.monthlyFuel !== undefined" class="card">
        <span class="label">Aylık Yakıt (₺)</span>
        <span class="value">{{ data.monthlyFuel?.toLocaleString('tr-TR') }}</span>
      </div>
      <div v-if="data.monthlyMaintenance !== undefined" class="card">
        <span class="label">Aylık Bakım (₺)</span>
        <span class="value">{{ data.monthlyMaintenance?.toLocaleString('tr-TR') }}</span>
      </div>
      <div v-if="data.hakedisSum !== undefined" class="card">
        <span class="label">Hakediş (₺)</span>
        <span class="value">{{ data.hakedisSum?.toLocaleString('tr-TR') }}</span>
      </div>
      <div v-if="data.todayJob" class="card wide">
        <span class="label">Bugünkü İş</span>
        <span class="value">{{ data.todayJob?.site || data.todayJob?.plate || '-' }}</span>
      </div>
      <div v-if="data.totalHoursToday !== undefined" class="card">
        <span class="label">Bugünkü Süre (saat)</span>
        <span class="value">{{ data.totalHoursToday }}</span>
      </div>
      <div v-if="data.rentedCranes !== undefined" class="card">
        <span class="label">Kiralanan Vinç</span>
        <span class="value">{{ data.rentedCranes }}</span>
      </div>
      <div v-if="data.hakedisSummary !== undefined" class="card">
        <span class="label">Hakediş Özeti (₺)</span>
        <span class="value">{{ data.hakedisSummary?.toLocaleString('tr-TR') }}</span>
      </div>
    </div>
    <div v-if="data?.monthlyIncomeBreakdown?.length" class="chart-section">
      <h3>Son 6 ay gelir (₺)</h3>
      <div class="chart-bars">
        <div v-for="item in data.monthlyIncomeBreakdown" :key="item.month" class="chart-row">
          <span class="chart-label">{{ item.label }}</span>
          <div class="chart-bar-wrap">
            <div class="chart-bar" :style="{ width: (Number(item.total) / chartMax * 100) + '%' }" />
          </div>
          <span class="chart-value">{{ Number(item.total).toLocaleString('tr-TR') }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard h2 { margin: 0 0 1rem; font-size: 1.25rem; }
.error { color: #c00; }
.cards { display: grid; grid-template-columns: repeat(auto-fill, minmax(160px, 1fr)); gap: 1rem; }
.card {
  background: #fff;
  padding: 1rem;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.08);
  display: flex; flex-direction: column; gap: 0.25rem;
}
.card.wide { grid-column: span 2; }
.card .label { font-size: 0.8rem; color: #666; }
.card .value { font-size: 1.25rem; font-weight: 600; }
.chart-section { margin-top: 1.5rem; background: #fff; padding: 1.25rem; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); max-width: 560px; }
.chart-section h3 { margin: 0 0 1rem; font-size: 1.1rem; }
.chart-bars { display: flex; flex-direction: column; gap: 0.5rem; }
.chart-row { display: grid; grid-template-columns: 72px 1fr 80px; align-items: center; gap: 0.75rem; }
.chart-label { font-size: 0.85rem; color: #555; }
.chart-bar-wrap { height: 24px; background: #f0f2f5; border-radius: 4px; overflow: hidden; }
.chart-bar { height: 100%; background: #1e3a5f; border-radius: 4px; min-width: 2px; transition: width 0.3s ease; }
.chart-value { font-size: 0.85rem; font-weight: 600; text-align: right; }
</style>
