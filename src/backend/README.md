# Vinç Yönetim API

ASP.NET Core 8 Web API. Multi-tenant, JWT, rol bazlı erişim.

## Gereksinimler

- .NET 8 SDK
- SQL Server (LocalDB veya tam instance)
- `appsettings.Development.json` içinde `ConnectionStrings:SqlConnection` tanımlı olmalı

## Çalıştırma

1. **Backend’i durdurun** (çalışıyorsa) – build için exe’nin kilitli olmaması gerekir.
2. Migration’ları uygulayın (yeni entity’ler eklendiyse):

   ```bash
   cd src/backend
   dotnet ef migrations add AddAllEntities   # veya migration adı
   dotnet ef database update
   ```

3. Projeyi çalıştırın:

   ```bash
   dotnet run
   ```

- API: http://localhost:5042 (veya launchSettings’teki port)
- Swagger: http://localhost:5042/swagger

## Varsayılan giriş

- **E-posta:** admin@vinc.local  
- **Şifre:** Admin123!

## Önemli endpoint’ler

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | /api/v1/auth/login | Giriş (email, password) |
| POST | /api/v1/auth/forgot-password | Şifremi unuttum |
| POST | /api/v1/auth/reset-password | Şifre sıfırla (token, newPassword) |
| GET | /api/v1/dashboard | Rol bazlı dashboard verisi |
| GET | /health | Canlılık |
| GET | /health/ready | Hazırlık (DB) |
| GET/POST/PUT/DELETE | /api/v1/firms | Firmalar CRUD |
| GET/POST/PUT/DELETE | /api/v1/cranes | Vinçler CRUD |
| GET/POST/PUT/DELETE | /api/v1/operators | Operatörler CRUD |
| GET/POST/PUT/DELETE | /api/v1/sites | Şantiyeler CRUD |
| GET/POST/PUT/DELETE | /api/v1/jobs | İşler CRUD |
| GET | /api/v1/notifications | Bildirimler |
| PATCH | /api/v1/notifications/{id}/read | Okundu işaretle |

Korunan endpoint’lerde header: `Authorization: Bearer <JWT>` gerekir.
