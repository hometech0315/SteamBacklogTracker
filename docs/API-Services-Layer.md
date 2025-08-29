# API Services Layer Documentation

## Overview

This document describes the Refit API Services integration for the Steam Backlog Tracker Blazor WebAssembly application. This layer provides type-safe HTTP client services for communicating with the backend API with proper error handling, caching, and loading state management.

## Architecture

The API services layer follows Clean Architecture principles and consists of the following components:

### Core Services

#### ISteamTrackerApi
- **Location**: `src/SteamBacklogTracker.Web/Services/ISteamTrackerApi.cs`
- **Purpose**: Refit interface defining all API endpoints
- **Features**:
  - Type-safe API calls with `ApiResponse<T>` return types
  - Automatic JSON serialization/deserialization
  - Support for cancellation tokens
  - Proper HTTP verb attributes (`[Get]`, `[Post]`, `[Put]`, `[Delete]`)

#### ApiClientService
- **Location**: `src/SteamBacklogTracker.Web/Services/ApiClientService.cs`
- **Purpose**: Centralized HTTP client configuration and setup
- **Features**:
  - Polly retry policies with exponential backoff
  - Circuit breaker pattern for resilience
  - Request/response logging
  - Configurable timeouts and base URLs

#### GameService
- **Location**: `src/SteamBacklogTracker.Web/Services/GameService.cs`
- **Purpose**: High-level service for game-related operations
- **Features**:
  - In-memory caching with configurable TTL
  - Automatic loading state management
  - Error handling and user-friendly messages
  - Cache invalidation on data modifications

#### ApiErrorHandler
- **Location**: `src/SteamBacklogTracker.Web/Services/ApiErrorHandler.cs`
- **Purpose**: Centralized error handling for API operations
- **Features**:
  - HTTP status code to user-friendly message mapping
  - JSON error response parsing
  - Retry recommendation logic
  - Structured logging

#### LoadingStateService
- **Location**: `src/SteamBacklogTracker.Web/Services/LoadingStateService.cs`
- **Purpose**: Manage loading states across the application
- **Features**:
  - Observable loading indicators
  - Automatic state management for operations
  - Event-driven state changes
  - Exception handling with state cleanup

## Configuration

### appsettings.json
```json
{
  "ApiConfiguration": {
    "BaseUrl": "https://localhost:7159",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "CircuitBreakerThreshold": 5,
    "CacheTtlMinutes": {
      "DashboardStats": 5,
      "GamesList": 10,
      "GameDetails": 30
    }
  }
}
```

### Dependency Injection Registration
Services are registered in `Program.cs` with appropriate lifetimes:
- `IApiErrorHandler`, `ILoadingStateService`, `IGameService`: Scoped
- `ISteamTrackerApi`: Configured through Refit with HTTP policies

## Available Endpoints

### Games API
- `GET /api/games` - Get all games
- `GET /api/games/{id}` - Get specific game details
- `GET /api/games/platform/{platform}` - Get games by platform
- `GET /api/games/search?searchTerm=` - Search games
- `POST /api/games` - Create new game
- `PUT /api/games/{id}` - Update existing game
- `DELETE /api/games/{id}` - Delete game
- `POST /api/games/sync/steam/{steamUserId}` - Sync Steam library
- `POST /api/games/sync/epic` - Sync Epic Games library

### Dashboard API
- `GET /api/dashboard/stats` - Get dashboard statistics

## Caching Strategy

The caching implementation uses `IMemoryCache` with the following TTL policies:

| Data Type | Cache Duration | Cache Key Pattern |
|-----------|----------------|-------------------|
| Dashboard Stats | 5 minutes | `dashboard_stats` |
| Games List | 10 minutes | `all_games` |
| Platform Games | 10 minutes | `platform_games_{platform}` |
| Game Details | 30 minutes | `game_{id}` |

Cache invalidation occurs automatically on:
- Game creation, update, or deletion
- Library synchronization operations
- Manual cache clearing

## Error Handling

### HTTP Status Code Mapping
- `400 Bad Request` → "Invalid request. Please check your input."
- `401 Unauthorized` → "You are not authorized to perform this action."
- `404 Not Found` → "The requested resource was not found."
- `500 Internal Server Error` → "Server error occurred. Please try again later."

### Retry Logic
- Transient errors are retried up to 3 times (configurable)
- Exponential backoff: 2^retry_attempt seconds
- Circuit breaker opens after 5 consecutive failures

## Usage Examples

### Basic Service Usage
```csharp
@inject IGameService GameService

// Get all games with caching
var result = await GameService.GetAllGamesAsync();
if (result.IsSuccess)
{
    var games = result.Data;
    // Use games data
}
else
{
    // Handle error: result.ErrorMessage
}
```

### Loading State Management
```csharp
@inject ILoadingStateService LoadingStateService

// Check if games are loading
var isLoading = LoadingStateService.IsLoading(LoadingOperations.LoadingGames);

// Subscribe to loading state changes
LoadingStateService.LoadingStateChanged += OnLoadingStateChanged;
```

### Force Cache Refresh
```csharp
// Force refresh from API
var result = await GameService.GetAllGamesAsync(forceRefresh: true);
```

## Testing

Unit tests are provided for core functionality:
- **Location**: `tests/SteamBacklogTracker.Web.Tests/Services/`
- **Coverage**: LoadingStateService functionality
- **Framework**: xUnit with .NET 8

To run tests:
```bash
dotnet test tests/SteamBacklogTracker.Web.Tests/
```

## Performance Considerations

- **Memory Usage**: Cached data is stored in memory with automatic expiration
- **Network Calls**: Minimized through intelligent caching
- **Concurrent Requests**: Thread-safe implementations using `ConcurrentDictionary`
- **Resource Cleanup**: Proper disposal of HTTP clients managed by DI container

## Monitoring and Logging

The services provide structured logging for:
- API request/response cycles with timing
- Cache hit/miss statistics
- Error occurrences with stack traces
- Circuit breaker state changes
- Loading state transitions

Log levels:
- `Debug`: Cache operations, request/response details
- `Information`: Service operations, cache clearing
- `Warning`: Retry attempts, parsing failures
- `Error`: API errors, exceptions

## Dependencies

- **Refit 7.2.22**: Type-safe HTTP client generation
- **Polly 8.4.1**: Resilience and transient-fault handling
- **Microsoft.Extensions.Caching.Memory**: In-memory caching
- **Microsoft.Extensions.Http.Polly**: Polly integration with HttpClientFactory

## Future Enhancements

Potential improvements for future releases:
- Redis cache for distributed scenarios
- Request deduplication for concurrent identical requests
- Enhanced metrics and monitoring
- GraphQL support for more efficient queries
- WebSocket support for real-time updates