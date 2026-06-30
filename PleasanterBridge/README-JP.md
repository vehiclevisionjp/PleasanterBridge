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

**インストール手順：**

1. [Visual C++ Redistributable (x64)](https://aka.ms/vs/17/release/vc_redist.x64.exe) をダウンロード
2. サーバーにインストール
3. サーバーを再起動
4. アプリケーションを再デプロイして実行

> **注意:** この要件は Windows Server（例：Windows Server 2022）へのデプロイ時に適用されます。MySQL および PostgreSQL プロバイダーにはこの依存関係はありません。

---

## 🔧 依存関係

このライブラリは以下の NuGet パッケージに依存しています：

| パッケージ名 | バージョン | 用途 |
|------------|----------|------|
| `Microsoft.EntityFrameworkCore` | 9.0.11 | データベースアクセス |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.11 | SQL Server プロバイダー |
| `Microsoft.Data.SqlClient.SNI.runtime` | 5.2.0 | SQL Server ネイティブネットワークインターフェース |
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
    apiKey: "your-api-key",
    server: "(local)",
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

```csharp
// データの準備
var data = new Dictionary<string, object>
{
    { "Title", "New Task" },
    { "Description", "詳細な説明" },
    { "DueDate", DateTime.UtcNow.AddDays(7) },
    { "Status", "未着手" },
    { "ClassA", "重要" }
};

// タスクの追加
var addResponse = await bridge.Insert("2826", data);
Console.WriteLine($"Add Task: {addResponse.StatusCode}");

// 複数タスクの一括追加
var bulkData = new List<Dictionary<string, object>>
{
    new Dictionary<string, object>
    {
        { "Title", "Task 1" },
        { "Description", "Description 1" },
        { "DueDate", DateTime.UtcNow.AddDays(5) },
        { "Status", "進行中" },
        { "ClassA", "通常" }
    },
    new Dictionary<string, object>
    {
        { "Title", "Task 2" },
        { "Description", "Description 2" },
        { "DueDate", DateTime.UtcNow.AddDays(10) },
        { "Status", "未着手" },
        { "ClassA", "低" }
    }
};

// 一括追加実行
var bulkInsertResponse = await bridge.BulkInsert("2826", bulkData);
Console.WriteLine($"Bulk Insert Tasks: {bulkInsertResponse.StatusCode}");

// タスクの更新
var updateData = new Dictionary<string, object>
{
    { "Status", "完了" },
    { "ClassA", "分類A" }
};

// 条件指定での一括更新
var updateResponse = await bridge.Update("2826", updateData, new Dictionary<string, object>
{
    { "Status", "進行中" }
});
Console.WriteLine($"Update Tasks: {updateResponse.StatusCode}");

// 条件指定での削除
var deleteResponse = await bridge.Delete("2826", new Dictionary<string, object>
{
    { "Status", "完了" }
});
Console.WriteLine($"Delete Completed Tasks: {deleteResponse.StatusCode}");
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
    database: "Implem.Pleasanter",
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

```csharp
// データの取得
var selectResult = repository.Select("123");
foreach (var record in selectResult)
{
    Console.WriteLine($"ResultId: {record["ResultId"]}, Title: {record["Title"]}");
}

// 条件付きでのデータ取得
var conditionResult = repository.Select("2826", new Dictionary<string, object>
{
    { "Status", "未着手" },
    { "ClassA", "重要" }
});

foreach (var record in conditionResult)
{
    Console.WriteLine($"Filtered - ResultId: {record["ResultId"]}, Title: {record["Title"]}");
}
```

---

## ⚙️ 設定クラス

### **`IPleasanterSettings` / `PleasanterSettings`**

API アクセスと DB アクセスの両方に必要な設定を管理します。

| プロパティ名 | 型 | 説明 |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API キー |
| `Url` | `string` | Pleasanter の URL |
| `Database` | `string` | データベース名 |
| `UserId` | `string` | データベースユーザーID |
| `Password` | `string` | データベースパスワード |
| `DatabaseType` | `DatabaseType` | データベースタイプ (SqlServer/MySQL/PostgreSQL) |

### **`IConnectionSettings` / `ConnectionSettings`**

DB アクセス専用の設定を管理します。

| プロパティ名 | 型 | 説明 |
|------------|------|------|
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


---

## 🧪 テスト


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
