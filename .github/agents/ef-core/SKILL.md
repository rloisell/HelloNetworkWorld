---
name: ef-core
description: Entity Framework Core patterns for .NET projects using Pomelo MariaDB provider — migration workflow, startup auto-migrate, primary constructors, service layer structure, and the Linux LINQ ReadOnlySpan overload bug. Use when adding migrations, setting up DbContext, structuring services, writing LINQ queries, or troubleshooting EF Core issues.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: .NET 10 + EF Core + Pomelo.EntityFrameworkCore.MySql + MariaDB.
---

# EF Core Agent

Guides correct Entity Framework Core usage with the Pomelo MariaDB provider.

---

## Provider Setup

```
NuGet packages:
  Microsoft.EntityFrameworkCore
  Pomelo.EntityFrameworkCore.MySql
  Microsoft.EntityFrameworkCore.Design  ← local dev only; exclude from runtime image
```

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=<project>_dev;User=<user>;Password=<pass>;AllowPublicKeyRetrieval=true;"
  }
}
```

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
```

---

## Entity Conventions

```csharp
public class MyEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### DbContext (primary constructor)

```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<MyEntity> MyEntities => Set<MyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
} // end ApplicationDbContext
```

---

## Migration Workflow

```bash
# Add
dotnet ef migrations add <MigrationName> --project src/<Project>.Api --startup-project src/<Project>.Api

# Apply
dotnet ef database update --project src/<Project>.Api --startup-project src/<Project>.Api

# List status
dotnet ef migrations list --project src/<Project>.Api

# Rollback to migration
dotnet ef database update <PreviousMigrationName> --project src/<Project>.Api

# Remove last un-applied file (does not touch DB)
dotnet ef migrations remove --project src/<Project>.Api

# Wipe schema — dangerous
dotnet ef database update 0 --project src/<Project>.Api
```

### Naming Conventions

| Scenario | Name |
|----------|------|
| Initial schema | `InitialCreate` |
| Add table | `Add<Entity>Table` |
| Add column | `Add<Column>To<Table>` |
| Remove column | `Remove<Column>From<Table>` |
| Add index | `Add<Field>IndexTo<Table>` |
| Refactor | `<Description>Refactor` |
| Seed change | `Seed<Entity>Data` |

---

## Startup Auto-Migrate

**ALWAYS `db.Database.Migrate()`. NEVER `EnsureCreated()`.**

`EnsureCreated()` creates the schema without the `__EFMigrationsHistory` table —
permanently blocking future migrations on that database.

```csharp
// Program.cs — after app build, before app.Run()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

---

## Linux LINQ ReadOnlySpan Gotcha

**Silent bug on Linux that does NOT appear on macOS development.**

```csharp
// ❌ BROKEN on Linux — compiles, throws at EF Core runtime
var results = db.Entities
    .Where(e => new[] { "active", "pending" }.Contains(e.Status))
    .ToList();

// ✅ CORRECT — declare as List<string> explicitly
var statuses = new List<string> { "active", "pending" };
var results = db.Entities
    .Where(e => statuses.Contains(e.Status))
    .ToList();
```

**Rule:** Always use `List<string>` (not `string[]`) for collections used in
LINQ `.Contains()` against EF Core queries.

---

## Service Layer Pattern

Controllers are thin — all logic in scoped services behind interfaces.

```csharp
// IMyService.cs
public interface IMyService
{
    Task<MyDto[]> GetAllAsync();
    Task<MyDto> GetByIdAsync(Guid id);       // throws NotFoundException
    Task<MyDto> CreateAsync(CreateRequest r);
    Task<MyDto> UpdateAsync(Guid id, UpdateRequest r);
    Task DeleteAsync(Guid id);
}

// MyService.cs
public class MyService(ApplicationDbContext db) : IMyService
{
    // ── QUERY ─────────────────────────────────────────────────────────────

    // returns all entities sorted by name
    public async Task<MyDto[]> GetAllAsync() =>
        await db.MyEntities
            .OrderBy(e => e.Name)
            .Select(e => new MyDto { Id = e.Id, Name = e.Name })
            .ToArrayAsync();

    // throws NotFoundException if not found
    public async Task<MyDto> GetByIdAsync(Guid id) =>
        await db.MyEntities
            .Where(e => e.Id == id)
            .Select(e => new MyDto { Id = e.Id, Name = e.Name })
            .FirstOrDefaultAsync()
        ?? throw new NotFoundException($"MyEntity {id} not found");
} // end MyService

// Program.cs registration
builder.Services.AddScoped<IMyService, MyService>();
```

### Domain Exceptions (→ RFC 7807 ProblemDetails)

| Exception | HTTP Status |
|-----------|------------|
| `NotFoundException` | 404 |
| `ForbiddenException` | 403 |
| `BadRequestException` | 400 |
| `UnauthorizedException` | 401 |

---

## Testing

```csharp
// In-memory DB for unit tests (not real MariaDB)
services.AddDbContext<ApplicationDbContext>(o =>
    o.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
```

Never test against real MariaDB from CI unit tests — use in-memory for portability.

---

## EF_KNOWLEDGE

> Append new EF Core discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [HelloNetworkWorld] Pomelo TCP connection on macOS Homebrew MariaDB — socket path auth not needed with password-authenticated user. Connection uses `Server=localhost;Port=3306;`.
- 2026-02-27: [HelloNetworkWorld] `db.Database.Migrate()` in Program.cs — InitialCreate migration applied cleanly on `hnw_dev` DB on first run.
- 2026-02-27: [DSC-modernization] Quartz.NET background job scheduler requires its own `AddQuartz()` DI extension. Keep Quartz config separate from EF Core registration to avoid Program.cs bloat.
