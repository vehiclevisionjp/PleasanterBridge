# はじめに

以前、Pleasanterのテーブル構造やAPI仕様を意識することなく、統一的なインターフェースでデータを操作できるNuGetパッケージ「PleasanterBridge」を作成したことをご紹介いたしました。

あれから機能追加と整備を進め、初回記事で「今後の予定」としていた項目の多くを実装・対応いたしましたので、改めて現在のPleasanterBridgeをご紹介いたします。

特に、初回記事で「作業中」としていた **画像ファイルの操作対応** が実装完了し、API経由での画像登録、データベース経由での画像取得が可能になりました。

# パッケージ概要

PleasanterBridge は、Pleasanterのデータベースおよび REST APIを簡潔に扱うための .NET ライブラリです。Pleasanter 独自の仕様（複数のテーブル構造、複雑なコンテナ形式）をカプセル化し、ユーザーは統一されたメソッドで操作することができます。

- 対象フレームワーク: .NET 9
- C# バージョン: 13.0
- 現在のバージョン: 1.4.0（ローカルNuGetリポジトリでの暫定運用フェーズ）

# 初回リリースからの進化

初回記事の公開以降、以下の対応を行いました。

| 項目 | 初回記事時点 | 現在 |
|------|------------|------|
| 画像ファイルの操作 | 作業中 | **対応完了**（API登録・DB取得） |
| README / ドキュメント | 今後の予定 | **整備完了**（日本語・英語） |
| ID 指定の型 | `string` | **`long` に統一** |
| データベース取得（Select） | ID・条件指定の基本形 | **ID一覧・エンティティ返却・配列マッチに拡張** |
| 導入方法 | DI 登録が中心 | **3通りの初期化方法を整理** |
| 配布形態 | 準備中 | **ローカルNuGetリポジトリで暫定運用** |

# PleasanterBridgeの特徴

#### ReferenceTypeの自動判別

Pleasanterでは、同じサイトのデータが Results / Issues / Wikis という3つの異なるテーブルに格納されます。PleasanterBridgeは、これらのテーブル間の差異を内部で吸収し、ユーザーは ReferenceType を意識することなく統一的なインターフェースでアクセス可能になります。

| テーブル名 | 主キー | 用途 |
|----------|-------|------|
| `Results` | `ResultId` | 記録テーブル（一覧形式のデータ管理） |
| `Issues` | `IssueId` | 課題テーブル（進捗・タスク管理） |
| `Wikis` | `WikiId` | Wiki テーブル（ドキュメント管理） |

```csharp
// テーブルの違いを意識せず、同じコードで取得できる
var resultData = repository.Select(12345);  // 内部で ResultId を利用
var issueData  = repository.Select(67890);  // 内部で IssueId を利用
var wikiData   = repository.Select(54321);  // 内部で WikiId を利用
```

#### 複雑なコンテナ形式が不要

Pleasanterの API では `ClassHash: { ClassA: valueA, ... }` といった独自のコンテナ構造を作成する必要があります。PleasanterBridge では、すべてのカラムと値を単純な `Dictionary<string, object>` の `{ Key: Value }` または、ジェネリック（クラスのインスタンス）形式で渡すだけで、内部で自動的に正しいコンテナに変換されます。

今回のアップデートで、画像用の `ImageHash` も自動生成の対象に加わりました。内部で生成されるコンテナは以下の通りです。

- `ClassHash`（文字列）
- `NumHash`（数値）
- `DateHash`（日付）
- `DescriptionHash`（説明）
- `CheckHash`（チェックボックス）
- `ImageHash`（画像）★今回追加

#### セーフティな入力処理

入力値に誤りがあった場合も安全に処理されます。

- カラム名を誤ってしまった場合、そのキーは自動的に無視されます
- 異なる ReferenceType のカラムを誤って渡した場合も、マッチしないものは無視され、エラーにはなりません
- 事前に検証されたカラム名のみが処理対象となり、予期しない動作を防ぎます

#### 柔軟な導入方法

**settingの登録**
サーバ接続設定などのsetting受け渡しに推奨される使用方法は、ASP.NET Core のDI（依存性注入コンテナ）への登録となっております。
しかし、DI を使わない環境では、`IPleasanterSettings` インターフェース経由でインスタンス化したり、機能のインスタンス化の際に直接設定値を渡すことも可能です。

API Bridge / Database Repository ともに、次の **3通りの初期化方法** をサポートしています。

- 方法A: インスタンス生成時に値を直接渡す
- 方法B: 手動DI（Settings インスタンスを渡す）
- 方法C: DI コンテナへ登録（推奨／`AddPleasanterBridge`）

**複数のデータベースに対応**
PleasanterBridge は、Pleasanter が対応する以下のすべてのデータベースをサポートしており、ユーザーはデータベース間の差異を意識する必要はありません。

- SQL Server (デフォルト)
- MySQL
- PostgreSQL

#### 柔軟なデータ受け渡し方式

データの受け渡し方を、利用シーンに応じて選択できます。

- Dictionary 方式: 簡易的に `Dictionary<string, object>` で値を渡す
- ジェネリック方式: 正確にカラム名を固定したクラスのインスタンスを渡す

どちらの方式も内部的には同じ処理パスを通るため、シームレスに選択・切り替えが可能です。

# 機能別の実装概要

### API 操作 (PleasanterAPIBridge)

Pleasanter の REST API と HTTP 通信を行い、データの新規作成・更新・削除を実行します。

**主要メソッド**

- Insert(): 単一レコードの新規作成
- BulkInsert(): 複数レコードの一括新規作成
- Update(): レコード ID による更新、または条件指定による複数レコード更新
- Upsert() / BulkUpsert(): マッチカラムを指定した更新挿入
- Delete(): レコード ID による削除、または条件指定による複数レコード削除

**内部処理**

- リクエストをPleasanterAPIの仕様に合わせて変換
- ClassHash・NumHash・DateHash・DescriptionHash・CheckHash・ImageHash といった複数のコンテナを自動生成
- レスポンスの統一的なハンドリング

> 💡 今回のアップデートで、ID を指定する引数（`siteId` / `referenceId`）の型を `string` から `long` に統一しました。Pleasanter の ID は数値であるため、型の取り違えを防ぎ、より自然に扱えるようになっています。

**Interface**

```csharp
// Dictionary版メソッド
public Task<PleasanterApiResponse> Insert(long siteId, Dictionary<string, object> dataRecord);
public Task<PleasanterApiResponse> BulkInsert(long siteId, List<Dictionary<string, object>> dataRecords);
public Task<PleasanterApiResponse> Update(long referenceId, Dictionary<string, object> updateData);
public Task<List<PleasanterApiResponse>> Update(long siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData);
public Task<PleasanterApiResponse> Upsert(long siteId, List<string> matchColumns, Dictionary<string, object> dataRecord);
public Task<PleasanterApiResponse> BulkUpsert(long siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords);
public Task<PleasanterApiResponse> Delete(long referenceId);
public Task<List<PleasanterApiResponse>> Delete(long siteId, Dictionary<string, object> targetColumns);

// ジェネリック版メソッド
public Task<PleasanterApiResponse> Insert<T>(long siteId, T dataRecord) where T : class;
public Task<PleasanterApiResponse> BulkInsert<T>(long siteId, List<T> dataRecords) where T : class;
public Task<PleasanterApiResponse> Update<T>(long referenceId, T updateData) where T : class;
public Task<List<PleasanterApiResponse>> Update<TTarget, TUpdate>(long siteId, TTarget targetColumns, TUpdate updateData)
	where TTarget : class
	where TUpdate : class;
public Task<PleasanterApiResponse> Upsert<T>(long siteId, List<string> matchColumns, T dataRecord) where T : class;
public Task<PleasanterApiResponse> BulkUpsert<T>(long siteId, List<string> matchColumns, List<T> dataRecords) where T : class;
public Task<List<PleasanterApiResponse>> Delete<T>(long siteId, T targetColumns) where T : class;
```

### 画像操作（新機能）

初回記事で「作業中」としていた画像対応が実装完了しました。**API経由での画像登録** と **データベース経由での画像取得** の双方に対応しています。

#### API経由での画像登録

画像は Base64 文字列を `ImageEntity` でラップし、`Image` キーに `Dictionary<string, ImageEntity>` を渡すだけで登録できます。内部で自動的に `ImageHash` コンテナへ変換されます。

```csharp
// Base64 画像（例として 1x1 png）
string base64Image = "iVBORw0KGgoAAAANSUhEUgAAAA...";

var insertData = new Dictionary<string, object>
{
	{ "Title", "画像付きレコード" },
	{ "Body", "画像登録のテスト" },
	{ "Image", new Dictionary<string, ImageEntity>
		{
			{ "ImageA", new ImageEntity(base64Image) },
			// 複数画像も登録可能
			// { "ImageB", new ImageEntity(base64Image2) }
		}
	}
};

var response = await bridge.Insert(siteId, insertData);
```

`ImageEntity` では、Base64 データに加えて、表示位置や代替テキスト、拡張子などをオプションで指定できます。

```csharp
public ImageEntity(
	string base64,
	bool headNewLine = false,
	bool endNewLine = false,
	int position = -1,
	string alt = "image",
	string extension = ".png"
);
```

#### データベース経由での画像取得

Pleasanter の本文（Markdown）には、画像が `![image](/binaries/{guid}/show)` という参照形式で埋め込まれます。PleasanterBridge は、この参照を検出し、`Binaries` テーブルから実体（バイナリ）を取得して **Base64 文字列へ自動変換** します。

- 本文テキストを解析し、画像参照部分とテキスト部分をセグメントに分割
- 同一 GUID の画像はキャッシュし、無駄な取得を抑制
- 取得したバイナリは Base64 文字列として返却

これにより、REST API を経由せずに、本文中の画像を含めたコンテンツを扱えるようになります。

### データベース取得 (IPleasanterRepository)

Pleasanter のデータベースに直接接続し、SQL を実行してデータを取得します。

**主要機能**

- Select(): ReferenceId（単一・複数）や SiteId + 条件でレコードを検索
- ReferenceType の自動判別に基づく正しいテーブルへの SQL 実行
- `Dictionary` 形式に加え、エンティティ（`TablesEntity`）形式での返却にも対応
- Class カラムの **配列マッチ**（いずれかを含めばヒット）に対応

- 利点
	- REST API では取得できない複雑なクエリも実行可能
	- 直接データベースアクセスのため、API の制限を受けない
	- Pleasanterを経由しないので高速な動作が可能
- 欠点:
	- Pleasanterを経由しないため、アクセスログが残らない
	- テーブルへのアクセス設定を渡す必要がある（PleasanterBridge全体の制約となります）

**Interface**

```csharp
// ReferenceId 指定（単一）
public List<Dictionary<string, object>> Select(long id);
public List<TablesEntity> Select(long id, bool isReturnEntity = false);

// ReferenceId 指定（複数）
public List<Dictionary<string, object>> Select(List<long> referenceIds);
public List<TablesEntity> Select(List<long> referenceIds, bool isReturnEntity = false);

// SiteId + 条件指定
public List<Dictionary<string, object>> Select(long siteId, Dictionary<string, object>? keyValue);
public List<TablesEntity> Select(long siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false);
```

**使用例**

```csharp
// ReferenceId で取得
var byId = repository.Select(67);

// SiteId + 条件で取得（SiteId は自動的に検索条件へ付与されます）
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

### 設定ファイル(IPleasanterSettings)

**Interface**

```csharp
string ApiyKey { get; set; }
string Url { get; set; }
string Server { get; set; }
string Database { get; set; }
string UserId { get; set; }
string Password { get; set; }
DatabaseType DatabaseType { get; set; }
```

# 導入方法（ローカルNuGetリポジトリでの暫定運用）

現在は公式 NuGet（nuget.org）への公開前のため、ローカルの NuGet リポジトリ経由での利用を前提とした暫定運用フェーズです。

```powershell
# 1. パッケージのビルド
dotnet pack -c Release -o ./nupkg

# 2. ローカルフォルダを NuGet ソースとして登録
dotnet nuget add source "C:\path\to\PleasanterBridge\nupkg" --name LocalPleasanterBridge

# 3. 利用したいプロジェクトへインストール
dotnet add package PleasanterBridge --version 1.4.0-local
```

### 初期化方法（3パターン）

**方法A: インスタンス生成時に値を直接渡す**

```csharp
var bridge = new PleasanterAPIBridge(
	httpClient,
	url: "http://localhost:59802",
	apiKey: "your-api-key",
	server: "(local)",
	database: "Implem.Pleasanter",
	userId: "sa",
	password: "your-password"
);
```

**方法B: 手動DI（Settings インスタンスを渡す）**

```csharp
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

**方法C: DI コンテナへ登録（推奨）**

```csharp
var settings = new PleasanterSettings(
	apiKey: "your-api-key",
	url: "http://localhost:59802",
	server: "(local)",
	database: "Implem.Pleasanter",
	userId: "sa",
	password: "your-password"
);

// API + DB をまとめて DI コンテナへ登録
builder.Services.AddPleasanterBridge(settings);
```

# 実装例

```csharp
// Dictionary 方式での新規作成
var targetSiteId = 100L;
var data = new Dictionary<string, object>
{
	{ "Title", "新規タスク" },
	{ "Body", "タスクの説明" },
	{ "Class_A", "分類A" }
};
var result = await bridge.Insert(targetSiteId, data);

var targetReferenceId = 101L;

// ジェネリック方式での更新
var updateData = new TaskUpdateRequest
{
	Title = "更新後のタスク",
	ClassA = "分類A Updated"
};
await bridge.Update<TaskUpdateRequest>(targetReferenceId, updateData);
```

# おわりに

初回記事で「今後の予定」としていた項目のうち、**画像ファイルの操作対応** と **README / ドキュメントの整備** を完了し、あわせて API の型統一やデータベース取得機能の拡張を行いました。

現在実装済みの機能については、正しく稼働することを確認しています。
今後のアップデート予定は以下の通りです。

- エラー処理およびログ出力の更なる充実
- 画像以外の添付ファイル（AttachmentHash）への対応
- 使用方法ドキュメントの拡充
- NuGet（nuget.org）への公開

公式 NuGet への公開準備が整い次第、改めてご案内いたします。ぜひご利用ください。
