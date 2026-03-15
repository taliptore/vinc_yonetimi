import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue'),
    meta: { public: true },
  },
  {
    path: '/',
    component: () => import('../views/LayoutView.vue'),
    meta: { requiresAuth: true },
    children: [
      { path: '', name: 'Dashboard', meta: { title: 'Dashboard' }, component: () => import('../views/DashboardView.vue') },
      { path: 'firms', name: 'Firms', meta: { title: 'Firmalar', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/FirmsView.vue') },
      { path: 'cranes', name: 'Cranes', meta: { title: 'Vinçler', roles: ['Admin'] }, component: () => import('../views/CranesView.vue') },
      { path: 'operators', name: 'Operators', meta: { title: 'Operatörler', roles: ['Admin'] }, component: () => import('../views/OperatorsView.vue') },
      { path: 'sites', name: 'Sites', meta: { title: 'Şantiyeler', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/SitesView.vue') },
      { path: 'jobs', name: 'Jobs', meta: { title: 'İşler' }, component: () => import('../views/JobsView.vue') },
      { path: 'reports', name: 'Reports', meta: { title: 'Raporlar', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/ReportsView.vue') },
      { path: 'tenants', name: 'Tenants', meta: { title: 'Yeni Firma (Tenant)', roles: ['Admin'] }, component: () => import('../views/TenantsView.vue') },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, _from, next) => {
  const auth = useAuthStore()
  auth.initFromStorage()
  if (to.meta.public) {
    if (auth.isAuthenticated) next({ path: '/' })
    else next()
    return
  }
  if (!auth.isAuthenticated) {
    next({ path: '/login', query: { redirect: to.fullPath } })
    return
  }
  const roles = to.meta.roles
  if (roles?.length && !roles.includes(auth.role)) next({ path: '/' })
  else next()
})

export default router
