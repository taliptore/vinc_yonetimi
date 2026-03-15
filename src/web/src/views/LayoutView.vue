<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const menuItems = computed(() => {
  const role = auth.role
  const items = [
    { path: '/', label: 'Dashboard', roles: ['Admin', 'Muhasebe', 'Operatör', 'Firma'] },
    { path: '/firms', label: 'Firmalar', roles: ['Admin', 'Muhasebe'] },
    { path: '/cranes', label: 'Vinçler', roles: ['Admin'] },
    { path: '/operators', label: 'Operatörler', roles: ['Admin'] },
    { path: '/sites', label: 'Şantiyeler', roles: ['Admin', 'Muhasebe'] },
    { path: '/jobs', label: 'İşler', roles: ['Admin', 'Muhasebe', 'Operatör', 'Firma'] },
    { path: '/reports', label: 'Raporlar', roles: ['Admin', 'Muhasebe'] },
    { path: '/tenants', label: 'Yeni Firma', roles: ['Admin'] },
  ]
  return items.filter((i) => i.roles.includes(role))
})

function logout() {
  auth.logout()
  router.push('/login')
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
.role { font-size: 0.8rem; color: #666; }
.btn-logout { padding: 0.35rem 0.75rem; background: #dc3545; color: #fff; border: none; border-radius: 4px; cursor: pointer; font-size: 0.85rem; }
.content { padding: 1.5rem; flex: 1; }
</style>
