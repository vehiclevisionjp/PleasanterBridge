# PleasanterBridge

PleasanterBridge は、[Pleasanter](https://pleasanter.org/) への API アクセスとデータベース直接アクセス（Select専用）を簡易化する .NET ライブラリです。

## 🎯 主な機能

### 1. **API ブリッジ (`IPleasanterApiBridge`)**
Pleasanter の REST API を利用してデータの作成・更新・削除を実行します。

- **Insert**: 単一レコードの挿入
- **BulkInsert**: 複数レコードの一括挿入
- **Update**: レコードの更新（ID指定 / 条件指定）
- **Upsert / BulkUpsert**: レコードの挿入または更新
- **Delete**: レコードの削除（ID指定 / 条件指定）

**✨ 新機能**: 各メソッドは `Dictionary<string, object>` だけでなく、**クラスインスタンス**も受け取れるようになりました！

### 2. **データベースリポジトリ (`IPleasanterRepository`)**
Pleasanter データベースへ直接 SQL アクセスし、データを読み取ります（Select専用）。

- **Select**: ReferenceID または SiteID でデータを取得
- **条件付き Select**: Dictionary 形式で条件を指定してフィルタリング

---

## 🔍 Pleasanter テーブル構造の透過的な処理

### Pleasanter のテーブル構造について

Pleasanter では、Site（サイト）に対するデータが以下の **3つの異なるテーブル** に格納されます：

| テーブル名 | 主キー | 用途 |
|----------|-------|------|
| `Results` | `ResultId` | 記録テーブル（一覧形式のデータ管理） |
| `Issues` | `IssueId` | 課題テーブル（進捗管理・タスク管理） |
| `Wikis` | `WikiId` | Wikiテーブル（ドキュメント管理） |

**問題点:**
- 各テーブルで **主キーの名前が異なる**（`ResultId`, `IssueId`, `WikiId`）
- **アクセス方法が異なる**（各テーブルに対して別々の処理が必要）
- `Results` と `Issues` は、Pleasanter の UI 上ではほぼ同じように見えるが、**内部的には別テーブル**

このため、ユーザーがテーブルごとに異なるコードを書く必要があり、開発が煩雑になります。

### PleasanterBridge の解決方法

PleasanterBridge は、これらのテーブルの差異を **内部で自動的に吸収** し、ユーザーが **統一的なインターフェース** でアクセスできるようにします。

#### ✅ 1. 参照タイプの自動判別

内部的に `PleasanterReferenceType` という列挙型を使用してテーブルを識別します：

```csharp
public enum PleasanterReferenceType
{
    Sites,
    Results,
    Issues,
Wikis
}
```

`GetReferenceProcess` が、指定された ID（SiteID または ReferenceID）から **自動的にテーブルを判別** します。

#### ✅ 2. ID プロパティ名の統一

各テーブルの主キー名が異なる問題を、`PleasanterIdPropertyName` で吸収します：

```csharp
internal enum PleasanterIdPropertyName
{
    ResultId,
    IssueId,
    WikiId
}
```

内部的には適切な ID 名を使用しますが、**ユーザーは意識する必要がありません**。

#### ✅ 3. ファクトリーパターンによる動的処理の切り替え

`DbProcessFactory` が、参照タイプに応じて **適切なエンティティとID名を自動選択** します：

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

### 実際の利用例：透過的なアクセス

以下のコードは、**どのテーブルに対しても同じ方法で動作** します：

```csharp
// Results テーブルのデータを取得（内部的に ResultId を使用）
var resultData = repository.Select("12345");

// Issues テーブルのデータを取得（内部的に IssueId を使用）
var issueData = repository.Select("67890");

// Wikis テーブルのデータを取得（内部的に WikiId を使用）
var wikiData = repository.Select("54321");

// 条件指定も同様に統一されたインターフェース
var filteredData = repository.Select("2826", new Dictionary<string, object>
{
{ "Status", 100 },
    { "ClassA", "分類A" }
});
```

### API ブリッジでの透過的な処理

API アクセスでも、条件指定による Update や Delete で同じ仕組みが使われます：

```csharp
// GetTargetRecordIds メソッド内で自動的にテーブルを判別
var targetColumns = new Dictionary<string, object>
{
    { "Status", 100 },
  { "ClassA", "完了" }
};

// Results テーブルでも Issues テーブルでも同じコードで動作
var responses = await bridge.Update("2826", targetColumns, updateData);
```

内部的には：
1. SiteID から参照タイプを判別（`Results` or `Issues` or `Wikis`）
2. 適切なID名（`ResultId`, `IssueId`, `WikiId`）を自動選択
3. 該当レコードを取得して処理

**ユーザーは、どのテーブルを使用しているかを意識せずに、統一されたコードを書くことができます。**

---

## 📦 インストール

### NuGet パッケージからインストール（予定）

```bash
dotnet add package PleasanterBridge
```

### ローカルビルド＆パッケージ化

プロジェクトをローカルで NuGet パッケージ化してテストする場合：

```cmd
cd PleasanterBridge
dotnet pack -c Release -o ./nupkg
```

または、同梱の `build.bat` を実行：

```cmd
.\build.bat
```

生成されたパッケージは `PleasanterBridge\nupkg\` フォルダに出力されます。

---

## 🔧 依存関係

このライブラリは以下の NuGet パッケージに依存しています：

| パッケージ名 | バージョン | 用途 |
|------------|----------|------|
| `Microsoft.EntityFrameworkCore` | 9.0.11 | データベースアクセス |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.11 | SQL Server プロバイダー |
| `MySql.EntityFrameworkCore` | 9.0.0 | MySQL プロバイダー |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.2 | PostgreSQL プロバイダー |
| `Microsoft.Extensions.Http` | 10.0.0 | HTTP クライアント管理 |

### ターゲットフレームワーク

- **.NET 9.0** (`net9.0`)

### サポートされているデータベース

- **SQL Server** (デフォルト)
- **MySQL**
- **PostgreSQL**

---

## 🚀 使い方

PleasanterBridge は、**API ブリッジ**と**データベースリポジトリ**の 2 つの機能を提供しており、それぞれ **3 つの初期化方法** をサポートしています。

---

## 1️⃣ API ブリッジ (`IPleasanterApiBridge`)

### 📌 初期化方法

#### **方法A: インスタンス化時に直接値を渡す**

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;

var httpClient = new HttpClient();

// SQL Server (デフォルト)
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

#### **方法B: 手動 DI（Settings インスタンスを渡す）**

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.Configuration;

// PostgreSQL を使用する例
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

#### **方法C: DI コンテナに登録（推奨）**

**`Program.cs` での設定:**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 設定情報を作成（PostgreSQL を使用する例）
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
    password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);

// PleasanterBridge を DI コンテナに登録
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

**コントローラーやサービスでの利用:**

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
    { "Title", "テストタイトル" },
      { "Body", "テスト本文" },
  { "ClassA", "分類A" }
  };

    var response = await _bridge.Insert("2826", data);
        Console.WriteLine($"Insert: {response.StatusCode}");
    }
}
```

---

### 📖 API ブリッジの使用例

#### **💡 Dictionary と クラスインスタンスの両方に対応**

PleasanterBridge の API ブリッジメソッドは、従来の `Dictionary<string, object>` に加えて、**任意のクラスインスタンス**も受け取ることができます。

**データクラスの定義例:**

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

#### **レコードの挿入**

**Dictionary版:**
```csharp
var insertData = new Dictionary<string, object>
{
    { "Title", "新規タイトル" },
  { "Body", "本文内容" },
    { "ClassA", "分類A" },
    { "NumA", 100 },
    { "DateA", "2025/01/01" },
    { "CheckA", true }
};

var response = await bridge.Insert("2826", insertData);
```

**クラスインスタンス版:**
```csharp
var insertData = new ResultRecord
{
    Title = "新規タイトル",
    Body = "本文内容",
    ClassA = "分類A",
  NumA = 100,
    DateA = "2025/01/01",
    CheckA = true
};

var response = await bridge.Insert("2826", insertData);
Console.WriteLine($"StatusCode: {response.StatusCode}, Id: {response.Id}");
```

#### **一括挿入**

**クラスインスタンス版:**
```csharp
var dataList = new List<ResultRecord>
{
    new() { Title = "データ1", ClassA = "分類1", NumA = 1 },
    new() { Title = "データ2", ClassA = "分類2", NumA = 2 }
};

var response = await bridge.BulkInsert("2826", dataList);
```

#### **レコードの更新（ID指定）**

**クラスインスタンス版:**
```csharp
var updateData = new UpdateData
{
    Title = "更新後タイトル",
 Body = "更新後本文"
};

var response = await bridge.Update("12345", updateData);
```

#### **レコードの更新（条件指定）**

**クラスインスタンス版:**
```csharp
var targetColumns = new TargetFilter
{
    Status = 100,
    ClassA = "分類A"
};

var updateData = new UpdateData
{
    Title = "一括更新タイトル"
};

var responses = await bridge.Update("2826", targetColumns, updateData);
```

#### **Upsert（挿入または更新）**

**クラスインスタンス版:**
```csharp
var upsertData = new ResultRecord
{
    Title = "Upsertデータ",
    ClassA = "分類A",
    ClassB = "分類B",
 NumA = 500
};

var matchColumns = new List<string> { "ClassA", "ClassB" };
var response = await bridge.Upsert("2826", matchColumns, upsertData);
```

#### **レコードの削除（ID指定）**

```csharp
var response = await bridge.Delete("12345");
```

#### **レコードの削除（条件指定）**

**クラスインスタンス版:**
```csharp
var targetColumns = new TargetFilter
{
    Status = 900,
 ClassA = "削除対象"
};

var responses = await bridge.Delete("2826", targetColumns);
```

---

## 2️⃣ データベースリポジトリ (`IPleasanterRepository`)

### 📌 初期化方法

#### **方法A: インスタンス化時に直接値を渡す**

```csharp
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.Configuration;

// SQL Server (デフォルト)
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

#### **方法B: 手動 DI（Settings インスタンスを渡す）**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Service;

// PostgreSQL を使用する例
var settings = new ConnectionSettings(
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "postgres",
  password: "your-password",
    databaseType: DatabaseType.PostgreSQL
);

var repository = new PleasanterRepository(settings);
```

#### **方法C: DI コンテナに登録（推奨）**

**`Program.cs` での設定:**

```csharp
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.APIBridge.Configuration;

var builder = WebApplication.CreateBuilder(args);

// MySQL を使用する例
var settings = new PleasanterSettings(
    apiKey: "your-api-key",
    url: "http://localhost:59802",
    server: "localhost",
    database: "Implem.Pleasanter",
    userId: "root",
    password: "your-password",
    databaseType: DatabaseType.MySQL
);

// PleasanterBridge（API + DB）を DI コンテナに登録
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

**コントローラーやサービスでの利用:**

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

### 📖 データベースリポジトリの使用例

---

## ⚙️ 設定クラス

### **`IPleasanterSettings` / `PleasanterSettings`**

API アクセスと DB アクセスの両方に必要な設定を管理します。

| プロパティ名 | 型 | 説明 |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API キー |
| `Url` | `string` | Pleasanter の URL |
| `Server` | `string` | データベースサーバー名 |
| `Database` | `string` | データベース名 |
| `UserId` | `string` | データベースユーザーID |
| `Password` | `string` | データベースパスワード |
| `DatabaseType` | `DatabaseType` | データベースタイプ (SqlServer/MySQL/PostgreSQL) |

### **`IConnectionSettings` / `ConnectionSettings`**

DB アクセス専用の設定を管理します。

| プロパティ名 | 型 | 説明 |
|------------|------|------|
| `Server` | `string` | データベースサーバー名 |
| `Database` | `string` | データベース名 |
| `UserId` | `string` | データベースユーザーID |
| `Password` | `string` | データベースパスワード |
| `DatabaseType` | `DatabaseType` | データベースタイプ (SqlServer/MySQL/PostgreSQL) |

### **`DatabaseType` Enum**

サポートされているデータベースタイプ：

```csharp
public enum DatabaseType
{
    SqlServer,  // デフォルト
    MySQL,
    PostgreSQL
}
```

---

## 📂 プロジェクト構成

// ...existing code...

---

## 🧪 テスト

// ...existing code...

---

## 📝 ライセンス

このプロジェクトのライセンスについては、プロジェクトオーナーにお問い合わせください。

---

## 🤝 コントリビューション

バグ報告や機能追加の提案は、GitHub の Issue またはプルリクエストで受け付けています。

---

## 🔗 関連リンク

- [Pleasanter 公式サイト](https://pleasanter.org/)
- [Pleasanter API ドキュメント](https://pleasanter.org/manual/api)
- [GitHub リポジトリ](https://github.com/TakutoOsugi/PleasanterBridge)

---

## 📧 お問い合わせ

プロジェクトに関する質問や問題がある場合は、GitHub の Issue をご利用ください。
