---
name: 🐛 Bug Fix
about: Report a bug for Copilot to fix
title: '[BUG] '
labels: ['copilot-task', 'bug', 'needs-fix']
assignees: []
---

## 🐛 Bug Description
<!-- A clear and concise description of the bug -->

## 🔄 Steps to Reproduce
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

## ✅ Expected Behavior
<!-- What should happen -->

## ❌ Actual Behavior  
<!-- What actually happens -->

## 🖥️ Environment
- **Platform:** Blazor WebAssembly/MAUI Windows/MAUI Android/MAUI iOS
- **Framework:** .NET 8 LTS
- **Browser:** [if web] Chrome/Firefox/Safari/Edge
- **OS:** Windows 11/macOS/Android 13/iOS 17
- **App Version:** v1.0.0

## 📝 Error Messages
'Paste any error messages or stack traces here'

## 📷 Screenshots
<!-- Add screenshots if applicable -->

## 🤖 Copilot Instructions

@copilot Please fix this bug with the following approach:

### **Investigation:**
- Analyze the root cause
- Review related code components
- Identify potential side effects

### **Solution Requirements:**
- Minimal code changes targeting .NET 8 LTS
- Preserve existing functionality
- Add defensive programming
- Include regression tests with xUnit

### **Testing:**
- Fix the specific issue
- Add test case to prevent regression
- Verify no new bugs introduced
- Test on all affected platforms (.NET 8 compatible)

## 🔍 Claude Code Review
After Copilot fix, use Claude Code to:
- [ ] Validate the fix approach
- [ ] Suggest performance improvements
- [ ] Review for potential edge cases
- [ ] Ensure consistent error handling

## ✅ Definition of Done
- [ ] Bug is fixed
- [ ] Regression test added
- [ ] No new bugs introduced
- [ ] Code reviewed and optimized
- [ ] Deployed and verified in staging