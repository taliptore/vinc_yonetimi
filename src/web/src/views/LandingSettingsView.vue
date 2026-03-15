<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const loading = ref(true)
const saving = ref(false)
const saveMessage = ref('')

const form = ref({
  heroTitle: '',
  heroDescription: '',
  sliderImagesJson: '[]',
  featuresJson: '[]',
  benefitsJson: '[]',
  galleryImagesJson: '[]',
  contactEmail: '',
  footerText: '',
})

const announcements = ref([])
const announcementForm = ref({ open: false, id: null, title: '', body: '', isActive: true, displayOrder: 0 })

onMounted(async () => {
  try {
    const { data: s } = await api.get('/landing-settings')
    form.value = {
      heroTitle: s.heroTitle ?? s.HeroTitle ?? '',
      heroDescription: s.heroDescription ?? s.HeroDescription ?? '',
      sliderImagesJson: typeof s.sliderImagesJson === 'string' ? s.sliderImagesJson : JSON.stringify(s.sliderImagesJson || []),
      featuresJson: typeof s.featuresJson === 'string' ? s.featuresJson : JSON.stringify(s.featuresJson || []),
      benefitsJson: typeof s.benefitsJson === 'string' ? s.benefitsJson : JSON.stringify(s.benefitsJson || []),
      galleryImagesJson: typeof s.galleryImagesJson === 'string' ? s.galleryImagesJson : JSON.stringify(s.galleryImagesJson || []),
      contactEmail: s.contactEmail ?? s.ContactEmail ?? '',
      footerText: s.footerText ?? s.FooterText ?? '',
    }
    const { data: list } = await api.get('/announcements')
    announcements.value = list || []
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
})

async function saveLanding() {
  saving.value = true
  saveMessage.value = ''
  try {
    const payload = {
      heroTitle: form.value.heroTitle,
      heroDescription: form.value.heroDescription,
      sliderImagesJson: form.value.sliderImagesJson || '[]',
      featuresJson: form.value.featuresJson || '[]',
      benefitsJson: form.value.benefitsJson || '[]',
      galleryImagesJson: form.value.galleryImagesJson || '[]',
      contactEmail: form.value.contactEmail || null,
      footerText: form.value.footerText || null,
    }
    await api.put('/landing-settings', payload)
    saveMessage.value = 'Kaydedildi.'
    setTimeout(() => { saveMessage.value = '' }, 3000)
  } catch (e) {
    saveMessage.value = 'Hata: ' + (e.response?.data?.message || e.message)
  } finally {
    saving.value = false
  }
}

function openAnnouncement(a = null) {
  if (a) {
    announcementForm.value = { open: true, id: a.id, title: a.title, body: a.body ?? '', isActive: a.isActive, displayOrder: a.displayOrder ?? 0 }
  } else {
    announcementForm.value = { open: true, id: null, title: '', body: '', isActive: true, displayOrder: announcements.value.length }
  }
}

function closeAnnouncement() {
  announcementForm.value = { open: false, id: null, title: '', body: '', isActive: true, displayOrder: 0 }
}

async function saveAnnouncement() {
  try {
    if (announcementForm.value.id) {
      await api.put(`/announcements/${announcementForm.value.id}`, {
        title: announcementForm.value.title,
        body: announcementForm.value.body,
        isActive: announcementForm.value.isActive,
        displayOrder: announcementForm.value.displayOrder,
      })
      const idx = announcements.value.findIndex(x => x.id === announcementForm.value.id)
      if (idx >= 0) {
        announcements.value[idx] = { ...announcements.value[idx], ...announcementForm.value }
      }
    } else {
      const { data } = await api.post('/announcements', {
        title: announcementForm.value.title,
        body: announcementForm.value.body,
        isActive: announcementForm.value.isActive,
        displayOrder: announcementForm.value.displayOrder,
      })
      announcements.value.push(data)
    }
    closeAnnouncement()
  } catch (e) {
    alert(e.response?.data?.message || e.message)
  }
}

async function deleteAnnouncement(id) {
  if (!confirm('Bu duyuruyu silmek istediğinize emin misiniz?')) return
  try {
    await api.delete(`/announcements/${id}`)
    announcements.value = announcements.value.filter(x => x.id !== id)
  } catch (e) {
    alert(e.response?.data?.message || e.message)
  }
}

function featuresHelp() {
  return 'JSON: [{"title":"Başlık","description":"Açıklama"}, ...]'
}
function listHelp() {
  return 'JSON string array: ["madde1","madde2"] veya tek satırda bir madde.'
}
</script>

<template>
  <div class="space-y-8">
    <h2 class="text-2xl font-semibold text-slate-800">Anasayfa Yönetimi</h2>

    <div v-if="loading" class="text-slate-500">Yükleniyor...</div>

    <template v-else>
      <!-- Landing settings -->
      <div class="bg-white rounded-xl shadow border border-slate-200 p-6 space-y-4">
        <h3 class="text-lg font-medium text-slate-800">Hero &amp; Genel</h3>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Hero başlığı</label>
          <input v-model="form.heroTitle" type="text" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300" placeholder="Ana sayfa başlığı" />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Hero açıklaması</label>
          <textarea v-model="form.heroDescription" rows="3" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300" placeholder="Kısa açıklama" />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Slider görselleri (JSON array URL)</label>
          <textarea v-model="form.sliderImagesJson" rows="3" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300 font-mono text-sm" placeholder='["https://...", "https://..."]' />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Sistem özellikleri (JSON)</label>
          <textarea v-model="form.featuresJson" rows="6" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300 font-mono text-sm" :placeholder="featuresHelp()" />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Faydalar (JSON string array)</label>
          <textarea v-model="form.benefitsJson" rows="4" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300 font-mono text-sm" placeholder='["Madde 1","Madde 2"]' />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Galeri görselleri (JSON array URL)</label>
          <textarea v-model="form.galleryImagesJson" rows="3" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300 font-mono text-sm" placeholder='["https://..."]' />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">İletişim e-posta</label>
          <input v-model="form.contactEmail" type="email" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300" placeholder="info@firma.com" />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Footer metni</label>
          <input v-model="form.footerText" type="text" class="w-full max-w-xl px-4 py-2 rounded-lg border border-slate-300" placeholder="Vinç Yönetim Sistemi" />
        </div>
        <div class="flex items-center gap-3">
          <button type="button" :disabled="saving" class="px-4 py-2 rounded-lg bg-slate-800 text-white hover:bg-slate-700 disabled:opacity-70" @click="saveLanding">
            {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
          </button>
          <span v-if="saveMessage" class="text-sm" :class="saveMessage.startsWith('Hata') ? 'text-red-600' : 'text-green-600'">{{ saveMessage }}</span>
        </div>
      </div>

      <!-- Announcements -->
      <div class="bg-white rounded-xl shadow border border-slate-200 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-slate-800">Duyurular</h3>
          <button type="button" class="px-4 py-2 rounded-lg bg-slate-800 text-white hover:bg-slate-700 text-sm" @click="openAnnouncement()">Yeni duyuru</button>
        </div>
        <div class="space-y-3">
          <div v-for="a in announcements" :key="a.id" class="flex items-center justify-between py-2 border-b border-slate-100">
            <div>
              <span class="font-medium text-slate-800">{{ a.title }}</span>
              <span v-if="!a.isActive" class="ml-2 text-xs text-slate-400">(pasif)</span>
            </div>
            <div class="flex gap-2">
              <button type="button" class="text-slate-600 hover:text-slate-800 text-sm" @click="openAnnouncement(a)">Düzenle</button>
              <button type="button" class="text-red-600 hover:text-red-800 text-sm" @click="deleteAnnouncement(a.id)">Sil</button>
            </div>
          </div>
          <p v-if="!announcements.length" class="text-slate-500 text-sm">Henüz duyuru yok.</p>
        </div>
      </div>

      <!-- Announcement modal -->
      <div v-if="announcementForm.open" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" @click.self="closeAnnouncement">
        <div class="bg-white rounded-xl shadow-xl max-w-lg w-full p-6 space-y-4">
          <h3 class="text-lg font-medium">{{ announcementForm.id ? 'Duyuru düzenle' : 'Yeni duyuru' }}</h3>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Başlık</label>
            <input v-model="announcementForm.title" type="text" class="w-full px-4 py-2 rounded-lg border border-slate-300" />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">İçerik</label>
            <textarea v-model="announcementForm.body" rows="4" class="w-full px-4 py-2 rounded-lg border border-slate-300" />
          </div>
          <div class="flex items-center gap-4">
            <label class="flex items-center gap-2">
              <input v-model="announcementForm.isActive" type="checkbox" class="rounded" />
              <span class="text-sm">Aktif</span>
            </label>
            <div>
              <label class="text-sm text-slate-600 mr-2">Sıra</label>
              <input v-model.number="announcementForm.displayOrder" type="number" class="w-20 px-2 py-1 rounded border border-slate-300" />
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" class="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-50" @click="closeAnnouncement">İptal</button>
            <button type="button" class="px-4 py-2 rounded-lg bg-slate-800 text-white hover:bg-slate-700" @click="saveAnnouncement">Kaydet</button>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
