import axios from 'axios'

const publicApi = axios.create({
  baseURL: '/api/v1',
  headers: { 'Content-Type': 'application/json' },
})

export async function getLandingSettings() {
  const { data } = await publicApi.get('/public/landing-settings')
  return data
}

export async function getAnnouncements() {
  const { data } = await publicApi.get('/public/announcements')
  return data
}
