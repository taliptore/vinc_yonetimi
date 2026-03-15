# Vinç Yönetim Web Panel

Vue 3 + Vite + Pinia + Vue Router. Backend API ile haberleşir.

## Gereksinim

- Node 18+
- Backend API çalışıyor olmalı (varsayılan: http://localhost:5042)

## Çalıştırma

```bash
cd src/web
npm install
npm run dev
```

Tarayıcı: http://localhost:5173

Vite proxy sayesinde `/api` istekleri otomatik olarak backend'e (5042) yönlendirilir.

## Varsayılan giriş

- E-posta: **admin@vinc.local**
- Şifre: **Admin123!**

## Sayfalar

- **Login** – Giriş
- **Dashboard** – Rol bazlı özet kartlar
- **Firmalar** – CRUD (Admin, Muhasebe)
- **Vinçler** – CRUD (Admin)
- **Operatörler** – CRUD (Admin)
- **Şantiyeler** – CRUD (Admin, Muhasebe)
- **İşler** – Listeleme (tüm roller), CRUD (Admin)

Menü rol bazlı gösterilir.
