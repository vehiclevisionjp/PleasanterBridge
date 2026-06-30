using Moq;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess;
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.DataRepository.Service.Implementation;
using PleasanterBridge.src.Utilities;

public class PleasanterRepositoryTest
{
    private readonly Mock<IDbProcessProvider> _dbProcessProviderMock = new();
    private readonly Mock<IDbProcessFactory> _dbProcessFactoryMock = new();
    private readonly Mock<IGetReferenceProcess> _referenceProcessMock = new();
    private readonly Mock<IGetSitesProcess> _getSitesProcessMock = new();
    private readonly Mock<IDbExecutionProcess> _dbExecutionProcessMock = new();
    private readonly Mock<IConnectionSettings> _settingsMock = new();

    [Fact]
    public void Constructor_WithDbProcessProvider_SetsProvider()
    {
        var repo = new PleasanterRepository(_dbProcessProviderMock.Object);
        Assert.NotNull(repo);
        Console.WriteLine("Constructor_WithDbProcessProvider_SetsProvider: Success");
    }

    [Fact]
    public void Constructor_WithConnectionSettings_SetsProvider()
    {
        _settingsMock.SetupGet(x => x.Server).Returns("server");
        _settingsMock.SetupGet(x => x.Database).Returns("db");
        _settingsMock.SetupGet(x => x.UserId).Returns("user");
        _settingsMock.SetupGet(x => x.Password).Returns("pass");

        var repo = new PleasanterRepository(_settingsMock.Object);
        Assert.NotNull(repo);
        Console.WriteLine("Constructor_WithConnectionSettings_SetsProvider: Success");
    }

    [Fact]
    public void Constructor_WithRawParams_SetsProvider()
    {
        var repo = new PleasanterRepository("server", "db", "user", "pass");
        Assert.NotNull(repo);
        Console.WriteLine("Constructor_WithRawParams_SetsProvider: Success");
    }

    [Fact]
    public void Select_ByReferenceId_ReturnsExpectedResult()
    {
        var repo = new PleasanterRepository(_dbProcessProviderMock.Object);
        var referenceId = 123;
        var referenceType = "Results";
        var expectedType = PleasanterReferenceType.Results;
        var expectedResult = new List<Dictionary<string, object>>
        {
            new() { { "ResultId", "id1" }, { "Name", "Test1" } }
        };

        _dbProcessProviderMock.Setup(x => x.GetReferenceProcess()).Returns(_referenceProcessMock.Object);
        _referenceProcessMock.Setup(x => x.Execute(referenceId, true)).Returns(referenceType);
        _dbProcessProviderMock.Setup(x => x.GetDbProcessFactory()).Returns(_dbProcessFactoryMock.Object);
        _dbProcessFactoryMock.Setup(x => x.Create(expectedType)).Returns(_dbExecutionProcessMock.Object);
        _dbExecutionProcessMock.Setup(x => x.Execute(referenceId, true, null)).Returns(expectedResult);

        var result = repo.Select(referenceId);

        Console.WriteLine($"Select_ByReferenceId_ReturnsExpectedResult: Count={result.Count}, Name={result[0]["Name"]}");
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Select_BySiteIdAndKeyValue_ReturnsExpectedResult()
    {
        var repo = new PleasanterRepository(_dbProcessProviderMock.Object);
        var siteId = 456;
        var keyValue = new Dictionary<string, object> { { "Key", "Value" } };
        var referenceType = "Issues";
        var expectedType = PleasanterReferenceType.Issues;
        var expectedResult = new List<Dictionary<string, object>>
        {
            new() { { "IssueId", "iss1" }, { "Title", "IssueTitle" } }
        };

        _dbProcessProviderMock.Setup(x => x.GetReferenceProcess()).Returns(_referenceProcessMock.Object);
        _referenceProcessMock.Setup(x => x.Execute(siteId, false)).Returns(referenceType);
        _dbProcessProviderMock.Setup(x => x.GetDbProcessFactory()).Returns(_dbProcessFactoryMock.Object);
        _dbProcessFactoryMock.Setup(x => x.Create(expectedType)).Returns(_dbExecutionProcessMock.Object);
        _dbExecutionProcessMock.Setup(x => x.Execute(siteId, false, keyValue)).Returns(expectedResult);

        var result = repo.Select(siteId, keyValue);

        Console.WriteLine($"Select_BySiteIdAndKeyValue_ReturnsExpectedResult: Count={result.Count}, Title={result[0]["Title"]}");
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Select_BySiteIdWithoutKeyValue_ReturnsSites()
    {
        var repo = new PleasanterRepository(_dbProcessProviderMock.Object);
        var siteId = 789;
        var expectedSites = new List<Dictionary<string, object>>
        {
            new() { { "SiteId", "site789" }, { "SiteName", "MySite" } }
        };

        _dbProcessProviderMock.Setup(x => x.GetSitesProcess()).Returns(_getSitesProcessMock.Object);

        var result = repo.Select(siteId, null);

        Console.WriteLine($"Select_BySiteIdWithoutKeyValue_ReturnsSites: Count={result.Count}, SiteName={result[0]["SiteName"]}");
        Assert.Equal(expectedSites, result);
    }
}