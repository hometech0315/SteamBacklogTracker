using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamBacklogTracker.Web.Services;
using Xunit;

namespace SteamBacklogTracker.Web.Tests.Services;

public class LoadingStateServiceTests
{
    private readonly LoadingStateService _loadingStateService;
    private readonly ILogger<LoadingStateService> _logger;

    public LoadingStateServiceTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        
        _logger = serviceProvider.GetRequiredService<ILogger<LoadingStateService>>();
        _loadingStateService = new LoadingStateService(_logger);
    }

    [Fact]
    public void IsLoading_ShouldReturnFalse_WhenOperationNotSet()
    {
        // Act
        var result = _loadingStateService.IsLoading("test_operation");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SetLoading_ShouldSetLoadingState()
    {
        // Arrange
        const string operationKey = "test_operation";

        // Act
        _loadingStateService.SetLoading(operationKey, true);

        // Assert
        Assert.True(_loadingStateService.IsLoading(operationKey));
    }

    [Fact]
    public void SetLoading_ShouldRemoveFromLoadingStates_WhenSetToFalse()
    {
        // Arrange
        const string operationKey = "test_operation";
        _loadingStateService.SetLoading(operationKey, true);

        // Act
        _loadingStateService.SetLoading(operationKey, false);

        // Assert
        Assert.False(_loadingStateService.IsLoading(operationKey));
    }

    [Fact]
    public void GetLoadingOperations_ShouldReturnCurrentlyLoadingOperations()
    {
        // Arrange
        _loadingStateService.SetLoading("operation1", true);
        _loadingStateService.SetLoading("operation2", true);
        _loadingStateService.SetLoading("operation3", false);

        // Act
        var result = _loadingStateService.GetLoadingOperations().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("operation1", result);
        Assert.Contains("operation2", result);
        Assert.DoesNotContain("operation3", result);
    }

    [Fact]
    public async Task ExecuteWithLoadingAsync_ShouldManageLoadingState()
    {
        // Arrange
        const string operationKey = "test_operation";
        var taskExecuted = false;

        // Act
        await _loadingStateService.ExecuteWithLoadingAsync(operationKey, async () =>
        {
            Assert.True(_loadingStateService.IsLoading(operationKey));
            taskExecuted = true;
            await Task.Delay(10);
        });

        // Assert
        Assert.True(taskExecuted);
        Assert.False(_loadingStateService.IsLoading(operationKey));
    }

    [Fact]
    public async Task ExecuteWithLoadingAsync_ShouldClearLoadingState_OnException()
    {
        // Arrange
        const string operationKey = "test_operation";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _loadingStateService.ExecuteWithLoadingAsync(operationKey, async () =>
            {
                await Task.Delay(10);
                throw new InvalidOperationException("Test exception");
            });
        });

        Assert.False(_loadingStateService.IsLoading(operationKey));
    }
}