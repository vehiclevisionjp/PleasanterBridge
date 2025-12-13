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

### About Pleasanter Table Structure

In Pleasanter, data for Sites is stored in **3 different tables**:

| Table Name | Primary Key | Purpose |
|----------|-------|------|
| `Results` | `ResultId` | Records table (list-based data management) |
| `Issues` | `IssueId` | Issues table (progress management / task management) |
| `Wikis` | `WikiId` | Wiki table (document management) |

**Problems:**
- **Different primary key names** for each table (`ResultId`, `IssueId`, `WikiId`)
- **Different access methods** (requires separate processing for each table)
- `Results` and `Issues` look almost identical in the Pleasanter UI, but are **internally separate tables**

This requires users to write different code for each table, making development cumbersome.

### PleasanterBridge's Solution

PleasanterBridge **automatically absorbs these table differences internally**, allowing users to access them through a **unified interface**.

#### ✅ 1. Automatic Reference Type Detection

Internally uses the `PleasanterReferenceType` enum to identify tables:

```csharp
public enum PleasanterReferenceType
{
    Sites,
    Results,
    Issues,
    Wikis
}
```

`GetReferenceProcess` **automatically determines the table** from the specified ID (SiteID or ReferenceID).

#### ✅ 2. Unified ID Property Names

Handles the different primary key names with `PleasanterIdPropertyName`:

```csharp
internal enum PleasanterIdPropertyName
{
    ResultId,
    IssueId,
    WikiId
}
```

Internally uses the appropriate ID name, but **users don't need to be aware of it**.

#### ✅ 3. Dynamic Processing Switching via Factory Pattern

`DbProcessFactory` **automatically selects the appropriate entity and ID name** based on the reference type:

```csharp
public IDbExecutionProcess Create(PleasanterReferenceType type)
{
    return type switch
    {
        PleasanterReferenceType.Results =>
            new GetDbBaseProcess<ResultsEntity>(_settings, PleasanterIdPropertyName.ResultId),

        PleasanterReferenceType.Issues =>
            new GetDbBaseProcess<IssuesEntity>(_settings, PleasanterIdPropertyName.IssueId),

        PleasanterReferenceType.Wikis =>
     new GetDbBaseProcess<WikisEntity>(_settings, PleasanterIdPropertyName.WikiId),

        _ => throw new InvalidOperationException($"Unsupported reference type: {type}")
    };
}
```

### Real-world Example: Transparent Access

The following code **works the same way for all tables**:

```csharp
// Retrieve data from Results table (internally uses ResultId)
var resultData = repository.Select("12345");

// Retrieve data from Issues table (internally uses IssueId)
var issueData = repository.Select("67890");

// Retrieve data from Wikis table (internally uses WikiId)
var wikiData = repository.Select("54321");

// Conditional selection also uses a unified interface
var filteredData = repository.Select("2826", new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "CategoryA" }
});
```

### Transparent Processing in API Bridge

The same mechanism is used for conditional Update and Delete operations in API access:

```csharp
// Automatically determines the table within GetTargetRecordIds method
var targetColumns = new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "Completed" }
};

// Works with the same code for both Results and Issues tables
var responses = await bridge.Update("2826", targetColumns, updateData);
```

Internally:
1. Determine reference type from SiteID (`Results` or `Issues` or `Wikis`)
2. Automatically select appropriate ID name (`ResultId`, `IssueId`, `WikiId`)
3. Retrieve and process the relevant records

**Users can write unified code without being aware of which table is being used.**

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
| `Microsoft.Extensions.Http` | 10.0.0 | HTTP client management |

### Target Framework

- **.NET 9.0** (`net9.0`)

---

## 🚀 Usage

PleasanterBridge provides two features: **API Bridge** and **Database Repository**, each supporting **3 initialization methods**.

---

## 1️⃣ API Bridge (`IPleasanterApiBridge`)

### 📌 Initialization Methods

#### **Method A: Pass Values Directly During Instantiation**

```csharp
using PleasanterBridge.src.APIBridge;

var httpClient = new HttpClient();
var bridge = new PleasanterAPIBridge(
 httpClient,
    pleasanterUrl: "your-pleasanter-url",
    apiKey: "your-api-key",
    server: "(local)",
    database: "Implem.Pleasanter(default)",
    userId: "sa(default)",
    password: "your-password"
);
```

#### **Method B: Manual DI (Pass Settings Instance)**

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;

var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "your-pleasanter-url",
    server: "(local)",
    database: "Implem.Pleasanter(default)",
    userId: "sa(default)",
    password: "your-password"
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

// Create configuration
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "your-pleasanter-url",
    server: "(local)",
    database: "Implem.Pleasanter(default)",
    userId: "sa(default)",
    password: "your-password"
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

#### **Insert Record**

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
Console.WriteLine($"StatusCode: {response.StatusCode}, Id: {response.Id}");
```

#### **Bulk Insert**

```csharp
var dataList = new List<Dictionary<string, object>>
{
    new() { { "Title", "Data 1" }, { "ClassA", "Category 1" } },
    new() { { "Title", "Data 2" }, { "ClassA", "Category 2" } }
};

var response = await bridge.BulkInsert("2826", dataList);
```

#### **Update Record (by ID)**

```csharp
var updateData = new Dictionary<string, object>
{
    { "Title", "Updated Title" },
    { "ClassA", "Updated Category" }
};

var response = await bridge.Update("12345", updateData);
```

#### **Update Record (by Conditions)**

```csharp
var targetColumns = new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "Category A" }
};

var updateData = new Dictionary<string, object>
{
  { "Title", "Bulk Updated Title" }
};

var responses = await bridge.Update("2826", targetColumns, updateData);
```

#### **Upsert (Insert or Update)**

```csharp
var upsertData = new Dictionary<string, object>
{
    { "Title", "Upsert Data" },
    { "ClassA", "Category A" },
    { "ClassB", "Category B" }
};

var matchColumns = new List<string> { "ClassA", "ClassB" };
var response = await bridge.Upsert("2826", matchColumns, upsertData);
```

#### **Delete Record (by ID)**

```csharp
var response = await bridge.Delete("12345");
```

#### **Delete Record (by Conditions)**

```csharp
var targetColumns = new Dictionary<string, object>
{
    { "Status", 900 },
    { "ClassA", "To Delete" }
};

var responses = await bridge.Delete("2826", targetColumns);
```

---

## 2️⃣ Database Repository (`IPleasanterRepository`)

### 📌 Initialization Methods

#### **Method A: Pass Values Directly During Instantiation**

```csharp
using PleasanterBridge.src.DataRepository.Service;

var repository = new PleasanterRepository(
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);
```

#### **Method B: Manual DI (Pass Settings Instance)**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Service;

var settings = new ConnectionSettings(
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

var repository = new PleasanterRepository(settings);
```

#### **Method C: Register with DI Container (Recommended)**

**Configuration in `Program.cs`:**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "your-pleasanter-url",
    server: "(local)",
    database: "Implem.Pleasanter(default)",
    userId: "sa(default)",
    password: "your-password"
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

#### **Retrieve Data by ReferenceID**

```csharp
var result = repository.Select("67");

foreach (var record in result)
{
    Console.WriteLine($"Title: {record["Title"]}, Status: {record["Status"]}");
}
```

#### **Retrieve Data by SiteID**

```csharp
var result = repository.Select("2826", keyValue: null);
```

#### **Retrieve Data with Conditions**

```csharp
var keyValues = new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "Category A" }
};

var result = repository.Select("2826", keyValues);
```

---

## ⚙️ Configuration Classes

### **`IPleasanterSettings` / `PleasanterSettings`**

Manages settings required for both API access and DB access.

| Property Name | Type | Description |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API key |
| `Url` | `string` | Pleasanter URL |
| `Server` | `string` | SQL Server name |
| `Database` | `string` | Database name |
| `UserId` | `string` | Database user ID |
| `Password` | `string` | Database password |

### **`IConnectionSettings` / `ConnectionSettings`**

Manages settings for DB access only.

| Property Name | Type | Description |
|------------|------|------|
| `Server` | `string` | SQL Server name |
| `Database` | `string` | Database name |
| `UserId` | `string` | Database user ID |
| `Password` | `string` | Database password |

---

## 📂 Project Structure

```
PleasanterBridge/
├── src/
│   ├── APIBridge/    # API access related
│   │   ├── Configuration/      # Configuration classes
│   │   ├── Contracts/    # API requests/responses
│   │   ├── Exception/      # API exceptions
││   ├── Factory/  # Factory classes
│   │   ├── Mapping/         # Mapping processes
│   │ └── Utilities/        # Utilities
│   ├── DataRepository/      # DB access related
│   │   ├── Data/               # DB context
│   │   ├── DbProcess/# DB processes
│   │   ├── Entity/         # Entities
│   │   ├── Service/# Service layer
│   │   └── Utilities/       # Utilities
│   ├── Configuration/     # Common configuration
│   └── Utilities/              # Common utilities
├── Tests/               # Integration tests
├── PleasanterBridge.csproj     # Project file
├── build.bat     # Build script
└── README.md         # This file
```

---

## 🧪 Testing

To run integration tests:

```bash
cd Tests
dotnet test
```

Tests require an actual Pleasanter instance and DB connection information.  
Modify the connection information in `Tests/APIBridge/Integration/PleasanterAPIBridge_IntegrationTest.cs` and  
`Tests/DataRepository/Integration/PleasanterRepository_IntegrationTest.cs` according to your environment.

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
