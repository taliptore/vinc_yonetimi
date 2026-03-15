import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const routes = [
  {
    path: '/',
    name: 'Landing',
    component: () => import('../views/LandingView.vue'),
    meta: { public: true },
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue'),
    meta: { public: true },
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('../views/RegisterView.vue'),
    meta: { public: true },
  },
  {
    path: '/app',
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
      { path: 'contracts', name: 'Contracts', meta: { title: 'Sözleşmeler', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/ContractsView.vue') },
      { path: 'invoices', name: 'Invoices', meta: { title: 'Faturalar', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/InvoicesView.vue') },
      { path: 'notifications', name: 'Notifications', meta: { title: 'Bildirimler' }, component: () => import('../views/NotificationsView.vue') },
      { path: 'audit-logs', name: 'AuditLogs', meta: { title: 'Denetim Kayıtları', roles: ['Admin', 'Muhasebe'] }, component: () => import('../views/AuditLogsView.vue') },
      { path: 'tenants', name: 'Tenants', meta: { title: 'Yeni Firma (Tenant)', roles: ['Admin'] }, component: () => import('../views/TenantsView.vue') },
      { path: 'landing-settings', name: 'LandingSettings', meta: { title: 'Anasayfa Yönetimi', roles: ['Admin'] }, component: () => import('../views/LandingSettingsView.vue') },
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
    if (auth.isAuthenticated) next({ path: '/app' })
    else next()
    return
  }
  if (!auth.isAuthenticated) {
    next({ path: '/login', query: { redirect: to.fullPath } })
    return
  }
  const roles = to.meta.roles
  if (roles?.length && !roles.includes(auth.role)) next({ path: '/app' })
  else next()
})

export default router
