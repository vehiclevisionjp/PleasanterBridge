using Moq;
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.APIBridge.Contracts;
using PleasanterBridge.src.APIBridge.Mapping;
using PleasanterBridge.src.DataRepository.Service;

public class PleasanterAPIBridgeTest
{
    private enum TestStatus
    {
        Completed = 100
    }

    private class StatusUpdateData
    {
        public TestStatus Status { get; set; }
    }

    private readonly Mock<IPleasanterSettings> _settingsMock = new();
    private readonly Mock<IPleasanterUrlProvider> _urlProviderMock = new();
    private readonly Mock<IPleasanterRepository> _repositoryMock = new();
    private readonly Mock<IGetReferenceProcess> _referenceProcessMock = new();
    private readonly Mock<IPleasanterResponseHandler> _responseHandlerMock = new();
    private readonly HttpClient _httpClient = new();

    private PleasanterAPIBridge CreateBridge()
    {
        return new PleasanterAPIBridge(
            _httpClient,
            _settingsMock.Object,
            _urlProviderMock.Object,
            _repositoryMock.Object,
            _referenceProcessMock.Object,
            _responseHandlerMock.Object
        );
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUrlIsNullOrEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new PleasanterAPIBridge(_httpClient, "", "key", "server", "db", "user", "pass"));
        Console.WriteLine($"Constructor_ThrowsArgumentException_WhenUrlIsNullOrEmpty: {ex.Message}");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new PleasanterAPIBridge(null, "url", "key", "server", "db", "user", "pass"));
        Console.WriteLine($"Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull: {ex.Message}");
    }

    [Fact]
    public async Task Insert_CallsResponseHandlerAndReturnsResult()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var data = new Dictionary<string, object>();
        var url = "http://test";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "OK" };

        _urlProviderMock.Setup(x => x.GetItemCreateUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Insert(siteId, data);

        Console.WriteLine($"Insert_CallsResponseHandlerAndReturnsResult: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
        _urlProviderMock.Verify(x => x.GetItemCreateUrl(siteId), Times.Once);
        _responseHandlerMock.Verify(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url), Times.Once);
    }

    [Fact]
    public async Task BulkInsert_CallsResponseHandlerAndReturnsResult()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var dataList = new List<Dictionary<string, object>>();
        var url = "http://bulk";
        var response = new PleasanterApiResponse { StatusCode = 201, Message = "Bulk OK" };

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.BulkInsert(siteId, dataList);

        Console.WriteLine($"BulkInsert_CallsResponseHandlerAndReturnsResult: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Update_WithClassStatus_SendsStatus()
    {
        var bridge = CreateBridge();
        var refId = 1;
        var data = new StatusUpdateData { Status = TestStatus.Completed };
        var url = "http://update";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Updated" };
        Dictionary<string, object>? sentRequest = null;

        _urlProviderMock.Setup(x => x.GetItemUpdateUrl(refId)).Returns(url);
        _responseHandlerMock
            .Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url))
            .Callback<Dictionary<string, object>, string>((request, _) => sentRequest = request)
            .ReturnsAsync(response);

        var result = await bridge.Update(refId, data);

        Assert.Equal(response, result);
        Assert.NotNull(sentRequest);
        Assert.Equal(100, sentRequest["Status"]);
    }

    [Fact]
    public async Task Update_ByReferenceId_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var refId = 1;
        var data = new Dictionary<string, object> { { "Status", 100 } };
        var url = "http://update";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Updated" };
        Dictionary<string, object>? sentRequest = null;

        _urlProviderMock.Setup(x => x.GetItemUpdateUrl(refId)).Returns(url);
        _responseHandlerMock
            .Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url))
            .Callback<Dictionary<string, object>, string>((request, _) => sentRequest = request)
            .ReturnsAsync(response);

        var result = await bridge.Update(refId, data);

        Console.WriteLine($"Update_ByReferenceId_CallsResponseHandler: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
        Assert.NotNull(sentRequest);
        Assert.Equal(100, sentRequest["Status"]);
    }

    [Fact]
    public async Task Update_BySiteId_CallsRepositoryAndResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var targetColumns = new Dictionary<string, object>();
        var updateData = new Dictionary<string, object>();
        var referenceType = "Results";
        var filteredRecords = new List<Dictionary<string, object>>
        {
            new() { { "ResultId", 1 } },
            new() { { "ResultId", 2 } }
        };
        var url = "http://update";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Batch Updated" };

        _referenceProcessMock.Setup(x => x.Execute(siteId, false)).Returns(referenceType);
        _repositoryMock.Setup(x => x.Select(siteId, targetColumns)).Returns(filteredRecords);
        _urlProviderMock.Setup(x => x.GetItemUpdateUrl(It.IsAny<int>())).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var results = await bridge.Update(siteId, targetColumns, updateData);

        Console.WriteLine($"Update_BySiteId_CallsRepositoryAndResponseHandler: Count={results.Count}, StatusCode={results[0].StatusCode}");
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task Upsert_CallsBulkUpsert()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var matchColumns = new List<string> { "col" };
        var data = new Dictionary<string, object>();
        var url = "http://upsert";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Upserted" };

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Upsert(siteId, matchColumns, data);

        Console.WriteLine($"Upsert_CallsBulkUpsert: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
    }

    [Fact]
    public async Task BulkUpsert_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var matchColumns = new List<string> { "col" };
        var dataList = new List<Dictionary<string, object>>();
        var url = "http://bulkupsert";
        var response = new PleasanterApiResponse { StatusCode = 201, Message = "BulkUpserted" };

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.BulkUpsert(siteId, matchColumns, dataList);

        Console.WriteLine($"BulkUpsert_CallsResponseHandler: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Delete_ByReferenceId_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var refId = 1;
        var url = "http://delete";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Deleted" };

        _urlProviderMock.Setup(x => x.GetItemDeleteUrl(refId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Delete(refId);

        Console.WriteLine($"Delete_ByReferenceId_CallsResponseHandler: StatusCode={result.StatusCode}, Message={result.Message}");
        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Delete_BySiteId_CallsRepositoryAndResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = 100;
        var targetColumns = new Dictionary<string, object>();
        var referenceType = "Results";
        var filteredRecords = new List<Dictionary<string, object>>
        {
            new() { { "ResultId", 1 } },
            new() { { "ResultId", 2 } }
        };
        var url = "http://delete";
        var response = new PleasanterApiResponse { StatusCode = 200, Message = "Batch Deleted" };

        _referenceProcessMock.Setup(x => x.Execute(siteId, false)).Returns(referenceType);
        _repositoryMock.Setup(x => x.Select(siteId, targetColumns)).Returns(filteredRecords);
        _urlProviderMock.Setup(x => x.GetItemDeleteUrl(It.IsAny<int>())).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var results = await bridge.Delete(siteId, targetColumns);

        Console.WriteLine($"Delete_BySiteId_CallsRepositoryAndResponseHandler: Count={results.Count}, StatusCode={results[0].StatusCode}");
        Assert.Equal(2, results.Count);
    }
}