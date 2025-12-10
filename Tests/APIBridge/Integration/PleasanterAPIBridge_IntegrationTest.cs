using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Service;

public class PleasanterAPIBridge_IntegrationTest
{
    string ApiKey = "75b2c4f1c9bdc67b618b2318a487e9a7db22c4f62fc725737aa03fa080ef6722519615f706338c213b91048ea2262cd97637bf7f63d6f9c98b95a947b12b5410";
    string PleasanterUrl = "http://localhost:59802";
    string Server = "(local)";
    string Database = "Implem.Pleasanter";
    string UserId = "sa";
    string Password = "alderaan";
    string SiteId = "2826"; // テスト用サイトID

    private IPleasanterApiBridge CreateBridge()
    {
        var httpClient = new HttpClient();
        return new PleasanterAPIBridge(
            httpClient,
            PleasanterUrl,
            ApiKey,
            Server,
            Database,
            UserId,
            Password
        );
    }

    [Fact]
    public async Task Insert_Update_Delete_IntegrationTest()
    {
        var bridge = CreateBridge();

        // 1. Insert
        var insertData = new Dictionary<string, object>
        {
            { "Title", "IntegrationTest Title" },
            { "Body", "IntegrationTest Body" },
            { "ClassA", "IntegrationTest ClassA" }
        };
        var insertResponse = await bridge.Insert(SiteId, insertData);
        Console.WriteLine($"Insert: Status={insertResponse.StatusCode}, Id={insertResponse.Id}, Message={insertResponse.Message}");

        Assert.True(insertResponse.StatusCode == 200 || insertResponse.StatusCode == 201);

        // 2. Update
        var updateData = new Dictionary<string, object>
        {
            { "Title", "IntegrationTest Title Updated" },
            { "ClassA", "IntegrationTest ClassA Updated" }
        };
        var updateResponse = await bridge.Update(insertResponse.Id.ToString(), updateData);
        Console.WriteLine($"Update: Status={updateResponse.StatusCode}, Id={updateResponse.Id}, Message={updateResponse.Message}");

        Assert.True(updateResponse.StatusCode == 200 || updateResponse.StatusCode == 201);

        // 3. Delete
        var deleteResponse = await bridge.Delete(insertResponse.Id.ToString());
        Console.WriteLine($"Delete: Status={deleteResponse.StatusCode}, Id={deleteResponse.Id}, Message={deleteResponse.Message}");

        Assert.True(deleteResponse.StatusCode == 200 || deleteResponse.StatusCode == 204);
    }

    [Fact]
    public async Task BulkInsert_BulkUpsert_IntegrationTest()
    {
        var bridge = CreateBridge();

        var dataList = new List<Dictionary<string, object>>
        {
            new() { 
                { "Title", "BulkTest1" },
                { "Body", "BulkBody1" },
                { "ClassA", "BulkClassA1" },
                { "ClassB", "BulkClassB1" },
                { "NumA", 1 },
                { "DateA", "2019/01/01" },
                { "DescriptionA", "DA1" }, 
                { "CheckA", true }
            },
            new() {
                { "Title", "BulkTest2" }, 
                { "Body", "BulkBody2" }, 
                { "ClassA", "BulkClassA2" },
                { "ClassB", "BulkClassB2" },
                { "NumA", 2 }, 
                { "DateA", "2025/01/01" }, 
                { "DescriptionA", "DA2" }, 
                { "CheckA", false } 
            }
        };

        var bulkInsertResponse = await bridge.BulkInsert(SiteId, dataList);
        Console.WriteLine($"BulkInsert: Status={bulkInsertResponse.StatusCode}, Message={bulkInsertResponse.Message}");

        Assert.True(bulkInsertResponse.StatusCode == 200 || bulkInsertResponse.StatusCode == 201);

        var matchColumns = new List<string> { "ClassA" };
        var updateDataList = new List<Dictionary<string, object>>
        {
            new() { { "Title", "BulkTest1 Updated" }, { "ClassA", "BulkClassA1" }, { "NumA", 10 } },
            new() { { "Title", "BulkTest2 Updated" }, { "ClassA", "BulkClassA2" }, { "NumA", 20 } }
        };
        var bulkUpsertResponse = await bridge.BulkUpsert(SiteId, matchColumns, updateDataList);
        Console.WriteLine($"BulkUpsert: Status={bulkUpsertResponse.StatusCode}, Message={bulkUpsertResponse.Message}");

        Assert.True(bulkUpsertResponse.StatusCode == 200 || bulkUpsertResponse.StatusCode == 201);
    }

    [Fact]
    public async Task Upsert_IntegrationTest()
    {
        var bridge = CreateBridge();

        var upsertData = new Dictionary<string, object>
        {
            { "Title", "UpsertTest" },
            { "Body", "UpsertBody" },
            { "ClassA", "UpsertClassA" },
            { "ClassB", "UpsertClassB" },
            { "NumA", 1 },
            { "DateA", "2019/01/01" },
            { "DescriptionA", "DA1" },
            { "CheckA", true }
        };
        var matchColumns = new List<string> { "ClassA", "ClassB" };

        var upsertResponse = await bridge.Upsert(SiteId, matchColumns, upsertData);
        Console.WriteLine($"Upsert: Status={upsertResponse.StatusCode}, Message={upsertResponse.Message}");

        Assert.True(upsertResponse.StatusCode == 200 || upsertResponse.StatusCode == 201);
    }
}