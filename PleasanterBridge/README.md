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

**✨ New Feature**: All methods now accept not only `Dictionary<string, object>`, but also **class instances**!

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

#### **💡 Supports Both Dictionary and Class Instances**

PleasanterBridge API Bridge methods can accept both traditional `Dictionary<string, object>` and **any class instances**.

**Example Data Class Definitions:**

```csharp
public class ResultRecord
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? ClassA { get; set; }
    public string? ClassB { get; set; }
    public int? NumA { get; set; }
    public string? DateA { get; set; }
    public bool? CheckA { get; set; }
}

public class TargetFilter
{
    public int? Status { get; set; }
    public string? ClassA { get; set; }
}

public class UpdateData
{
  public string? Title { get; set; }
    public string? Body { get; set; }
}
```

#### **Insert Record**

**Dictionary Version:**
```csharp
var insertData = new Dictionary<string, object>
{
  { "Title", "New Title" },
    { "Body", "Body Content" },
    { "ClassA", "Category A" },
    { "NumA", 100 },
    { "DateA", "2025/01/01" },
    { "CheckA", true }
};

var response = await bridge.Insert("2826", insertData);
```

**Class Instance Version:**
```csharp
var insertData = new ResultRecord
{
    Title = "New Title",
    Body = "Body Content",
    ClassA = "Category A",
    NumA = 100,
    DateA = "2025/01/01",
    CheckA = true
};

var response = await bridge.Insert("2826", insertData);
Console.WriteLine($"StatusCode: {response.StatusCode}, Id: {response.Id}");
```

#### **Bulk Insert**

**Class Instance Version:**
```csharp
var dataList = new List<ResultRecord>
{
    new() { Title = "Data 1", ClassA = "Category 1", NumA = 1 },
new() { Title = "Data 2", ClassA = "Category 2", NumA = 2 }
};

var response = await bridge.BulkInsert("2826", dataList);
```

#### **Update Record (by ID)**

**Class Instance Version:**
```csharp
var updateData = new UpdateData
{
    Title = "Updated Title",
    Body = "Updated Body"
};

var response = await bridge.Update("12345", updateData);
```

#### **Update Record (by Conditions)**

**Class Instance Version:**
```csharp
var targetColumns = new TargetFilter
{
    Status = 100,
    ClassA = "Category A"
};

var updateData = new UpdateData
{
    Title = "Bulk Updated Title"
};

var responses = await bridge.Update("2826", targetColumns, updateData);
```

#### **Upsert (Insert or Update)**

**Class Instance Version:**
```csharp
var upsertData = new ResultRecord
{
    Title = "Upsert Data",
    ClassA = "Category A",
    ClassB = "Category B",
    NumA = 500
};

var matchColumns = new List<string> { "ClassA", "ClassB" };
var response = await bridge.Upsert("2826", matchColumns, upsertData);
```

#### **Delete Record (by ID)**

```csharp
var response = await bridge.Delete("12345");
```

#### **Delete Record (by Conditions)**

**Class Instance Version:**
```csharp
var targetColumns = new TargetFilter
{
  Status = 900,
    ClassA = "To Delete"
};

var responses = await bridge.Delete("2826", targetColumns);
```

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

---

using PleasanterBridge.src.APIBridge.Entity;

namespace PleasanterBridge.src.Utilities
{
    internal class TypedDataValidator
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        private static readonly Type[] AllowedTypes =
        {
            typeof(string),
            typeof(bool),
            typeof(double),
            typeof(decimal),
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(ImageEntity)
        };

        public static void Validate(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                if (kvp.Value == null) continue;
                Type valueType = kvp.Value.GetType();
                if (!AllowedTypes.Contains(valueType))
                {
                    throw new ArgumentException(
                        $"unsupported type '{kvp.Key}' : {valueType.Name}"
                    );
                }
            }
        }

        public static void ValidateList(List<Dictionary<string, object>> dataList)
        {
            foreach (var data in dataList)
            {
                Validate(data);
            }
        }
    }
}