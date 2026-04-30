# TTO — Teknoloji Transfer Ofisi CMS

Üniversite Teknoloji Transfer Ofisleri için geliştirilmiş, tam yetkili içerik yönetim sistemi (CMS).  
**Backend:** .NET 9 Web API · **Frontend:** Angular 18 · **Veritabanı:** MSSQL

---

## Ekran Görüntüleri

| Anasayfa | Admin Paneli — Giriş | Sayfa Yönetimi |
|---|---|---|
| [screenshots/login.png](https://github.com/niyazix/ttoProject/blob/main/screenshots/Anasayfa.png) | *(ekran görüntüsü ekleyin)* | *(ekran görüntüsü ekleyin)* |

| Haber Yönetimi | Duyuru Yönetimi | Kullanıcı & Rol Yönetimi |
|---|---|---|
| *(ekran görüntüsü ekleyin)* | *(ekran görüntüsü ekleyin)* | *(ekran görüntüsü ekleyin)* |

---

## Özellikler

- **Dinamik Sayfa Yönetimi** — HTML içerik blokları (zengin metin, görsel, video, banner, galeri)
- **Haber Yönetimi** — kategori, etiket, kapak görseli, yayın tarihi
- **Duyuru Yönetimi** — başlangıç/bitiş tarihi, yayın kontrolü
- **Menü Yönetimi** — sürükle-bırak ile sıralama, iç/dış link desteği
- **Kullanıcı & Rol Sistemi** — Admin / Editör / İzleyici rolleri, sayfa bazlı izin (view, edit, publish, delete)
- **İletişim Mesajları** — form gönderimlerini yönetim panelinden görüntüleme
- **JWT Kimlik Doğrulama** — access token, refresh token
- **Soft Delete** — tüm içerikler silinmez, gizlenir; geri alınabilir
- **Otomatik Audit** — `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` otomatik doldurulur
- **SEO** — her sayfa için `MetaTitle` ve `MetaDescription` desteği
- **Swagger UI** — `http://localhost:5000/swagger` üzerinden tüm endpoint'ler test edilebilir

---

## Kurulum

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org) & npm
- SQL Server (LocalDB, Express veya tam sürüm)
- Angular CLI: `npm install -g @angular/cli`

---

### 1. Veritabanı

`TTO.API/appsettings.json` dosyasındaki bağlantı dizesini düzenleyin:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TTODb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Migration ve seed işlemleri API ilk açıldığında **otomatik** çalışır.

---

### 2. JWT Secret Key

`appsettings.json` içindeki `SecretKey` değerini değiştirin (minimum 32 karakter):

```json
"Jwt": {
  "SecretKey": "BURAYA_EN_AZ_32_KARAKTERLIK_GIZLI_ANAHTAR_YAZIN"
}
```

---

### 3. Backend — API

```bash
cd TTO.API
dotnet run
```

API `http://localhost:5000` adresinde çalışır.  
Swagger: `http://localhost:5000/swagger`

---

### 4. Frontend — Angular

```bash
cd tto-web
npm install
ng serve
```

Uygulama `http://localhost:4200` adresinde çalışır.

---

## Varsayılan Admin Girişi

> Uygulama ilk çalıştırıldığında seed verisi ile oluşturulur.

| Alan | Değer |
|---|---|
| **E-posta** | `admin@tto.edu.tr` |
| **Şifre** | `Admin123!` |

**Giriş adresi:** `http://localhost:4200/admin/login`

> **Güvenlik:** Canlı ortamda şifreyi değiştirmeyi ve `appsettings.json` içindeki `SecretKey`'i güçlü bir değerle değiştirmeyi unutmayın.

---

## Admin Paneli Kullanımı

### Giriş

1. `http://localhost:4200/admin/login` adresine gidin
2. E-posta: `admin@tto.edu.tr` · Şifre: `Admin123!`
3. Giriş yaptıktan sonra sol menüden tüm modüllere erişebilirsiniz

### Sayfa Oluşturma

1. Sol menüden **Sayfalar**'a tıklayın
2. **Yeni Sayfa** butonuna tıklayın
3. Başlık, slug (URL), SEO alanlarını doldurun → **Kaydet**
4. Oluşturulan sayfanın satırındaki **İçerik Blokları** (katman) ikonuna tıklayın
5. **Blok Ekle** ile zengin metin, görsel, video gibi bloklar ekleyin
6. Sayfayı yayına almak için **göz** ikonuna tıklayın

### Yayınlanan Sayfa URL'i

```
http://localhost:4200/{slug}
```

---

## Proje Yapısı

```
TTO/
├── TTO.API/            # .NET 9 Web API — controller'lar, DTO'lar, servisler
├── TTO.Core/           # Entity sınıfları ve enum'lar
├── TTO.Infrastructure/ # EF Core DbContext, migration'lar, seed verisi
└── tto-web/            # Angular 18 uygulaması
    └── src/app/
        ├── core/       # AuthService, ApiService, guard'lar
        ├── features/
        │   ├── admin/  # Admin panel modülleri
        │   └── public/ # Genel site sayfaları
        └── shared/     # Ortak bileşenler
```

---

## API Endpoint'leri (Özet)

| Metot | URL | Açıklama |
|---|---|---|
| `POST` | `/api/auth/login` | Giriş — JWT token döner |
| `POST` | `/api/auth/refresh` | Token yenileme |
| `GET` | `/api/pages` | Yayınlanan sayfalar (public) |
| `GET` | `/api/pages/{slug}` | Slug ile sayfa detayı (public) |
| `GET` | `/api/pages/admin/all` | Tüm sayfalar (admin) |
| `POST` | `/api/pages` | Yeni sayfa oluştur |
| `GET` | `/api/pages/{id}/blocks` | Sayfa blokları (admin) |
| `POST` | `/api/pages/{id}/blocks` | Blok ekle |
| `GET` | `/api/news` | Haberler (public) |
| `GET` | `/api/announcements` | Duyurular (public) |
| `POST` | `/api/contact` | İletişim formu gönder |
| `GET` | `/api/menu` | Menü öğeleri (public) |

Tüm endpoint'ler için: `http://localhost:5000/swagger`

---

## Lisans

MIT
