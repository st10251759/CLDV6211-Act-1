<div align="center">

# 🍪 SnackTrack MVC - Complete Local Development Tutorial

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blueviolet?logo=asp.net&logoColor=white)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%208-green?logo=entity-framework&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-orange?logo=sql-server&logoColor=white)](https://learn.microsoft.com/sql/sql-server/)
[![Azure Blob](https://img.shields.io/badge/Azure%20Blob-AzURITE-blue?logo=azure&logoColor=white)](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)
[![Node.js](https://img.shields.io/badge/Node.js-20+-lightgreen?logo=node.js&logoColor=white)](https://nodejs.org/)
[![License MIT](https://img.shields.io/github/license/st10251759/CLDV6211-Act-1)](https://github.com/st10251759/CLDV6211-Act-1/blob/main/LICENSE)
[![Demo Video](https://img.shields.io/badge/Watch%20Demo-YouTube-red?logo=youtube&logoColor=white)](https://youtube.com/your-video)

</div>

## 🎯 Overview

**SnackTrack** is a complete **ASP.NET Core MVC learning project** that evolves from:

1. **In-memory lists** → **SQL Server LocalDB** (EF Core)
2. **Plain forms** → **Image uploads with Azurite blob storage**

**Perfect for students learning**:
- MVC patterns + Razor views
- Entity Framework Core migrations
- Dependency Injection
- **Azure Blob Storage emulation** (Azurite)
- Async programming + file uploads

---

## 📋 Prerequisites

```bash
# Core Tools
Visual Studio 2022+ Community
SQL Server Management Studio (SSMS)
.NET 10

# For Blob Storage (Azurite)
Node.js 18+ (npm)
```

---

## 🌐 Step 1: Setup Azurite (Local Azure Blob Storage)

### Install Node.js + Azurite
```bash
# 1. Download Node.js: https://nodejs.org → LTS version
# 2. Verify installation
node --version    # v20.x.x
npm --version     # 10.x.x

# 3. Install Azurite globally
npm install -g azurite

# 4. Start Azurite blob service (keep terminal open!)
azurite-blob --skipApiVersionCheck --silent --location c:\azurite
```

**✅ Verify**: Open `http://127.0.0.1:10000/devstoreaccount1/` ✅

### Visual Studio Alternative (No Node.js needed)
```
Tools → Options → Azure Storage Emulator → ✅ Skip API Version Check
View → Other Windows → Azure Storage Emulator → Start
```

**Resources**:
- [Azurite Docs](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) [web:88]
- [Node.js Download](https://nodejs.org/)

---

## 🗄️ Step 2: Database Migration (SQL Server LocalDB)

### 1. Create Database in SSMS
```sql
-- Connect to localhost → New Query
USE master;
CREATE DATABASE SnackMVCAppDb;
GO
USE SnackMVCAppDb;
-- Full script + 15 seeded snacks in /sql/seed-data.sql
```

### 2. Install EF Core Packages
**Package Manager Console**:
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Azure.Storage.Blobs
```

### 3. appsettings.json Connection Strings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SnackMVCAppDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True",
    "BlobConnection": "UseDevelopmentStorage=true"
  }
}
```

### 4. Create ApplicationDbContext (`Data/ApplicationDbContext.cs`)
```csharp
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Models;

namespace SnackMVCApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Snack> Snacks { get; set; }
    }
}
```

### 5. Register Services in Program.cs
```csharp
using SnackMVCApp.Data;
using SnackMVCApp.Services;

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<BlobService>(); // Azurite image storage
```

### 6. Run Migrations
```powershell
Add-Migration InitialCreate
Update-Database
```

---

## 📱 Step 3: Blob Storage Integration

### Snack Model Changes
```
Added: string? ImageUrl     // Stores blob URL in SQL
Added: [NotMapped] IFormFile? ImageFile  // File upload from form
```

### BlobService Features
- ✅ **UploadAsync(IFormFile)** → Returns public image URL
- ✅ **DeleteAsync(string url)** → Removes from Azurite
- ✅ **Lazy container creation** → No startup crashes
- ✅ **Unique filenames** → GUID prevents overwrites

**Flow**: Form file → BlobService → Azurite → URL saved to SQL → `<img src="@ImageUrl">`

---

## 🚀 Step 4: Complete Setup & Test

1. **Start Azurite** (`azurite-blob --skipApiVersionCheck`)
2. **F5** → Browse seeded snacks
3. **Create** → Upload image → Check `http://127.0.0.1:10000/devstoreaccount1/snack-images/`
4. **Edit** → Replace image → Old image auto-deleted
5. **Delete** → Snack + image permanently removed

---

## ✅ Feature Checklist

| Feature | Status |
|---------|--------|
| SQL LocalDB + EF Core | ✅ Persistent CRUD |
| Azurite Blob Images | ✅ Upload/Download/Delete |
| Responsive Bootstrap Views | ✅ Index table + image thumbs |
| Model Validation | ✅ Required + Range checks |
| Async Operations | ✅ Non-blocking |

---

## 🎨 Live Demo Features

```
📱 Index: Snack table + image thumbnails + action buttons
👁️ Details: Full image + star rating + price
➕ Create: Image upload + form validation
✏️ Edit: Replace image (old auto-deleted)
🗑️ Delete: Snack + image permanently removed
```

---

## 🐛 Troubleshooting

| Error | Solution |
|-------|----------|
| **API version 2026-02-06 not supported** | `azurite-blob --skipApiVersionCheck` OR SDK `12.22.2` |
| **Azurite not running** | Check `http://127.0.0.1:10000` |
| **No image uploads** | `enctype="multipart/form-data"` in forms |
| **DB connection failed** | SSMS → Verify `SnackMVCAppDb` |

---

## 📚 Student Resources

| Topic | Resource |
|-------|----------|
| [EF Core Tutorial](https://learn.microsoft.com/aspnet/core/data/ef-mvc/intro) [web:2] |
| [Azurite Setup](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) [web:88] |
| [Azure Blob SDK](https://learn.microsoft.com/azure/storage/blobs/storage-quickstart-blobs-dotnet) [web:95] |
| [Node.js Download](https://nodejs.org/) |
| [MVC File Upload](https://www.youtube.com/watch?v=vhyXYSLfXx0) [web:104] |

---

## 📱 Deploy to Azure (Next Steps)

```
✅ LocalDB → Azure SQL Database
✅ Azurite → Azure Blob Storage
✅ App Service deployment
```

**Swap connection strings → Deploy!** Zero code changes.

---

## 🤝 Contributing

1. Fork repository
2. Create feature branch
3. Test locally (Azurite + SSMS)
4. Pull request

## 📄 License
[MIT License](LICENSE)

<div align="center">
  
![SnackTrack](https://via.placeholder.com/800x200/EEFABD/263B6A?text=SnackTrack+MVC+Complete)
**⭐ Star if helpful!**  
**Made with ❤️ for CLDV6211 students**

[![ForTheBadge](https://img.shields.io/badge/Built%20With-.NET-brightgreen)](https://dotnet.microsoft.com/)
[![ForTheBadge](https://img.shields.io/badge/Learn-MVC-brightgreen)](https://learn.microsoft.com/aspnet/core/mvc/)

</div>
