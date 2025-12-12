using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.Configuration;

namespace Tests.APIBridge.Integration
{
    // テスト用のデータクラス
    public class TestDataRecord
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? ClassA { get; set; }
        public string? ClassB { get; set; }
        public int? NumA { get; set; }
        public string? DateA { get; set; }
        public string? DescriptionA { get; set; }
        public bool? CheckA { get; set; }
    }

    public class TestTargetColumns
    {
        public int? Status { get; set; }
        public string? ClassA { get; set; }
    }

    public class TestUpdateData
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
    }

    public class PleasanterAPIBridge_GenericMethods_IntegrationTest
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
        public async Task Insert_WithGenericType_Success()
        {
            var bridge = CreateBridge();

            // クラスインスタンスを使用
            var insertData = new TestDataRecord
            {
                Title = "Generic Insert Test",
                Body = "Testing generic insert method",
                ClassA = "GenericClassA",
                NumA = 100
            };

            var response = await bridge.Insert(SiteId, insertData);
            Console.WriteLine($"Generic Insert: Status={response.StatusCode}, Id={response.Id}, Message={response.Message}");

            Assert.True(response.StatusCode == 200 || response.StatusCode == 201);

            // クリーンアップ
            if (response.Id > 0)
            {
                await bridge.Delete(response.Id.ToString());
            }
        }

        [Fact]
        public async Task BulkInsert_WithGenericType_Success()
        {
            var bridge = CreateBridge();

            // クラスインスタンスのリストを使用
            var dataList = new List<TestDataRecord>
            {
                new TestDataRecord
                {
                    Title = "Generic Bulk Test 1",
                    Body = "Body 1",
                    ClassA = "GenericBulkClassA1",
                    ClassB = "GenericBulkClassB1",
                    NumA = 1,
                    CheckA = true
                },
                new TestDataRecord
                {
                    Title = "Generic Bulk Test 2",
                    Body = "Body 2",
                    ClassA = "GenericBulkClassA2",
                    ClassB = "GenericBulkClassB2",
                    NumA = 2,
                    CheckA = false
                }
            };

            var response = await bridge.BulkInsert(SiteId, dataList);
            Console.WriteLine($"Generic BulkInsert: Status={response.StatusCode}, Message={response.Message}");

            Assert.True(response.StatusCode == 200 || response.StatusCode == 201);
        }

        [Fact]
        public async Task Update_WithGenericType_Success()
        {
            var bridge = CreateBridge();

            // まずデータを挿入
            var insertData = new TestDataRecord
            {
                Title = "Original Title",
                Body = "Original Body",
                ClassA = "GenericUpdateTest"
            };

            var insertResponse = await bridge.Insert(SiteId, insertData);
            Assert.True(insertResponse.StatusCode == 200 || insertResponse.StatusCode == 201);

            // クラスインスタンスを使用して更新
            var updateData = new TestUpdateData
            {
                Title = "Updated Title via Generic",
                Body = "Updated Body via Generic"
            };

            var updateResponse = await bridge.Update(insertResponse.Id.ToString(), updateData);
            Console.WriteLine($"Generic Update: Status={updateResponse.StatusCode}, Message={updateResponse.Message}");

            Assert.True(updateResponse.StatusCode == 200 || updateResponse.StatusCode == 201);

            // クリーンアップ
            await bridge.Delete(insertResponse.Id.ToString());
        }

        [Fact]
        public async Task Update_WithConditions_GenericType_Success()
        {
            var bridge = CreateBridge();

            // まずデータを挿入
            var insertData = new TestDataRecord
            {
                Title = "Condition Update Test",
                Body = "Original Body",
                ClassA = "GenericConditionTest",
                NumA = 999
            };

            var insertResponse = await bridge.Insert(SiteId, insertData);
            Assert.True(insertResponse.StatusCode == 200 || insertResponse.StatusCode == 201);

            // クラスインスタンスを使用して条件指定更新
            var targetColumns = new TestTargetColumns
            {
                ClassA = "GenericConditionTest"
            };

            var updateData = new TestUpdateData
            {
                Title = "Updated by Condition via Generic"
            };

            var updateResponses = await bridge.Update(SiteId, targetColumns, updateData);
            Console.WriteLine($"Generic Conditional Update: Count={updateResponses.Count}");

            Assert.True(updateResponses.Count > 0);
            Assert.All(updateResponses, r => Assert.True(r.StatusCode == 200 || r.StatusCode == 201));

            // クリーンアップ
            await bridge.Delete(insertResponse.Id.ToString());
        }

        [Fact]
        public async Task Upsert_WithGenericType_Success()
        {
            var bridge = CreateBridge();

            // クラスインスタンスを使用してUpsert
            var upsertData = new TestDataRecord
            {
                Title = "Generic Upsert Test",
                Body = "Upsert Body",
                ClassA = "GenericUpsertClassA",
                ClassB = "GenericUpsertClassB",
                NumA = 555
            };

            var matchColumns = new List<string> { "ClassA", "ClassB" };
            var response = await bridge.Upsert(SiteId, matchColumns, upsertData);
            Console.WriteLine($"Generic Upsert: Status={response.StatusCode}, Message={response.Message}");

            Assert.True(response.StatusCode == 200 || response.StatusCode == 201);
        }

        [Fact]
        public async Task Delete_WithConditions_GenericType_Success()
        {
            var bridge = CreateBridge();

            // まずデータを挿入
            var insertData = new TestDataRecord
            {
                Title = "Generic Delete Test",
                Body = "To be deleted",
                ClassA = "GenericDeleteTest"
            };

            var insertResponse = await bridge.Insert(SiteId, insertData);
            Assert.True(insertResponse.StatusCode == 200 || insertResponse.StatusCode == 201);

            // クラスインスタンスを使用して条件指定削除
            var targetColumns = new TestTargetColumns
            {
                ClassA = "GenericDeleteTest"
            };

            var deleteResponses = await bridge.Delete(SiteId, targetColumns);
            Console.WriteLine($"Generic Conditional Delete: Count={deleteResponses.Count}");

            Assert.True(deleteResponses.Count > 0);
            Assert.All(deleteResponses, r => Assert.True(r.StatusCode == 200 || r.StatusCode == 204));
        }

        [Fact]
        public async Task CompareGenericAndDictionary_SameResult()
        {
            var bridge = CreateBridge();

            // Dictionary版
            var dictionaryData = new Dictionary<string, object>
            {
                { "Title", "Dictionary Test" },
                { "Body", "Dictionary Body" },
                { "ClassA", "CompareTest" }
            };

            var dictionaryResponse = await bridge.Insert(SiteId, dictionaryData);

            // Generic版
            var genericData = new TestDataRecord
            {
                Title = "Generic Test",
                Body = "Generic Body",
                ClassA = "CompareTest"
            };

            var genericResponse = await bridge.Insert(SiteId, genericData);

            Console.WriteLine($"Dictionary: Status={dictionaryResponse.StatusCode}, Id={dictionaryResponse.Id}");
            Console.WriteLine($"Generic: Status={genericResponse.StatusCode}, Id={genericResponse.Id}");

            // 両方とも成功することを確認
            Assert.True(dictionaryResponse.StatusCode == 200 || dictionaryResponse.StatusCode == 201);
            Assert.True(genericResponse.StatusCode == 200 || genericResponse.StatusCode == 201);

            // クリーンアップ
            if (dictionaryResponse.Id > 0)
            {
                await bridge.Delete(dictionaryResponse.Id.ToString());
            }
            if (genericResponse.Id > 0)
            {
                await bridge.Delete(genericResponse.Id.ToString());
            }
        }
    }
}
