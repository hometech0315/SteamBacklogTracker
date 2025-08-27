claude-code "Transform the Steam Backlog Tracker project from React to Blazor + MAUI with the following complete specification:

## ARCHITECTURE TRANSFORMATION

### 1. KEEP UNCHANGED (Backend):
- SteamBacklogTracker.API (ASP.NET Core 8 Web API)
- SteamBacklogTracker.Core (Domain entities and interfaces)  
- SteamBacklogTracker.Infrastructure (Data access and external APIs)
- SteamBacklogTracker.Application (Services and DTOs)

### 2. REPLACE FRONTEND:
Replace the entire React frontend with:

**SteamBacklogTracker.Shared:**
- Shared DTOs and models between Web and MAUI
- Common validation attributes
- Shared constants and enums

**SteamBacklogTracker.Web (Blazor WebAssembly):**
- Modern Blazor WebAssembly project (.NET 8 LTS)
- MudBlazor 8.x for UI components with gaming dark theme
- Refit for type-safe HTTP API calls  
- ImageSharp for game cover optimization
- Azure Static Web Apps deployment ready

**SteamBacklogTracker.MAUI (Cross-platform):**
- MAUI Blazor Hybrid app targeting Windows, Android, iOS (.NET 8 LTS)
- Shared Razor components with Web project
- Platform-specific services for file system access (Epic Games local files)
- SQLite for offline support and caching
- Local game library synchronization

### 3. UI FRAMEWORK - MudBlazor Gaming Theme:
Create a Steam-inspired dark theme with:
- Primary: Steam blue (#1e88e5)
- Secondary: Achievement gold (#ffc107) 
- Background: Dark gaming theme (#1a1a1a)
- Surface: Card backgrounds (#2d2d30)
- Custom CSS for gaming aesthetics

### 4. KEY COMPONENTS TO CREATE:

**Dashboard Components:**
- StatCards.razor (Total games, hours played, achievement %)
- RecentActivity.razor (Recently played games)
- ProgressCharts.razor (Gaming analytics with MudChart)
- QuickActions.razor (Sync Steam, filter games)

**Game Components:**
- GameCard.razor (Individual game display with cover, hours, achievements)
- GameGrid.razor (Grid layout with virtualization for performance)
- GameList.razor (List layout with sorting and filtering)
- GameDetail.razor (Detailed view with achievement breakdown)
- AchievementProgress.razor (Visual progress bars)

**Filter Components:**
- GameFilters.razor (Advanced filtering: platform, genre, completion status)
- SearchBar.razor (Real-time search across game library)

### 5. BLAZOR WEB SPECIFIC:
- Program.cs with MudBlazor services and Refit HTTP clients
- wwwroot with gaming-optimized PWA manifest
- Custom CSS with Steam-inspired styling
- Responsive design (mobile-first with MudGrid)

### 6. MAUI SPECIFIC:
- MauiProgram.cs with Blazor WebView and platform services
- Platform folders (Windows, Android, iOS) with platform-specific code
- Local file access services for Epic Games integration
- SQLite database for offline game library
- App icons and splash screens with gaming theme

### 7. INTEGRATION REQUIREMENTS:
- Same Steam API endpoints from existing backend
- Refit interfaces matching current API controllers
- Error handling with user-friendly messages
- Loading states for all async operations
- Caching strategy for game metadata and images

### 8. PERFORMANCE OPTIMIZATIONS:
- MudDataGrid with virtualization for large game libraries
- Image lazy loading and optimization
- Memory cache for frequently accessed data
- Pagination for Steam API calls
- Efficient re-rendering with proper Blazor lifecycle

### 9. DEPLOYMENT CONFIGURATIONS:
Update GitHub Actions workflows for:
- Azure Static Web Apps (Blazor WebAssembly)
- Microsoft Store (MAUI Windows)
- Google Play Store (MAUI Android) 
- Apple App Store (MAUI iOS)

### 10. PROJECT STRUCTURE:
src/
├── SteamBacklogTracker.API/              # Keep unchanged
├── SteamBacklogTracker.Core/             # Keep unchanged  
├── SteamBacklogTracker.Infrastructure/   # Keep unchanged
├── SteamBacklogTracker.Application/      # Keep unchanged
├── SteamBacklogTracker.Shared/           # NEW: Shared models
├── SteamBacklogTracker.Web/              # NEW: Blazor WebAssembly
└── SteamBacklogTracker.MAUI/             # NEW: MAUI Cross-platform

### 11. MAINTAIN CLEAN ARCHITECTURE:
- Dependency injection patterns
- Interface segregation  
- Repository patterns (keep existing)
- Service layer separation
- Proper error handling and logging

### 12. DEVELOPMENT EXPERIENCE:
- VS Code optimized project structure
- GitHub Copilot-friendly code patterns
- Comprehensive XML documentation
- Unit test projects for new components
- README with setup instructions

Create a complete, production-ready transformation that maintains all existing backend functionality while providing a modern, cross-platform gaming UI experience."