# Vinç Yönetim Mobil (Operatör)

Flutter ile Android uygulaması. Operatör girişi, iş listesi, iş başlat/bitir (GPS doğrulama), arıza bildirimi.

## Gereksinim

- Flutter SDK
- Backend API çalışır olmalı

## API adresi

Varsayılan: `http://10.0.2.2:5042/api/v1` (Android emülatör → bilgisayar localhost).

**Gerçek cihazda** bilgisayarın yerel IP adresini kullanın. `lib/services/api_service.dart` içinde `_baseUrl` değerini değiştirin:

```dart
static const String _baseUrl = 'http://192.168.1.XXX:5042/api/v1';
```

## Çalıştırma

```bash
cd src/mobile
flutter pub get
flutter run
```

(Android cihaz/emülatör bağlı olmalı.)

## Özellikler

- **Giriş:** E-posta / şifre (Operatör hesabı)
- **Dashboard:** Bugünkü iş, çalışma süresi
- **İş listesi:** Atanmış işler, işe tıklayınca iş başlat/bitir ekranı
- **İş başlat / İş bitir:** Konum alınır, backend 200m şantiye kontrolü yapar
- **Arıza bildirimi:** Vinç ID + açıklama ile bildirim gönderilir

## Backend endpoint'leri (mobil)

- `POST /api/v1/auth/login`
- `GET /api/v1/dashboard`
- `GET /api/v1/jobs`
- `POST /api/v1/jobs/{id}/start` (body: latitude, longitude)
- `POST /api/v1/jobs/{id}/end` (body: latitude, longitude)
- `POST /api/v1/breakdowns` (body: craneId, description)
