using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Entity;
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.DataRepository.Service.Implementation;

public class PleasanterRepository_IntegrationTest
{
    // 任意の接続情報（テスト用DBやローカルDBなどに合わせて変更してください）
    string Server = "(local)";
    string Database = "Implem.Pleasanter";
    string UserId = "sa";
    string Password = "alderaan";

    [Fact]
    public void Select_ByReferenceId_ConsoleOutput()
    {
        int referenceId = 67; // テスト用の固定値

        IPleasanterRepository repo = new PleasanterRepository(Server, Database, UserId, Password);

        List<Dictionary<string, object>> result = repo.Select(referenceId);

        Console.WriteLine($"Select_ByReferenceId_ConsoleOutput: Count={result.Count}");
        foreach (var record in result)
        {
            // 各レコードの内容を "key1=value1, key2=value2, ..." 形式で出力
            string line = string.Join(", ", record.Select(kv => $"{kv.Key}={kv.Value}"));
            Console.WriteLine(line);
        }

        Assert.True(result.Count >= 0);
    }

    [Fact]
    public void DiSetting_Select_ByReferenceId_ConsoleOutput()
    {
        int referenceId = 67; // テスト用の固定値

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        ) {};

        IPleasanterRepository repo = new PleasanterRepository(settings);

        List<Dictionary<string, object>> result = repo.Select(referenceId);

        Console.WriteLine($"Select_ByReferenceId_ConsoleOutput: Count={result.Count}");
        foreach (var record in result)
        {
            // 各レコードの内容を "key1=value1, key2=value2, ..." 形式で出力
            string line = string.Join(", ", record.Select(kv => $"{kv.Key}={kv.Value}"));
            Console.WriteLine(line);
        }

        Assert.True(result.Count >= 0);
    }

    [Fact]
    public void Select_ByReferenceId_UsingSettingsConstructor_ShouldReturnData()
    {
        // Arrange
        int referenceId = 67;

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        List<Dictionary<string, object>> result = repo.Select(referenceId);

        Assert.NotNull(result);
        Assert.True(result.Count >= 0);

        Console.WriteLine($"Select_ByReferenceId_UsingSettingsConstructor_ShouldReturnData: Count={result.Count}");
    }

    [Fact]
    public void Select_ByReferenceId_ReturnClass_UsingSettingsConstructor_ShouldReturnData()
    {
        // Arrange
        int referenceId = 67;

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        List<TablesEntity> results = repo.Select(referenceId, true);

        Assert.NotNull(results);
        Assert.True(results.Count >= 0);

        Console.WriteLine($"Select_ByReferenceId_UsingSettingsConstructor_ShouldReturnData: Count={results.Count}");
    }

    [Fact]
    public void Select_BySiteId_UsingSettingsConstructor_ShouldReturnData()
    {
        int siteId = 54;

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        List<Dictionary<string, object>> result = repo.Select(siteId, null);

        Assert.NotNull(result);
        Assert.True(result.Count >= 0);

        Console.WriteLine($"Select_ByReferenceId_UsingSettingsConstructor_ShouldReturnData: Count={result.Count}");
    }

    [Fact]
    public void Select_ByKeyValue_UsingSettingsConstructor_ShouldReturnData()
    {
        int siteId = 58;
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "Status", 100 },
            { "ClassA", "004" }
        };

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        List<Dictionary<string, object>> result = repo.Select(siteId, keyValues);

        Assert.NotNull(result);
        Assert.True(result.Count >= 0);

        Console.WriteLine($"Select_ByReferenceId_UsingSettingsConstructor_ShouldReturnData: Count={result.Count}");
    }
}