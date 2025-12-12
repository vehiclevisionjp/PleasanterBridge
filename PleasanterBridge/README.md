# PleasanterBridge

PleasanterBridge is a .NET library that simplifies API access and direct database access (Select only) to [Pleasanter](https://pleasanter.org/).

## 🎯 Key Features

### 1. **API Bridge (`IPleasanterApiBridge`)**
Execute data creation, updates, and deletions using Pleasanter's REST API.

- **Insert**: Insert a single record
- **BulkInsert**: Bulk insert multiple records
- **Update**: Update records (by ID or conditions)
- **Upsert / BulkUpsert**: Insert or update records
- **Delete**: Delete records (by ID or conditions)

### 2. **Database Repository (`IPleasanterRepository`)**
Direct SQL access to the Pleasanter database for reading data (Select only).

- **Select**: Retrieve data by ReferenceID or SiteID
- **Conditional Select**: Filter data using Dictionary-based conditions

---

## 🔍 Transparent Handling of Pleasanter Table Structures

// ...existing code about table structures...

---

## 📦 Installation

### Install from NuGet Package (planned)

```bash
dotnet add package PleasanterBridge
```

### Local Build & Packaging

To package the project locally as a NuGet package for testing:

```cmd
cd PleasanterBridge
dotnet pack -c Release -o ./nupkg
```

Or run the included `build.bat`:

```cmd
.\build.bat
```

The generated package will be output to the `PleasanterBridge\nupkg\` folder.

---

## 🔧 Dependencies

This library depends on the following NuGet packages:

| Package Name | Version | Purpose |
|------------|----------|------|
| `Microsoft.EntityFrameworkCore` | 9.0.11 | Database access |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.11 | SQL Server provider |
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

PleasanterBridge provides two features: **API Bridge** and **Database Repository**, each supporting **3 initialization methods**.

---

## 1️⃣ API Bridge (`IPleasanterApiBridge`)

### 📌 Initialization Methods

#### **Method A: Pass Values Directly During Instantiation**

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;

var httpClient = new HttpClient();

// SQL Server (default)
var bridge = new PleasanterAPIBridge(
 httpClient,
    pleasanterUrl: "http://localhost:59802",
  apiKey: "your-api-key",
  server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// MySQL
var bridgeMySQL = new PleasanterAPIBridge(
    httpClient,
    pleasanterUrl: "http://localhost:59802",
    apiKey: "your-api-key",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "root",
    password: "your-password",
    databaseType: DatabaseType.MySQL
);

// PostgreSQL
var bridgePostgreSQL = new PleasanterAPIBridge(
    httpClient,
    pleasanterUrl: "http://localhost:59802",
    apiKey: "your-api-key",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
 databaseType: DatabaseType.PostgreSQL
);
```

#### **Method B: Manual DI (Pass Settings Instance)**

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.Configuration;

// Example using PostgreSQL
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);

var httpClient = new HttpClient();
var bridge = new PleasanterAPIBridge(httpClient, settings);
```

#### **Method C: Register with DI Container (Recommended)**

**Configuration in `Program.cs`:**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Create configuration (example using PostgreSQL)
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
  server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
  password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);

// Register PleasanterBridge in DI container
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

**Usage in Controllers or Services:**

```csharp
using PleasanterBridge.src.APIBridge;

public class MyService
{
    private readonly IPleasanterApiBridge _bridge;

public MyService(IPleasanterApiBridge bridge)
    {
        _bridge = bridge;
    }

    public async Task InsertData()
    {
        var data = new Dictionary<string, object>
  {
            { "Title", "Test Title" },
 { "Body", "Test Body" },
      { "ClassA", "Category A" }
        };

        var response = await _bridge.Insert("2826", data);
     Console.WriteLine($"Insert: {response.StatusCode}");
    }
}
```

---

### 📖 API Bridge Usage Examples

// ...existing code...

---

## 2️⃣ Database Repository (`IPleasanterRepository`)

### 📌 Initialization Methods

#### **Method A: Pass Values Directly During Instantiation**

```csharp
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.Configuration;

// SQL Server (default)
var repository = new PleasanterRepository(
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// MySQL
var repositoryMySQL = new PleasanterRepository(
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "root",
    password: "your-password",
    databaseType: DatabaseType.MySQL
);

// PostgreSQL
var repositoryPostgreSQL = new PleasanterRepository(
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);
```

#### **Method B: Manual DI (Pass Settings Instance)**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Service;

// Example using PostgreSQL
var settings = new ConnectionSettings(
    server: "localhost",
  database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);

var repository = new PleasanterRepository(settings);
```

#### **Method C: Register with DI Container (Recommended)**

**Configuration in `Program.cs`:**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Example using MySQL
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
  url: "http://localhost:59802",
  server: "localhost",
    database: "Implem.Pleasanter",
    userId: "root",
    password: "your-password",
    databaseType: DatabaseType.MySQL
);

// Register PleasanterBridge (API + DB) in DI container
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

**Usage in Controllers or Services:**

```csharp
using PleasanterBridge.src.DataRepository.Service;

public class MyService
{
 private readonly IPleasanterRepository _repository;

    public MyService(IPleasanterRepository repository)
    {
        _repository = repository;
    }

    public void GetData()
    {
   var result = _repository.Select("67");
    foreach (var record in result)
      {
    Console.WriteLine(string.Join(", ", record));
        }
    }
}
```

---

### 📖 Database Repository Usage Examples

// ...existing code...

---

## ⚙️ Configuration Classes

### **`IPleasanterSettings` / `PleasanterSettings`**

Manages settings required for both API access and DB access.

| Property Name | Type | Description |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API key |
| `Url` | `string` | Pleasanter URL |
| `Server` | `string` | Database server name |
| `Database` | `string` | Database name |
| `UserId` | `string` | Database user ID |
| `Password` | `string` | Database password |
| `DatabaseType` | `DatabaseType` | Database type (SqlServer/MySQL/PostgreSQL) |

### **`IConnectionSettings` / `ConnectionSettings`**

Manages settings for DB access only.

| Property Name | Type | Description |
|------------|------|------|
| `Server` | `string` | Database server name |
| `Database` | `string` | Database name |
| `UserId` | `string` | Database user ID |
| `Password` | `string` | Database password |
| `DatabaseType` | `DatabaseType` | Database type (SqlServer/MySQL/PostgreSQL) |

### **`DatabaseType` Enum**

Supported database types:

```csharp
public enum DatabaseType
{
    SqlServer,  // Default
    MySQL,
    PostgreSQL
}
```

---

## 📂 Project Structure

// ...existing code...

---

## 🧪 Testing

// ...existing code...

---

## 📝 License

For licensing information about this project, please contact the project owner.

---

## 🤝 Contribution

Bug reports and feature suggestions are welcome via GitHub Issues or Pull Requests.

---

## 🔗 Related Links

- [Pleasanter Official Site](https://pleasanter.org/)
- [Pleasanter API Documentation](https://pleasanter.org/manual/api)
- [GitHub Repository](https://github.com/TakutoOsugi/PleasanterBridge)

---

## 📧 Contact

If you have questions or issues regarding the project, please use GitHub Issues.