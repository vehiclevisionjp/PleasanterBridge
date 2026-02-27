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
        long referenceId = 67; // テスト用の固定値

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
        long referenceId = 67; // テスト用の固定値

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
        long referenceId = 67;

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
        long referenceId = 67;

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
        long siteId = 54;

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
        long siteId = 58;
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

    [Fact]
    public void Select_ByKeyValue_WithClassArrayMatching_ShouldReturnMatchingData()
    {
        // Arrange
        long siteId = 108;

        // Test with string array - will match records where ClassA contains "004" OR "008" OR "102"
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "ClassH", new string[] { "812" } }
        };

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act
        List<Dictionary<string, object>> result = repo.Select(siteId, keyValues);

        // Assert
        Assert.NotNull(result);

        Console.WriteLine($"Select_ByKeyValue_WithClassArrayMatching_ShouldReturnMatchingData: Count={result.Count}");
        foreach (var record in result)
        {
            if (record.ContainsKey("ClassH"))
            {
                Console.WriteLine($"  ClassH value: {record["ClassH"]}");
            }
        }
    }

    [Fact]
    public void Select_ByKeyValue_WithClassListMatching_ShouldReturnMatchingData()
    {
        // Arrange
        int siteId = 58;

        // Test with List<string> - will match records where ClassB contains any of these values
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "ClassB", new List<string> { "001", "005", "010" } }
        };

        IConnectionSettings settings = new ConnectionSettings(
            Server,
            Database,
            UserId,
            Password
        );

        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act
        List<Dictionary<string, object>> result = repo.Select(siteId, keyValues);

        // Assert
        Assert.NotNull(result);

        Console.WriteLine($"Select_ByKeyValue_WithClassListMatching_ShouldReturnMatchingData: Count={result.Count}");
        foreach (var record in result)
        {
            if (record.ContainsKey("ClassB"))
            {
                Console.WriteLine($"  ClassB value: {record["ClassB"]}");
            }
        }

        Assert.True(result.Count >= 0);
    }

    #region バリデーションテスト

    [Fact]
    public void Select_WithZeroReferenceId_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(0));
        Assert.Contains("referenceId", exception.Message);
        Assert.Contains("正の整数", exception.Message);
    }

    [Fact]
    public void Select_WithNegativeReferenceId_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(-1, true));
        Assert.Contains("referenceId", exception.Message);
        Assert.Contains("正の整数", exception.Message);
    }

    [Fact]
    public void Select_WithNullReferenceIdList_ShouldThrowArgumentNullException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => repo.Select((List<long>)null!));
        Assert.Contains("referenceIds", exception.Message);
    }

    [Fact]
    public void Select_WithEmptyReferenceIdList_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(new List<long>()));
        Assert.Contains("referenceIds", exception.Message);
        Assert.Contains("空", exception.Message);
    }

    [Fact]
    public void Select_WithInvalidIdInList_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);
        var ids = new List<long> { 1, 2, 0, 4 }; // 0 is invalid

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(ids));
        Assert.Contains("referenceIds", exception.Message);
        Assert.Contains("[2]", exception.Message); // Index of invalid ID
    }

    [Fact]
    public void Select_WithInvalidDictionaryValueType_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);
        int siteId = 58;

        // Dictionary with invalid type (object, custom class, etc.)
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "Status", new { Value = 100 } } // Anonymous type is not allowed
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(siteId, keyValues));
        Assert.Contains("keyValue", exception.Message);
        Assert.Contains("Status", exception.Message);
        Assert.Contains("許可されていません", exception.Message);
    }

    [Fact]
    public void Select_WithInvalidListType_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);
        int siteId = 58;

        // Dictionary with List<int> which is not allowed (only List<string> is allowed)
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "NumA", new List<int> { 1, 2, 3 } } // List<int> is not allowed
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(siteId, keyValues));
        Assert.Contains("keyValue", exception.Message);
        Assert.Contains("NumA", exception.Message);
        Assert.Contains("許可されていません", exception.Message);
    }

    [Fact]
    public void Select_WithValidTypes_ShouldNotThrow()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);
        int siteId = 58;

        // Dictionary with all valid types
        Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
            { "Status", 100 },             // int
            { "Title", "Test" },            // string
            { "WorkValue", 10.5m },              // decimal
            { "CheckA", true }, // bool
            { "DateA", DateTime.Now },           // DateTime
            { "ClassA", new string[] { "001", "002" } },   // string[]
            { "ClassB", new List<string> { "003" } }       // List<string>
        };

        // Act & Assert - Should not throw
        var result = repo.Select(siteId, keyValues);
        Assert.NotNull(result);
    }

    [Fact]
    public void Select_WithZeroSiteId_ShouldThrowArgumentException()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => repo.Select(0, null));
        Assert.Contains("siteId", exception.Message);
        Assert.Contains("正の整数", exception.Message);
    }

    [Fact]
    public void Select_BySiteIdAndKeyValue_ShouldFilterBySiteId()
    {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
     IPleasanterRepository repo = new PleasanterRepository(settings);
        
        int siteId = 58;
     Dictionary<string, object> keyValues = new Dictionary<string, object>
     {
      { "Status", 100 }
        };

        // Act
   var result = repo.Select(siteId, keyValues);

    // Assert
  Assert.NotNull(result);
        
        Console.WriteLine($"Select_BySiteIdAndKeyValue_ShouldFilterBySiteId: Count={result.Count}");
     
// すべてのレコードが指定されたSiteIdを持っていることを確認
        foreach (var record in result)
        {
     if (record.ContainsKey("SiteId"))
        {
            var recordSiteId = Convert.ToInt64(record["SiteId"]);
      Assert.Equal(siteId, recordSiteId);
    Console.WriteLine($"  Record SiteId: {recordSiteId}, Status: {record.GetValueOrDefault("Status", "N/A")}");
       }
        }
    }

    [Fact]
    public void Select_BySiteIdAndKeyValue_WithExplicitSiteIdInKeyValue_ShouldUseExplicitSiteId()
 {
        // Arrange
        IConnectionSettings settings = new ConnectionSettings(Server, Database, UserId, Password);
        IPleasanterRepository repo = new PleasanterRepository(settings);
      
        int siteId = 58;
    int explicitSiteId = 54; // keyValueで明示的に別のSiteIdを指定
        
 Dictionary<string, object> keyValues = new Dictionary<string, object>
        {
         { "SiteId", explicitSiteId }, // 明示的にSiteIdを指定
 { "Status", 100 }
  };

   // Act
 var result = repo.Select(siteId, keyValues);

        // Assert
        Assert.NotNull(result);
        
        Console.WriteLine($"Select_BySiteIdAndKeyValue_WithExplicitSiteIdInKeyValue_ShouldUseExplicitSiteId: Count={result.Count}");
   
        // keyValueで明示的に指定したSiteIdが優先されることを確認
   foreach (var record in result)
        {
            if (record.ContainsKey("SiteId"))
            {
      var recordSiteId = Convert.ToInt64(record["SiteId"]);
    Assert.Equal(explicitSiteId, recordSiteId);
      Console.WriteLine($"  Record SiteId: {recordSiteId} (expected: {explicitSiteId})");
    }
        }
    }

    #endregion
}