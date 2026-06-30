# PleasanterBridge

🌐 **Language**: **English** | [日本語](./README.ja.md)

PleasanterBridge is a .NET library that simplifies API access and direct database access (Select only) to [Pleasanter](https://pleasanter.org/).

> ⚠️ **Provisional Distribution**
> This library is currently in a provisional phase and is intended to be **consumed via a local NuGet repository**.
> It has **not** been published to the public NuGet feed (nuget.org) yet. Please follow the
> [Using a Local NuGet Repository](#-using-a-local-nuget-repository-provisional) steps below to install it.

---

## 🎯 Key Features

### 1. API Bridge (`IPleasanterApiBridge`)
Execute data creation, updates, and deletions using Pleasanter's REST API.

- **Insert**: Insert a single record
- **BulkInsert**: Bulk insert multiple records
- **Update**: Update records (by ID or conditions)
- **Upsert / BulkUpsert**: Insert or update records
- **Delete**: Delete records (by ID or conditions)

**✨ Highlight**: In addition to `Dictionary<string, object>`, every method also accepts **class instances**.

### 2. Database Repository (`IPleasanterRepository`)
Directly access the Pleasanter database via SQL to read data (Select only).

- **Select**: Retrieve data by ReferenceID or SiteID
- **Conditional Select**: Filter using Dictionary-based conditions (supports array matching on Class columns)

---

## 📦 Using a Local NuGet Repository (Provisional)

### Step 1: Build the package

Pack the `PleasanterBridge` project locally as a NuGet package.

```powershell
cd C:\nugetRepo\PleasanterBridge\PleasanterBridge
dotnet pack -c Release -o ./nupkg
```

The generated `PleasanterBridge.1.4.0-local.nupkg` is output to the `PleasanterBridge\nupkg\` folder.

### Step 2: Register the local folder as a NuGet source

```powershell
dotnet nuget add source "C:\nugetRepo\PleasanterBridge\PleasanterBridge\nupkg" --name LocalPleasanterBridge
```

> In Visual Studio, add the folder under __Tools > NuGet Package Manager > Package Manager Settings > Package Sources__.

### Step 3: Install into your project

```powershell
dotnet add package PleasanterBridge --version 1.4.0-local
```

> 💡 Specify the `-local` suffixed version. When updating, rebuild the package and then clear the local cache (`dotnet nuget locals all --clear`) to ensure the latest bits are restored.

---

## 🔍 Transparent Handling of Pleasanter Table Structures

In Pleasanter, a site's data is stored in **three different tables**.

| Table | Primary Key | Purpose |
|----------|-------|------|
| `Results` | `ResultId` | Record table (list-style data management) |
| `Issues` | `IssueId` | Issue table (progress / task management) |
| `Wikis` | `WikiId` | Wiki table (document management) |

**Challenge**: Each table uses a different primary key name and access method, forcing callers to write table-specific code.

PleasanterBridge **absorbs these differences internally** and exposes a **unified interface**, so you can write code without worrying about which table is involved.

```csharp
// Retrieve data with the same code regardless of the table
var resultData = repository.Select(12345);  // Uses ResultId internally
var issueData  = repository.Select(67890);  // Uses IssueId internally
var wikiData   = repository.Select(54321);  // Uses WikiId internally
```

---

## 🔧 Dependencies

| Package | Version | Purpose |
|------------|----------|------|
| `Microsoft.EntityFrameworkCore` | 9.0.11 | Database access |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.11 | SQL Server provider |
| `Microsoft.Data.SqlClient.SNI.runtime` | 5.2.0 | SQL Server native network interface |
| `MySql.EntityFrameworkCore` | 9.0.0 | MySQL provider |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.2 | PostgreSQL provider |
| `Microsoft.Extensions.Http` | 10.0.0 | HTTP client management |

### Target Framework

- **.NET 9.0** (`net9.0`)

### Supported Databases

- **SQL Server** (default)
- **MySQL**
- **PostgreSQL**

---

## 🚀 Usage

Both the API Bridge and Database Repository support **3 initialization methods** each.

### 1️⃣ API Bridge (`IPleasanterApiBridge`)

#### Method A: Pass values directly during instantiation

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;

var httpClient = new HttpClient();

// SQL Server (default)
var bridge = new PleasanterAPIBridge(
    httpClient,
    url: "http://localhost:59802",
    apiKey: "your-api-key",
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// For MySQL / PostgreSQL, specify databaseType
var bridgePostgres = new PleasanterAPIBridge(
    httpClient,
    url: "http://localhost:59802",
    apiKey: "your-api-key",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);
```

#### Method B: Manual DI (pass a Settings instance)

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;

var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

var bridge = new PleasanterAPIBridge(new HttpClient(), settings);
```

#### Method C: Register with a DI container (recommended)

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// Register PleasanterBridge (API + DB) in the DI container
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

### 📖 API Bridge Examples

#### Both Dictionary and class instances are supported

```csharp
// Example data class
public class ResultRecord
{
    public string? Title { get; set; }
    public string? ClassA { get; set; }
    public int? NumA { get; set; }
    public DateTime? DateA { get; set; }
    public bool? CheckA { get; set; }
}
```

**Insert**

```csharp
// Dictionary version
var insertData = new Dictionary<string, object>
{
    { "Title", "New Title" },
    { "ClassA", "Category A" },
    { "NumA", 100 },
    { "DateA", DateTime.Now },
    { "CheckA", true }
};
var response = await bridge.Insert(2826, insertData);

// Class instance version
var record = new ResultRecord { Title = "New Title", ClassA = "Category A", NumA = 100 };
var response2 = await bridge.Insert(2826, record);

Console.WriteLine($"StatusCode: {response.StatusCode}, Id: {response.Id}");
```

**Update by conditions**

```csharp
var targetColumns = new Dictionary<string, object> { { "Status", 100 }, { "ClassA", "Category A" } };
var updateData    = new Dictionary<string, object> { { "Title", "Bulk Updated Title" } };
var responses = await bridge.Update(2826, targetColumns, updateData);
```

**Upsert / Delete**

```csharp
// Upsert
var matchColumns = new List<string> { "ClassA", "ClassB" };
await bridge.Upsert(2826, matchColumns, insertData);

// Delete by ID
await bridge.Delete(12345);

// Delete by conditions
await bridge.Delete(2826, new Dictionary<string, object> { { "Status", 900 } });
```

### 2️⃣ Database Repository (`IPleasanterRepository`)

```csharp
using PleasanterBridge.src.DataRepository.Service;

var repository = new PleasanterRepository(
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// Retrieve by ReferenceID
var byId = repository.Select(67);

// Retrieve by SiteID + conditions (SiteId is automatically added to the filter)
var filtered = repository.Select(2826, new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "Category A" }
});

// Class columns also support array matching (matches if any of the values is present)
var classMatch = repository.Select(2826, new Dictionary<string, object>
{
    { "ClassA", new string[] { "004", "008", "102" } }
});
```

---

## ⚙️ Configuration Classes

### `PleasanterSettings` (API + DB access)

| Property | Type | Description |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API key |
| `Url` | `string` | Pleasanter URL |
| `Server` | `string` | Database server name |
| `Database` | `string` | Database name |
| `UserId` | `string` | Database user ID |
| `Password` | `string` | Database password |
| `DatabaseType` | `DatabaseType` | Database type (SqlServer / MySQL / PostgreSQL) |

### `ConnectionSettings` (DB access only)

Holds `Server` / `Database` / `UserId` / `Password` / `DatabaseType`.

---

## 🧪 Testing

To run the integration tests:

```powershell
cd C:\nugetRepo\PleasanterBridge\Tests
dotnet test
```

The tests require a live Pleasanter instance and DB connection details.
Adjust the connection settings under `Tests/APIBridge/Integration/` and `Tests/DataRepository/Integration/` to match your environment.

---

## 🔗 Related Links

- [Pleasanter Official Site](https://pleasanter.org/)
- [Pleasanter API Documentation](https://pleasanter.org/manual/api)
- [GitHub Repository](https://github.com/vehiclevisionjp/PleasanterBridge)

---

## 📧 Contact

For questions or issues regarding this project, please use GitHub Issues.
