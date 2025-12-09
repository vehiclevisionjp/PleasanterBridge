using Moq;
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.APIBridge.Contracts;
using PleasanterBridge.src.APIBridge.Mapping;
using PleasanterBridge.src.DataRepository.Service;
using Xunit;

public class PleasanterAPIBridgeTest
{
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
        Assert.Throws<ArgumentException>(() =>
            new PleasanterAPIBridge(_httpClient, "", "key", "server", "db", "user", "pass"));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PleasanterAPIBridge(null, "url", "key", "server", "db", "user", "pass"));
    }

    [Fact]
    public async Task Insert_CallsResponseHandlerAndReturnsResult()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var data = new Dictionary<string, object>();
        var url = "http://test";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetItemCreateUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Insert(siteId, data);

        Assert.Equal(response, result);
        _urlProviderMock.Verify(x => x.GetItemCreateUrl(siteId), Times.Once);
        _responseHandlerMock.Verify(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url), Times.Once);
    }

    [Fact]
    public async Task BulkInsert_CallsResponseHandlerAndReturnsResult()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var dataList = new List<Dictionary<string, object>>();
        var url = "http://bulk";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.BulkInsert(siteId, dataList);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Update_ByReferenceId_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var refId = "id";
        var data = new Dictionary<string, object>();
        var url = "http://update";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetItemUpdateUrl(refId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Update(refId, data);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Update_BySiteId_CallsRepositoryAndResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var targetColumns = new Dictionary<string, object>();
        var updateData = new Dictionary<string, object>();
        var recordIds = new List<string> { "id1", "id2" };
        var referenceType = "Results";
        var filteredRecords = new List<Dictionary<string, object>>
        {
            new() { { "ResultId", "id1" } },
            new() { { "ResultId", "id2" } }
        };
        var url = "http://update";
        var response = new PleasanterApiResponse();

        _referenceProcessMock.Setup(x => x.Execute(siteId, false)).Returns(referenceType);
        _repositoryMock.Setup(x => x.Select(siteId, targetColumns)).Returns(filteredRecords);
        _urlProviderMock.Setup(x => x.GetItemUpdateUrl(It.IsAny<string>())).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var results = await bridge.Update(siteId, targetColumns, updateData);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task Upsert_CallsBulkUpsert()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var matchColumns = new List<string> { "col" };
        var data = new Dictionary<string, object>();
        var url = "http://upsert";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Upsert(siteId, matchColumns, data);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task BulkUpsert_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var matchColumns = new List<string> { "col" };
        var dataList = new List<Dictionary<string, object>>();
        var url = "http://bulkupsert";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetBulkUpsertUrl(siteId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.BulkUpsert(siteId, matchColumns, dataList);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Delete_ByReferenceId_CallsResponseHandler()
    {
        var bridge = CreateBridge();
        var refId = "id";
        var url = "http://delete";
        var response = new PleasanterApiResponse();

        _urlProviderMock.Setup(x => x.GetItemDeleteUrl(refId)).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var result = await bridge.Delete(refId);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Delete_BySiteId_CallsRepositoryAndResponseHandler()
    {
        var bridge = CreateBridge();
        var siteId = "site";
        var targetColumns = new Dictionary<string, object>();
        var referenceType = "Results";
        var filteredRecords = new List<Dictionary<string, object>>
        {
            new() { { "ResultId", "id1" } },
            new() { { "ResultId", "id2" } }
        };
        var url = "http://delete";
        var response = new PleasanterApiResponse();

        _referenceProcessMock.Setup(x => x.Execute(siteId, false)).Returns(referenceType);
        _repositoryMock.Setup(x => x.Select(siteId, targetColumns)).Returns(filteredRecords);
        _urlProviderMock.Setup(x => x.GetItemDeleteUrl(It.IsAny<string>())).Returns(url);
        _responseHandlerMock.Setup(x => x.SendPost(It.IsAny<Dictionary<string, object>>(), url)).ReturnsAsync(response);

        var results = await bridge.Delete(siteId, targetColumns);

        Assert.Equal(2, results.Count);
    }
}