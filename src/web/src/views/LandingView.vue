<script setup>
import { ref, onMounted } from 'vue'
import { getLandingSettings, getAnnouncements } from '../services/publicApi'

const settings = ref(null)
const announcements = ref([])
const sliderIndex = ref(0)
const loading = ref(true)
const mobileMenuOpen = ref(false)

onMounted(async () => {
  document.title = 'Vinç Yönetim Sistemi | Ana Sayfa'
  try {
    const [s, a] = await Promise.all([getLandingSettings(), getAnnouncements()])
    settings.value = s
    announcements.value = a || []
  } catch {
    settings.value = {
      heroTitle: 'Vinç Yönetim Sistemi ile tüm operasyonlarınızı tek panelden yönetin',
      heroDescription: 'Vinç kiralama, operatör yönetimi, hakediş, şantiye takibi ve mobil saha uygulaması ile tüm süreçleri dijital yönetin.',
      sliderImages: [],
      features: [
        { title: 'Vinç Yönetimi', description: 'Vinçlerinizi plaka, tonaj ve kullanım durumuna göre takip edin.' },
        { title: 'Operatör Yönetimi', description: 'Operatörlerin çalışma saatleri ve performansını izleyin.' },
        { title: 'Şantiye Yönetimi', description: 'Kiralanan vinçlerin hangi şantiyede çalıştığını görün.' },
      ],
      benefits: [
        'Vinç doluluk analizi',
        'Operatör yevmiye takibi',
        'Hakediş hesaplama',
        'Mobil saha yönetimi',
        'Yakıt ve bakım takibi',
      ],
      galleryImages: [],
      contactEmail: '',
      footerText: 'Vinç Yönetim Sistemi',
    }
  } finally {
    loading.value = false
  }
})

const features = () => (settings.value?.features?.length ? settings.value.features : settings.value ? [
  { title: 'Vinç Yönetimi', description: 'Vinçlerinizi plaka, tonaj ve kullanım durumuna göre takip edin.' },
  { title: 'Operatör Yönetimi', description: 'Operatörlerin çalışma saatleri ve performansını izleyin.' },
  { title: 'Şantiye Yönetimi', description: 'Kiralanan vinçlerin hangi şantiyede çalıştığını görün.' },
] : [])
const benefits = () => (settings.value?.benefits?.length ? settings.value.benefits : settings.value ? [
  'Vinç doluluk analizi', 'Operatör yevmiye takibi', 'Hakediş hesaplama', 'Mobil saha yönetimi', 'Yakıt ve bakım takibi',
] : [])
const sliderImages = () => (settings.value?.sliderImages?.length ? settings.value.sliderImages : [])
const galleryImages = () => (settings.value?.galleryImages?.length ? settings.value.galleryImages : [])

function scrollTo(id) {
  document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' })
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-800">
    <!-- Header -->
    <header class="fixed top-0 left-0 right-0 z-50 bg-white/95 backdrop-blur border-b border-slate-200">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 flex items-center justify-between h-16">
        <router-link to="/" class="text-xl font-bold text-slate-800">Vinç Yönetim</router-link>
        <nav class="hidden md:flex items-center gap-8">
          <button type="button" @click="scrollTo('hero')" class="text-slate-600 hover:text-slate-900">Ana Sayfa</button>
          <button type="button" @click="scrollTo('features')" class="text-slate-600 hover:text-slate-900">Sistem Özellikleri</button>
          <button type="button" @click="scrollTo('benefits')" class="text-slate-600 hover:text-slate-900">Faydalar</button>
          <button type="button" @click="scrollTo('gallery')" class="text-slate-600 hover:text-slate-900">Galeri</button>
          <button type="button" @click="scrollTo('contact')" class="text-slate-600 hover:text-slate-900">İletişim</button>
        </nav>
        <div class="hidden md:flex items-center gap-3">
          <router-link to="/login" class="btn-secondary">Giriş Yap</router-link>
          <router-link to="/register" class="btn-primary">Hesap Oluştur</router-link>
        </div>
        <button type="button" class="md:hidden p-2 text-slate-600 hover:bg-slate-100 rounded-lg" aria-label="Menü" @click="mobileMenuOpen = !mobileMenuOpen">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" /></svg>
        </button>
      </div>
      <div v-if="mobileMenuOpen" class="md:hidden border-t border-slate-200 bg-white px-4 py-4 space-y-2">
        <button type="button" class="block w-full text-left py-2 text-slate-600" @click="scrollTo('hero'); mobileMenuOpen = false">Ana Sayfa</button>
        <button type="button" class="block w-full text-left py-2 text-slate-600" @click="scrollTo('features'); mobileMenuOpen = false">Sistem Özellikleri</button>
        <button type="button" class="block w-full text-left py-2 text-slate-600" @click="scrollTo('benefits'); mobileMenuOpen = false">Faydalar</button>
        <button type="button" class="block w-full text-left py-2 text-slate-600" @click="scrollTo('gallery'); mobileMenuOpen = false">Galeri</button>
        <button type="button" class="block w-full text-left py-2 text-slate-600" @click="scrollTo('contact'); mobileMenuOpen = false">İletişim</button>
        <div class="flex gap-2 pt-2">
          <router-link to="/login" class="btn-secondary flex-1 text-center" @click="mobileMenuOpen = false">Giriş Yap</router-link>
          <router-link to="/register" class="btn-primary flex-1 text-center" @click="mobileMenuOpen = false">Hesap Oluştur</router-link>
        </div>
      </div>
    </header>

    <!-- Hero -->
    <section id="hero" class="pt-28 pb-20 px-4 bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
      <div class="max-w-4xl mx-auto text-center">
        <template v-if="!loading && settings">
          <h1 class="text-4xl sm:text-5xl font-bold leading-tight mb-6">
            {{ settings.heroTitle }}
          </h1>
          <p class="text-lg sm:text-xl text-slate-300 mb-10 max-w-2xl mx-auto">
            {{ settings.heroDescription }}
          </p>
        </template>
        <div v-else class="animate-pulse space-y-4">
          <div class="h-12 bg-slate-700 rounded w-3/4 mx-auto" />
          <div class="h-6 bg-slate-700 rounded w-full max-w-xl mx-auto" />
        </div>
        <div class="flex flex-wrap justify-center gap-4">
          <router-link to="/register" class="btn-primary bg-emerald-500 hover:bg-emerald-600 text-white px-8 py-3 rounded-xl font-semibold">
            Hemen Hesap Oluştur
          </router-link>
          <router-link to="/login" class="btn-secondary border-white/40 text-white hover:bg-white/10 px-8 py-3 rounded-xl font-semibold">
            Giriş Yap
          </router-link>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section id="features" class="py-20 px-4">
      <div class="max-w-6xl mx-auto">
        <h2 class="text-3xl font-bold text-center text-slate-800 mb-12">Sistem Tanıtımı</h2>
        <div class="grid md:grid-cols-3 gap-8">
          <div
            v-for="(f, i) in features()"
            :key="i"
            class="bg-white rounded-2xl p-8 shadow-lg border border-slate-100 hover:shadow-xl transition-shadow"
          >
            <div class="w-12 h-12 rounded-xl bg-slate-100 flex items-center justify-center text-2xl mb-4">📋</div>
            <h3 class="text-xl font-semibold text-slate-800 mb-2">{{ f.title }}</h3>
            <p class="text-slate-600">{{ f.description }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Benefits -->
    <section id="benefits" class="py-20 px-4 bg-slate-100">
      <div class="max-w-4xl mx-auto text-center">
        <h2 class="text-3xl font-bold text-slate-800 mb-10">Neden Vinç Yönetim Sistemi?</h2>
        <ul class="space-y-4 text-left inline-block">
          <li
            v-for="(b, i) in benefits()"
            :key="i"
            class="flex items-center gap-3 text-slate-700"
          >
            <span class="w-6 h-6 rounded-full bg-emerald-500 text-white flex items-center justify-center text-sm flex-shrink-0">✓</span>
            {{ b }}
          </li>
        </ul>
      </div>
    </section>

    <!-- Slider -->
    <section v-if="sliderImages().length" class="py-20 px-4">
      <div class="max-w-4xl mx-auto">
        <h2 class="text-3xl font-bold text-center text-slate-800 mb-10">Sistem Ekran Görüntüleri</h2>
        <div class="relative rounded-2xl overflow-hidden shadow-2xl bg-slate-200 aspect-video">
          <img
            v-for="(img, i) in sliderImages()"
            :key="i"
            :src="img"
            :alt="'Ekran ' + (i + 1)"
            class="absolute inset-0 w-full h-full object-contain transition-opacity duration-300"
            :class="{ 'opacity-100 z-10': sliderIndex === i, 'opacity-0': sliderIndex !== i }"
          />
          <button
            v-if="sliderImages().length > 1"
            type="button"
            class="absolute left-4 top-1/2 -translate-y-1/2 z-20 w-12 h-12 rounded-full bg-white/90 shadow flex items-center justify-center text-slate-700 hover:bg-white"
            @click="sliderIndex = (sliderIndex - 1 + sliderImages().length) % sliderImages().length"
          >
            ‹
          </button>
          <button
            v-if="sliderImages().length > 1"
            type="button"
            class="absolute right-4 top-1/2 -translate-y-1/2 z-20 w-12 h-12 rounded-full bg-white/90 shadow flex items-center justify-center text-slate-700 hover:bg-white"
            @click="sliderIndex = (sliderIndex + 1) % sliderImages().length"
          >
            ›
          </button>
          <div v-if="sliderImages().length > 1" class="absolute bottom-4 left-1/2 -translate-x-1/2 z-20 flex gap-2">
            <button
              v-for="(_, i) in sliderImages()"
              :key="i"
              type="button"
              class="w-2.5 h-2.5 rounded-full transition-colors"
              :class="sliderIndex === i ? 'bg-white' : 'bg-white/50'"
              @click="sliderIndex = i"
            />
          </div>
        </div>
      </div>
    </section>

    <!-- Gallery -->
    <section id="gallery" class="py-20 px-4 bg-slate-100">
      <div class="max-w-6xl mx-auto">
        <h2 class="text-3xl font-bold text-center text-slate-800 mb-10">Galeri</h2>
        <div v-if="galleryImages().length" class="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="(img, i) in galleryImages()"
            :key="i"
            class="rounded-xl overflow-hidden shadow-lg bg-white"
          >
            <img :src="img" :alt="'Galeri ' + (i + 1)" class="w-full h-56 object-cover" />
          </div>
        </div>
        <p v-else class="text-center text-slate-500">Galeri görselleri yönetici panelinden eklenebilir.</p>
      </div>
    </section>

    <!-- Announcements -->
    <section class="py-20 px-4">
      <div class="max-w-4xl mx-auto">
        <h2 class="text-3xl font-bold text-center text-slate-800 mb-10">Duyurular</h2>
        <div v-if="announcements.length" class="space-y-6">
          <div
            v-for="a in announcements"
            :key="a.id"
            class="bg-white rounded-xl p-6 shadow border border-slate-100"
          >
            <h3 class="font-semibold text-slate-800 mb-2">{{ a.title }}</h3>
            <p class="text-slate-600 whitespace-pre-wrap">{{ a.body }}</p>
            <p class="text-sm text-slate-400 mt-2">{{ a.createdAt?.slice(0, 10) }}</p>
          </div>
        </div>
        <p v-else class="text-center text-slate-500">Henüz duyuru yok.</p>
      </div>
    </section>

    <!-- Footer -->
    <footer id="contact" class="py-12 px-4 bg-slate-900 text-slate-300">
      <div class="max-w-6xl mx-auto flex flex-col md:flex-row items-center justify-between gap-6">
        <p class="text-slate-400">{{ settings?.footerText || 'Vinç Yönetim Sistemi' }}</p>
        <div class="flex flex-col sm:flex-row items-center gap-4">
          <span v-if="settings?.contactEmail" class="text-slate-400">E-posta: {{ settings.contactEmail }}</span>
          <router-link to="/" class="text-slate-400 hover:text-white">Ana Sayfa</router-link>
          <router-link to="/login" class="text-slate-400 hover:text-white">Giriş Yap</router-link>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.btn-primary {
  @apply inline-flex items-center justify-center rounded-xl font-medium transition-colors bg-slate-800 text-white hover:bg-slate-700 px-5 py-2.5;
}
.btn-secondary {
  @apply inline-flex items-center justify-center rounded-xl font-medium border border-slate-300 text-slate-700 hover:bg-slate-100 px-5 py-2.5 transition-colors;
}
</style>
