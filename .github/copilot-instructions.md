# Copilot Instructions - Steam Backlog Tracker

## Project Overview

This is a **Steam + Epic Games backlog tracker** with achievement analytics built using **Clean Architecture** and **.NET 8 LTS**. The application helps gamers track their gaming library, monitor achievements, and analyze gaming patterns with a Steam-inspired dark gaming theme.

## Architecture & Technology Stack

### Backend (.NET 8 LTS)
- **Architecture**: Clean Architecture pattern
- **API**: ASP.NET Core 8 Web API
- **Database**: Entity Framework Core 8 with SQLite
- **Patterns**: Repository pattern, CQRS, Dependency Injection

### Frontend (Blazor WebAssembly)
- **Framework**: Blazor WebAssembly (.NET 8)
- **UI Library**: MudBlazor 8.x with custom Steam-inspired gaming theme
- **HTTP Client**: Refit for type-safe API communication
- **State Management**: Blazor component state with future SignalR integration

### Project Structure
```
src/
├── SteamBacklogTracker.API/           # ASP.NET Core 8 Web API
├── SteamBacklogTracker.Core/          # Domain entities and interfaces
├── SteamBacklogTracker.Infrastructure/ # Data access (EF Core) and external APIs
├── SteamBacklogTracker.Application/   # Services and DTOs
├── SteamBacklogTracker.Shared/        # Shared models
└── SteamBacklogTracker.Web/           # Blazor WebAssembly (.NET 8)
```

## Current Implementation Status

### ✅ Completed Components
1. **Gaming Theme (Issue #5)** - Complete Steam-inspired dark theme
   - Steam blue (#1e88e5) and achievement gold (#ffc107) color palette
   - MudBlazor 8.0.0 integration with custom CSS architecture
   - 5 specialized CSS files: gaming-theme.css, dashboard-styles.css, navigation-styles.css, game-card-styles.css, form-styles.css
   - Responsive design with gaming aesthetics and animations

2. **Clean Architecture Backend**
   - Domain entities: Game, Achievement, Genre, Tag
   - EF Core with SQLite database and migrations
   - Basic API controllers for Games and Dashboard

3. **Navigation & Layout** (Partial)
   - Mini drawer sidebar navigation with Steam-like styling
   - Navigation badges and gaming icons
   - Responsive mobile-first design

### 🟡 Partially Completed
1. **Dashboard Components (Issue #1)** - 30% complete
   - Static UI implemented with stats cards and recent activity
   - Needs API integration for real data

2. **Navigation Layout (Issue #6)** - 50% complete
   - Basic sidebar implemented
   - Missing breadcrumbs and advanced navigation

### ❌ Pending Implementation (Priority Order)
1. **API Services Layer (Issue #4)** - Foundation critical
2. **Game Display Components (Issue #2)** - Reusable components
3. **Core Pages Implementation (Issue #7)** - Main pages
4. **Dialog Components (Issue #3)** - Modals and dialogs
5. **PWA Mobile Optimization (Issue #8)** - Progressive Web App features

## Coding Standards & Conventions

### C# Backend
- Use **Clean Architecture** patterns consistently
- Follow **SOLID principles**
- Implement proper **async/await** patterns
- Use **Entity Framework Core** conventions
- Apply **dependency injection** throughout
- Target **.NET 8 LTS** features

### Blazor Frontend
- Use **MudBlazor components** with gaming theme integration
- Follow **Blazor WebAssembly** best practices
- Implement **component-based architecture**
- Use **Refit** for HTTP API calls
- Apply **gaming theme CSS classes** consistently
- Maintain **responsive design** patterns

### CSS & Styling
- Use **CSS custom properties** (CSS variables) for theming
- Follow **Steam-inspired gaming aesthetics**
- Implement **responsive design** with mobile-first approach
- Use **animations and transitions** for gaming feel
- Apply **consistent spacing and typography**

## Gaming Theme Guidelines

### Color Palette
```css
/* Primary Steam Colors */
--steam-blue: #1e88e5;
--steam-blue-dark: #1565c0;
--steam-blue-light: #42a5f5;

/* Achievement Colors */
--achievement-gold: #ffc107;
--achievement-gold-dark: #ff8f00;

/* Dark Gaming Background */
--gaming-background: #1a1a1a;
--gaming-surface: #2d2d30;
--gaming-surface-light: #383838;
```

### Component Styling
- Use **gaming-container** for main content areas
- Apply **gaming-card** for individual game items
- Use **achievement-badge** for achievement indicators
- Implement **gaming-btn** for primary actions
- Apply **status indicators** for game completion states

### Animations
- Use **subtle hover effects** with transform and glow
- Implement **achievement animations** for unlocks
- Apply **loading animations** with gaming aesthetics
- Use **transition timings** defined in CSS variables

## API Integration Patterns

### Refit Service Interfaces
```csharp
public interface ISteamTrackerApi
{
    [Get("/api/games")]
    Task<List<GameDto>> GetGamesAsync();
    
    [Get("/api/dashboard/stats")]
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
```

### Error Handling
- Implement **global error boundaries**
- Use **MudBlazor snackbar** for user notifications
- Apply **retry policies** for API calls
- Handle **loading and error states** consistently

## Database Schema

### Core Entities
- **Game**: Title, Platform, HoursPlayed, CompletionStatus, AchievementCount
- **Achievement**: Name, Description, UnlockedDate, GameId
- **Genre/Tag**: Categorization and filtering

### Relationships
- Game 1:N Achievements
- Game M:N Genres
- Game M:N Tags

## Testing Approach
- Use **xUnit** for unit testing
- Implement **integration tests** for API endpoints
- Apply **Blazor component testing** for UI components
- Use **Entity Framework** in-memory database for testing

## External Integrations (Future)
- **Steam Web API** for library synchronization
- **Epic Games API** for Epic library support
- **IGDB API** for game metadata and images
- **Achievement tracking APIs**

## Deployment & Environment
- **Development**: SQLite database
- **Production**: Consider PostgreSQL or SQL Server
- **Hosting**: Azure App Service or similar
- **CI/CD**: GitHub Actions integration

## Key Dependencies
```xml
<!-- Backend -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" />
<PackageReference Include="Swashbuckle.AspNetCore" />

<!-- Frontend -->
<PackageReference Include="MudBlazor" Version="8.0.0" />
<PackageReference Include="Refit.HttpClientFactory" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" />
```

## Specific Instructions for Copilot

### When Working on Components
1. **Always use MudBlazor components** as the base
2. **Apply gaming theme CSS classes** for consistent styling
3. **Implement responsive design** with mobile considerations
4. **Use Refit services** for API communication
5. **Follow Clean Architecture** patterns in backend code

### When Working on Styling
1. **Use CSS custom properties** from gaming-theme.css
2. **Implement Steam-like aesthetics** with blue/gold palette
3. **Add subtle animations** for better UX
4. **Ensure accessibility** compliance
5. **Test responsive behavior** across devices

### When Working on APIs
1. **Follow REST conventions** consistently
2. **Implement proper error handling** and logging
3. **Use Entity Framework** best practices
4. **Apply async patterns** throughout
5. **Maintain Clean Architecture** separation

### Priority Implementation Order
1. Start with **Issue #4: API Services Layer** for foundation
2. Then **Issue #2: Game Display Components** for reusable UI
3. Follow with **Issue #7: Core Pages Implementation** for main functionality
4. Complete **Issue #1: Dashboard Components** with real data
5. Finish with **Issue #3: Dialog Components** for enhanced UX

Remember: This is a gaming-focused application, so always prioritize user experience, visual appeal, and smooth interactions that feel appropriate for a Steam-like gaming environment.