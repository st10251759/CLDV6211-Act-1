<div align="center">

# 🍪 SnackTrack MVC - In-Memory to Database Migration Tutorial

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blueviolet?logo=asp.net&logoColor=white)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%208-green?logo=entity-framework&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-orange?logo=sql-server&logoColor=white)](https://learn.microsoft.com/sql/sql-server/)
[![License MIT](https://img.shields.io/github/license/yourusername/SnackTrackMVC)](https://github.com/yourusername/SnackTrackMVC/blob/main/LICENSE)
[![Demo Video](https://img.shields.io/badge/Watch%20Demo-YouTube-red?logo=youtube&logoColor=white)](https://youtube.com/your-video)

</div>

## 🎯 Overview

Transform your **in-memory SnackTrack MVC app** (using `static List<Snack>`) into a **production-ready database application** with **Entity Framework Core** and **SQL Server LocalDB**. 

**Current Problem**: Data lost on app restart.  
**Solution**: Persistent SQL database + EF Core ORM.

**What you'll learn**:
- SQL Server database creation & seeding
- EF Core DbContext & migrations
- Dependency Injection setup
- Async CRUD controller

---

## 📋 Prerequisites

```bash
# Required Tools
Visual Studio 2022+ (Community OK)
SQL Server Management Studio (SSMS)
.NET 8 SDK
```

---

## 🚀 Step-by-Step Migration Guide

### 1. Create Database in SSMS
```sql
-- Run in SSMS connected to localhost
USE master;
CREATE DATABASE SnackMVCAppDb;
GO
USE SnackMVCAppDb;
-- Full table + 15 records script in /sql/seed-data.sql
```

<details>
<summary>🔍 View Seed Script</summary>

```sql
-- Snacks table matching your model constraints
CREATE TABLE Snacks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Brand NVARCHAR(100) NOT NULL,
    Type INT NOT NULL CHECK (Type BETWEEN 0 AND 5), -- SnackType enum
    Description NVARCHAR(200) NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Price DECIMAL(10,2) NOT NULL CHECK (Price BETWEEN 0 AND 1000)
);
-- Insert 15 sample snacks...
```

</details>

### 2. Install EF Core Packages
**Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console):
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
```

### 3. Add Connection String
**`appsettings.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SnackMVCAppDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 4. Create ApplicationDbContext
**Add folder** `Data/` → New Class `ApplicationDbContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Models;

namespace SnackMVCApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        public DbSet<Snack> Snacks { get; set; }
    }
}
```

### 5. Register DbContext in Program.cs
```csharp
using SnackMVCApp.Data; // Add this

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Add this code to register your Application DB context and Connectionstring
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// ... rest unchanged
```

### 6. Run EF Migrations
**Package Manager Console**:
```powershell
Add-Migration InitialCreate
Update-Database
```
✅ **Success**: Schema synced! Check SSMS Tables → `dbo.Snacks`.

### 7. Update SnacksController (Key Changes)
**Delete** old controller. Replace with **EF version**:

| In-Memory (OLD) | EF Core (NEW) | Why? |
|-----------------|---------------|------|
| `static List<Snack> snacks` | `private readonly ApplicationDbContext _context` | DI injection |
| `snacks.ToList()` | `await _context.Snacks.ToListAsync()` | Async DB query |
| `snack.Id = Max() + 1` | **Auto IDENTITY** | DB generates ID |
| `snacks.Add()` | `_context.Add(); SaveChangesAsync()` | EF tracks changes |

**Full EF Controller** (`Controllers/SnacksController.cs`):
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Data;
using SnackMVCApp.Models;

namespace SnackMVCApp.Controllers
{
    public class SnacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SnacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Snacks.ToListAsync());
        }

        // Full CRUD methods: Details, Create, Edit, Delete...
        // See complete code in /Controllers/SnacksController.cs
    }
}
```

### 8. Test CRUD Functionality
```
1. F5 → Index shows 15 seeded snacks
2. Add → Creates new record (ID auto-generated)
3. Edit → Updates DB
4. Delete → Removes permanently
5. Restart → Data persists!
```

---

## ✅ Migration Checklist

| Step | Status | Notes |
|------|--------|-------|
| SSMS Database Created | ☐ | Run seed script |
| EF Packages Installed | ☐ | 3 packages |
| Connection String Added | ☐ | localhost |
| DbContext Created | ☐ | Data/ApplicationDbContext.cs |
| Program.cs Updated | ☐ | AddDbContext |
| Migrations Run | ☐ | InitialCreate |
| Controller Updated | ☐ | EF async |
| CRUD Tested | ☐ | Persistence verified |

---

## 🎨 Features After Migration

- **Persistent Data**: Survives app restarts
- **Async Operations**: Scalable performance
- **Auto ID Generation**: No manual Max() logic
- **Model Validation**: DB CHECK constraints
- **Scaffold Ready**: Easy view generation

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| "Cannot connect to DB" | SSMS → Verify `SnackMVCAppDb` exists |
| "No migrations" | `dotnet ef migrations add InitialCreate` |
| "No DbContext" | Check `Program.cs` using statement |
| "Async error" | Add `await` + `Task<IActionResult>` |

## 📄 License
This project is [MIT](LICENSE) licensed.

<div align="center">
  
**⭐ Star if helpful!**  
**Made with ❤️ for .NET students**

[![ForTheBadge](https://img.shields.io/badge/Built%20With-%E2%9D%A4%EF%B8%8F-brightpink)](https://github.com/yourusername)

</div>
