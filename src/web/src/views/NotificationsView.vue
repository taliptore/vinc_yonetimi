<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const list = ref([])
const loading = ref(true)
const unreadOnly = ref(false)

async function load() {
  loading.value = true
  try {
    const { data } = await api.get('/notifications', { params: { unreadOnly: unreadOnly.value } })
    list.value = data
  } finally {
    loading.value = false
  }
}
onMounted(load)

async function markRead(id) {
  await api.patch(`/notifications/${id}/read`)
  load()
}
</script>

<template>
  <div class="page">
    <div class="toolbar">
      <h2>Bildirimler</h2>
      <label class="filter">
        <input v-model="unreadOnly" type="checkbox" @change="load" />
        Sadece okunmamış
      </label>
    </div>
    <div class="list-wrap">
      <p v-if="loading">Yükleniyor...</p>
      <ul v-else-if="list.length" class="notif-list">
        <li v-for="n in list" :key="n.id" :class="{ unread: !n.isRead }">
          <div class="notif-head">
            <strong>{{ n.title }}</strong>
            <span class="date">{{ n.createdAt?.slice(0, 16)?.replace('T', ' ') }}</span>
          </div>
          <p v-if="n.body" class="body">{{ n.body }}</p>
          <button v-if="!n.isRead" type="button" class="btn small" @click="markRead(n.id)">Okundu işaretle</button>
        </li>
      </ul>
      <p v-else class="muted">Bildirim yok.</p>
    </div>
  </div>
</template>

<style scoped>
.page { max-width: 640px; }
.toolbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
.toolbar h2 { margin: 0; font-size: 1.25rem; }
.filter { font-size: 0.9rem; }
.list-wrap { background: #fff; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); padding: 1rem; }
.notif-list { list-style: none; margin: 0; padding: 0; }
.notif-list li { padding: 0.75rem 0; border-bottom: 1px solid #eee; }
.notif-list li:last-child { border-bottom: none; }
.notif-list li.unread { background: #f8f9fa; margin: 0 -1rem; padding: 0.75rem 1rem; }
.notif-head { display: flex; justify-content: space-between; gap: 0.5rem; }
.notif-head .date { font-size: 0.8rem; color: #666; }
.body { margin: 0.25rem 0 0.5rem; font-size: 0.9rem; color: #444; }
.btn.small { padding: 0.2rem 0.5rem; font-size: 0.8rem; }
.muted { color: #888; }
</style>
