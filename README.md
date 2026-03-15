# Vinç Yönetim Sistemi (ERP)

SaaS mimarisinde multi-tenant, rol tabanlı erişim kontrolü ile çalışan vinç kiralama ve iş takip sistemi.  
**Backend:** ASP.NET Core 8 Web API | **Web panel:** Vue 3 + Vite + Pinia | **Mobil:** Flutter (Android).

---

## Gereksinimler

- .NET 8 SDK  
- Node.js 18+ (web panel)  
- SQL Server (LocalDB veya tam sürüm)  
- Flutter SDK (mobil için)

---

## Çalıştırma

### 1. Veritabanı

SQL Server'da boş bir veritabanı oluşturun (örn. `vinctakip`).  
Backend ilk çalıştırmada migration ile tabloları oluşturur ve seed ile varsayılan tenant + admin kullanıcı ekler.

### 2. Backend

```bash
cd src/backend
```

`appsettings.Development.json` içinde connection string'i ayarlayın:

```json
"ConnectionStrings": {
  "SqlConnection": "Data Source=SUNUCU;Initial Catalog=vinctakip;Integrated Security=True;..."
}
```

```bash
dotnet run
```

- **API:** http://localhost:5042  
- **Swagger:** http://localhost:5042/swagger  

### 3. Web panel

```bash
cd src/web
npm install
npm run dev
```

- **Adres:** http://localhost:5173  
- **Varsayılan giriş:** `admin@vinc.local` / `Admin123!`  

### 4. Mobil (Flutter)

```bash
cd src/mobile
flutter pub get
flutter run
```

Gerçek cihazda kullanmak için `lib/services/api_service.dart` içindeki `_baseUrl` değerini bilgisayarınızın IP adresi ile güncelleyin (örn. `http://192.168.1.10:5042/api/v1`).

---

## Roller

| Rol      | Özet erişim |
|----------|-------------|
| Admin    | Tüm CRUD, raporlar, sözleşme/fatura, tenant oluşturma (Owner Admin), denetim kayıtları |
| Muhasebe | Firmalar, şantiyeler, raporlar, sözleşme/fatura, denetim kayıtları |
| Operatör | İş listesi, iş başlat/bitir (GPS), yevmiye, arıza bildirimi, fotoğraf yükleme |
| Firma    | Kendi işleri, kiralanan vinçler, hakediş özeti |

---

## Proje yapısı

```
vinc-yonetimi/
├── docs/           # Geliştirme planı (PLAN.md)
├── src/
│   ├── backend/    # ASP.NET Core 8 Web API
│   ├── web/        # Vue 3 + Vite + Pinia
│   └── mobile/     # Flutter
└── README.md
```

API ön eki: `/api/v1`. Tüm listeleme ve işlemler tenant bazlı filtrelenir.
