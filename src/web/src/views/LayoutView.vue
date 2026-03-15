<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const notifOpen = ref(false)
const notifs = ref([])
const notifLoading = ref(false)

const unreadCount = computed(() => notifs.value.filter(n => !n.isRead).length)

async function loadNotifs() {
  if (notifLoading.value) return
  notifLoading.value = true
  try {
    const { data } = await api.get('/notifications')
    notifs.value = (data || []).slice(0, 10)
  } catch {
    notifs.value = []
  } finally {
    notifLoading.value = false
  }
}

function toggleNotif() {
  notifOpen.value = !notifOpen.value
  if (notifOpen.value && notifs.value.length === 0) loadNotifs()
}

function closeNotif(e) {
  if (!e.target.closest('.notif-trigger') && !e.target.closest('.notif-dropdown')) notifOpen.value = false
}

onMounted(() => {
  loadNotifs()
  document.addEventListener('click', closeNotif)
})
onUnmounted(() => document.removeEventListener('click', closeNotif))

const menuItems = computed(() => {
  const role = auth.role
  const items = [
    { path: '/app', label: 'Dashboard', roles: ['Admin', 'Muhasebe', 'Operatör', 'Firma'] },
    { path: '/app/firms', label: 'Firmalar', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/cranes', label: 'Vinçler', roles: ['Admin'] },
    { path: '/app/operators', label: 'Operatörler', roles: ['Admin'] },
    { path: '/app/sites', label: 'Şantiyeler', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/jobs', label: 'İşler', roles: ['Admin', 'Muhasebe', 'Operatör', 'Firma'] },
    { path: '/app/reports', label: 'Raporlar', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/contracts', label: 'Sözleşmeler', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/invoices', label: 'Faturalar', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/audit-logs', label: 'Denetim Kayıtları', roles: ['Admin', 'Muhasebe'] },
    { path: '/app/tenants', label: 'Yeni Firma', roles: ['Admin'] },
    { path: '/app/landing-settings', label: 'Anasayfa Yönetimi', roles: ['Admin'] },
  ]
  return items.filter((i) => i.roles.includes(role))
})

function logout() {
  auth.logout()
  router.push('/')
}
</script>

<template>
  <div class="layout">
    <aside class="sidebar">
      <div class="logo">Vinç Yönetim</div>
      <nav>
        <router-link v-for="item in menuItems" :key="item.path" :to="item.path" active-class="active">
          {{ item.label }}
        </router-link>
      </nav>
    </aside>
    <div class="main">
      <header class="header">
        <h1 class="page-title">{{ route.meta.title || route.name }}</h1>
        <div class="user">
          <div class="notif-trigger" style="position: relative;">
            <button type="button" class="btn-notif" @click="toggleNotif" title="Bildirimler">
              Bildirimler
              <span v-if="unreadCount > 0" class="badge">{{ unreadCount > 99 ? '99+' : unreadCount }}</span>
            </button>
            <div v-if="notifOpen" class="notif-dropdown">
              <p v-if="notifLoading" class="muted">Yükleniyor...</p>
              <template v-else>
                <a v-for="n in notifs" :key="n.id" href="#" class="notif-item" :class="{ unread: !n.isRead }" @click.prevent="router.push('/app/notifications'); notifOpen = false">
                  <strong>{{ n.title }}</strong>
                  <span class="date">{{ n.createdAt?.slice(0, 16)?.replace('T', ' ') }}</span>
                </a>
                <router-link to="/app/notifications" class="notif-all" @click="notifOpen = false">Tümünü gör</router-link>
              </template>
            </div>
          </div>
          <span>{{ auth.user?.email }}</span>
          <span class="role">{{ auth.role }}</span>
          <button type="button" class="btn-logout" @click="logout">Çıkış</button>
        </div>
      </header>
      <main class="content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<style scoped>
.layout { display: flex; min-height: 100vh; }
.sidebar {
  width: 220px;
  background: #1e3a5f;
  color: #fff;
  padding: 1rem 0;
}
.logo { padding: 0 1rem 1rem; font-weight: 700; font-size: 1.1rem; border-bottom: 1px solid rgba(255,255,255,0.2); margin-bottom: 1rem; }
.sidebar nav { display: flex; flex-direction: column; }
.sidebar nav a {
  padding: 0.6rem 1rem;
  color: rgba(255,255,255,0.85);
  text-decoration: none;
}
.sidebar nav a:hover { background: rgba(255,255,255,0.1); }
.sidebar nav a.active { background: rgba(255,255,255,0.2); color: #fff; }
.main { flex: 1; display: flex; flex-direction: column; background: #f0f2f5; }
.header {
  background: #fff;
  padding: 0.75rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 1px 4px rgba(0,0,0,0.08);
}
.page-title { margin: 0; font-size: 1.25rem; font-weight: 600; }
.user { display: flex; align-items: center; gap: 0.75rem; }
.btn-notif { position: relative; padding: 0.35rem 0.75rem; background: #f0f2f5; border: 1px solid #ddd; border-radius: 4px; cursor: pointer; font-size: 0.85rem; }
.btn-notif .badge { position: absolute; top: -6px; right: -6px; min-width: 18px; padding: 0 4px; background: #dc3545; color: #fff; font-size: 0.7rem; border-radius: 9px; text-align: center; }
.notif-dropdown { position: absolute; top: 100%; right: 0; margin-top: 4px; width: 320px; max-height: 360px; overflow: auto; background: #fff; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 200; padding: 0.5rem 0; }
.notif-item { display: block; padding: 0.5rem 1rem; color: #333; text-decoration: none; border-bottom: 1px solid #eee; }
.notif-item.unread { background: #f0f7ff; }
.notif-item .date { display: block; font-size: 0.75rem; color: #666; }
.notif-all { display: block; padding: 0.5rem 1rem; text-align: center; color: #1e3a5f; font-size: 0.9rem; }
.role { font-size: 0.8rem; color: #666; }
.btn-logout { padding: 0.35rem 0.75rem; background: #dc3545; color: #fff; border: none; border-radius: 4px; cursor: pointer; font-size: 0.85rem; }
.content { padding: 1.5rem; flex: 1; }
.muted { padding: 1rem; color: #888; font-size: 0.9rem; }
</style>
