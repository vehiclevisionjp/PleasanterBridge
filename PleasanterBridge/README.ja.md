# PleasanterBridge

🌐 **言語**: **日本語** | [English](./README.md)

PleasanterBridge は、[Pleasanter](https://pleasanter.org/) への API アクセスとデータベース直接アクセス（Select のみ）を簡素化する .NET ライブラリです。

> ⚠️ **暫定運用について**
> 本ライブラリは現在 **ローカルの NuGet リポジトリ経由で利用** することを前提とした暫定運用フェーズです。
> 公式 NuGet (nuget.org) への公開は未実施のため、後述の [ローカル NuGet リポジトリでの利用](#-ローカル-nuget-リポジトリでの利用暫定運用) の手順に従って導入してください。

---

## 🎯 主な機能

### 1. API Bridge (`IPleasanterApiBridge`)
Pleasanter の REST API を利用して、データの作成・更新・削除を実行します。

- **Insert**: 単一レコードの登録
- **BulkInsert**: 複数レコードの一括登録
- **Update**: レコードの更新（ID 指定または条件指定）
- **Upsert / BulkUpsert**: レコードの登録または更新
- **Delete**: レコードの削除（ID 指定または条件指定）

**✨ 特徴**: すべてのメソッドは `Dictionary<string, object>` に加え、**クラスインスタンス**も受け取れます。

### 2. Database Repository (`IPleasanterRepository`)
Pleasanter データベースへ直接 SQL アクセスし、データを読み取ります（Select のみ）。

- **Select**: ReferenceID または SiteID によるデータ取得
- **条件付き Select**: Dictionary ベースの条件によるフィルタリング（Class カラムの配列マッチ対応）

---

## 📦 ローカル NuGet リポジトリでの利用（暫定運用）

### ステップ 1: パッケージのビルド

`PleasanterBridge` プロジェクトをローカルで NuGet パッケージ化します。

```powershell
cd C:\nugetRepo\PleasanterBridge\PleasanterBridge
dotnet pack -c Release -o ./nupkg
```

生成された `PleasanterBridge.1.3.2-local.nupkg` が `PleasanterBridge\nupkg\` フォルダに出力されます。

### ステップ 2: ローカルフォルダを NuGet ソースとして登録

```powershell
dotnet nuget add source "C:\nugetRepo\PleasanterBridge\PleasanterBridge\nupkg" --name LocalPleasanterBridge
```

> Visual Studio から登録する場合は、__ツール > NuGet パッケージ マネージャー > パッケージ マネージャー設定 > パッケージ ソース__ で上記フォルダを追加してください。

### ステップ 3: 利用したいプロジェクトへインストール

```powershell
dotnet add package PleasanterBridge --version 1.3.2-local
```

> 💡 `-local` サフィックス付きのバージョンを指定してください。更新時はパッケージ再ビルド後、ローカルキャッシュをクリア（`dotnet nuget locals all --clear`）すると確実です。

---

## 🔍 Pleasanter テーブル構造の透過的な処理

Pleasanter では、サイトのデータが **3 種類のテーブル** に格納されます。

| テーブル名 | 主キー | 用途 |
|----------|-------|------|
| `Results` | `ResultId` | 記録テーブル（一覧形式のデータ管理） |
| `Issues` | `IssueId` | 課題テーブル（進捗・タスク管理） |
| `Wikis` | `WikiId` | Wiki テーブル（ドキュメント管理） |

**課題**: テーブルごとに主キー名やアクセス方法が異なり、利用者が個別にコードを書く必要があります。

PleasanterBridge は **これらの差異を内部で自動的に吸収** し、**統一されたインターフェース** でアクセスできるようにします。利用者はどのテーブルかを意識せずにコードを書けます。

```csharp
// テーブルの違いを意識せず、同じコードで取得できる
var resultData = repository.Select(12345);  // 内部で ResultId を利用
var issueData  = repository.Select(67890);  // 内部で IssueId を利用
var wikiData   = repository.Select(54321);  // 内部で WikiId を利用
```

---

## 🔧 依存パッケージ

| パッケージ名 | バージョン | 用途 |
|------------|----------|------|
| `Microsoft.EntityFrameworkCore` | 9.0.11 | データベースアクセス |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.11 | SQL Server プロバイダー |
| `MySql.EntityFrameworkCore` | 9.0.0 | MySQL プロバイダー |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.2 | PostgreSQL プロバイダー |
| `Microsoft.Extensions.Http` | 10.0.0 | HTTP クライアント管理 |

### ターゲットフレームワーク

- **.NET 9.0** (`net9.0`)

### 対応データベース

- **SQL Server**（デフォルト）
- **MySQL**
- **PostgreSQL**

---

## 🚀 使い方

API Bridge と Database Repository の両機能が、それぞれ **3 通りの初期化方法** をサポートします。

### 1️⃣ API Bridge (`IPleasanterApiBridge`)

#### 方法 A: インスタンス生成時に値を直接渡す

```csharp
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;

var httpClient = new HttpClient();

// SQL Server（デフォルト）
var bridge = new PleasanterAPIBridge(
    httpClient,
    url: "http://localhost:59802",
    apiKey: "your-api-key",
    server: "(local)",
    database: "Implem.Pleasanter",
    userId: "sa",
    password: "your-password"
);

// MySQL / PostgreSQL の場合は databaseType を指定
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

#### 方法 B: 手動 DI（Settings インスタンスを渡す）

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

#### 方法 C: DI コンテナへ登録（推奨）

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

// PleasanterBridge（API + DB）を DI コンテナへ登録
builder.Services.AddPleasanterBridge(settings);

var app = builder.Build();
app.Run();
```

### 📖 API Bridge 使用例

#### Dictionary 版とクラスインスタンス版の両対応

```csharp
// データクラスの例
public class ResultRecord
{
    public string? Title { get; set; }
    public string? ClassA { get; set; }
    public int? NumA { get; set; }
    public DateTime? DateA { get; set; }
    public bool? CheckA { get; set; }
}
```

**登録（Insert）**

```csharp
// Dictionary 版
var insertData = new Dictionary<string, object>
{
    { "Title", "新規タイトル" },
    { "ClassA", "カテゴリA" },
    { "NumA", 100 },
    { "DateA", DateTime.Now },
    { "CheckA", true }
};
var response = await bridge.Insert(2826, insertData);

// クラスインスタンス版
var record = new ResultRecord { Title = "新規タイトル", ClassA = "カテゴリA", NumA = 100 };
var response2 = await bridge.Insert(2826, record);

Console.WriteLine($"StatusCode: {response.StatusCode}, Id: {response.Id}");
```

**条件指定の更新（Update）**

```csharp
var targetColumns = new Dictionary<string, object> { { "Status", 100 }, { "ClassA", "カテゴリA" } };
var updateData    = new Dictionary<string, object> { { "Title", "一括更新タイトル" } };
var responses = await bridge.Update(2826, targetColumns, updateData);
```

**Upsert / 削除（Delete）**

```csharp
// Upsert
var matchColumns = new List<string> { "ClassA", "ClassB" };
await bridge.Upsert(2826, matchColumns, insertData);

// ID 指定で削除
await bridge.Delete(12345);

// 条件指定で削除
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

// ReferenceID で取得
var byId = repository.Select(67);

// SiteID + 条件で取得（SiteId は自動的に検索条件へ付与されます）
var filtered = repository.Select(2826, new Dictionary<string, object>
{
    { "Status", 100 },
    { "ClassA", "カテゴリA" }
});

// Class カラムは配列マッチにも対応（いずれかを含めばヒット）
var classMatch = repository.Select(2826, new Dictionary<string, object>
{
    { "ClassA", new string[] { "004", "008", "102" } }
});
```

---

## ⚙️ 設定クラス

### `PleasanterSettings`（API + DB アクセス用）

| プロパティ名 | 型 | 説明 |
|------------|------|------|
| `ApiyKey` | `string` | Pleasanter API キー |
| `Url` | `string` | Pleasanter の URL |
| `Server` | `string` | データベースサーバー名 |
| `Database` | `string` | データベース名 |
| `UserId` | `string` | データベースユーザー ID |
| `Password` | `string` | データベースパスワード |
| `DatabaseType` | `DatabaseType` | DB 種別（SqlServer / MySQL / PostgreSQL） |

### `ConnectionSettings`（DB アクセス専用）

`Server` / `Database` / `UserId` / `Password` / `DatabaseType` を保持します。

---

## 🧪 テスト

統合テストを実行するには:

```powershell
cd C:\nugetRepo\PleasanterBridge\Tests
dotnet test
```

テストには実際の Pleasanter インスタンスと DB 接続情報が必要です。
`Tests/APIBridge/Integration/` および `Tests/DataRepository/Integration/` の接続情報を環境に合わせて変更してください。

---

## 🔗 関連リンク

- [Pleasanter 公式サイト](https://pleasanter.org/)
- [Pleasanter API ドキュメント](https://pleasanter.org/manual/api)
- [GitHub リポジトリ](https://github.com/vehiclevisionjp/PleasanterBridge)

---

## 📧 お問い合わせ

プロジェクトに関する質問や問題は GitHub Issues をご利用ください。
