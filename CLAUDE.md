# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Steam + Epic Games backlog tracker with achievement analytics built with a Clean Architecture approach using .NET 8 LTS.

## Repository Structure

```
src/
├── SteamBacklogTracker.API/           # ASP.NET Core 8 Web API
├── SteamBacklogTracker.Core/          # Domain entities and interfaces
├── SteamBacklogTracker.Infrastructure/ # Data access (EF Core + SQLite) and external APIs
├── SteamBacklogTracker.Application/   # Services and DTOs
├── SteamBacklogTracker.Shared/        # Shared models between Web and future MAUI
└── SteamBacklogTracker.Web/           # Blazor WebAssembly (.NET 8)
```

## Technology Stack

**Backend:**
- .NET 8 LTS (Long Term Support)
- ASP.NET Core 8 Web API
- Entity Framework Core 8 with SQLite
- Clean Architecture pattern

**Frontend:**
- Blazor WebAssembly (.NET 8)
- MudBlazor 8.x for UI components
- Refit for HTTP API calls
- ImageSharp.Web for image optimization

## Current Development Status

✅ **Completed:**
- Clean Architecture backend structure
- Entity Framework with migrations
- Basic API controllers (Games, Dashboard)
- Blazor WebAssembly project setup
- All projects configured for .NET 8 LTS

🚧 **In Progress:**
- Blazor components and pages need to be implemented
- Steam API integration
- Achievement tracking features
- Gaming-themed UI with MudBlazor

## Database Schema

Current entities:
- `Game` - Game information (title, platform, hours played, etc.)
- `Achievement` - Individual achievements with unlock status
- `Genre` and `Tag` - Game categorization

## API Endpoints

- `/api/dashboard/stats` - Dashboard statistics
- `/api/games` - Game library management
- Ready for Steam API integration

## Blazor WebAssembly Structure

- Uses official .NET 8 Blazor WebAssembly template
- Configured with proper runtime files (blazor.webassembly.js)
- No dotnet.js compatibility issues
- Ready for component development

## Development Notes

- All projects target .NET 8 LTS for long-term stability
- Solution compiles without errors
- Database migrations are ready
- Ready for GitHub Copilot integration