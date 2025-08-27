---
name: 🚀 Feature Request
about: Request a new feature implementation via Copilot
title: '[FEATURE] '
labels: ['copilot-task', 'feature', 'needs-implementation']
assignees: []
---

## 📝 Feature Description
<!-- Provide a clear and concise description of the feature -->

## 🎯 User Story
**As a** [type of user]
**I want** [some goal]
**So that** [some reason/value]

## ✅ Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2  
- [ ] Criterion 3
- [ ] Unit tests included
- [ ] Documentation updated

## 🏗️ Technical Requirements

### **Component Type:**
- [ ] Blazor WebAssembly Component (.NET 8)
- [ ] MAUI Cross-platform Component (.NET 8)
- [ ] ASP.NET Core 8 API Endpoint
- [ ] Infrastructure Service
- [ ] Entity Framework Core 8 Migration

### **Dependencies:**
- **NuGet Packages:** List any required packages
- **External APIs:** Steam API, Epic Games, etc.
- **Database Changes:** Yes/No

### **Performance Considerations:**
- **Expected Load:** Low/Medium/High
- **Caching Required:** Yes/No
- **Background Processing:** Yes/No

## 🤖 Copilot Instructions

@copilot Please implement this feature with the following requirements:

### **Architecture:**
- Follow Clean Architecture patterns
- Use dependency injection (.NET 8 patterns)
- Implement proper separation of concerns
- Target .NET 8 LTS framework

### **Error Handling:**
- Try-catch blocks with specific exceptions
- Proper logging (built-in .NET 8 logging)
- User-friendly error messages
- Graceful degradation

### **Testing:**
- Unit tests with xUnit (.NET 8 compatible)
- Mock external dependencies
- Test coverage >80%
- Integration tests for API endpoints

### **Code Quality:**
- Follow C# 12 (.NET 8) coding conventions
- Use proper async/await patterns
- Include XML documentation
- Optimize for .NET 8 performance

### **UI Guidelines (if applicable):**
- Use MudBlazor 8.x components
- Responsive design (mobile-first)
- Follow Steam-inspired gaming theme
- Accessibility compliance (WCAG 2.1)

## 🔍 Claude Code Review Checklist

After Copilot implementation, use Claude Code to verify:
- [ ] Architectural consistency with Clean Architecture
- [ ] Performance optimization opportunities
- [ ] Security considerations
- [ ] Code duplication elimination
- [ ] Advanced error handling patterns
- [ ] Integration with existing components

## 📊 Definition of Done
- [ ] Code implemented and tested locally
- [ ] All acceptance criteria met
- [ ] Code reviewed by Claude Code
- [ ] SonarCloud quality gate passed
- [ ] Documentation updated
- [ ] Deployed to staging environment

## 🔗 Related Issues
<!-- Link any related issues or dependencies -->

## 📷 Screenshots/Mockups
<!-- If applicable, add screenshots or mockups -->