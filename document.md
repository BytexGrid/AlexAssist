# AlexAssist - Personal Assistant & Diary Application

## Project Overview
AlexAssist is a modern WPF application built with .NET 8, designed to serve as a personal assistant and diary application. The application features a stunning Material Design interface with theme support and various productivity tools.

## Features (Implemented)
-  Modern Material Design UI with Dark/Light theme support
-  Responsive and intuitive user interface
-  MVVM Architecture implementation
-  Theme switching capability
-  Dynamic content loading
-  Personal Diary with code snippets support
-  SQLite database integration for data persistence
-  Search and filtering functionality
-  Mood tracking in diary entries
-  Task Management with priority and category support
   - Create, edit, and delete tasks
   - Mark tasks as complete
   - Filter and search tasks
   - Due date tracking
   - Priority levels (High, Medium, Low)
   - Category organization

## Features (Planned)
-  Dashboard with Progress Tracking
-  Motivational Quotes
-  File Storage for Notes and Attachments

## Technical Stack
- .NET 8.0
- WPF (Windows Presentation Foundation)
- Material Design Theme
- SQLite Database
- Entity Framework Core

## Project Structure
- `AlexAssist.UI` - Main WPF Application
  - `Models/` - Data models and database context
    - `AppDbContext.cs` - EF Core database context
    - `DiaryEntry.cs` - Diary entry model
    - `DiaryRepository.cs` - Repository for diary operations
    - `TodoTask.cs` - Task model
    - `TaskRepository.cs` - Repository for task operations
  - `ViewModels/` - MVVM view models
  - `Views/` - UI components and pages
  - `Resources/` - Application resources and assets

## Progress
- [x] Initial project setup
- [x] Added required NuGet packages
- [x] Implemented Material Design theme
- [x] Created MVVM architecture
- [x] Implemented theme switching
- [x] Created data models
- [x] Set up database context
- [x] Implement diary feature
- [x] Add code snippets support
- [x] Implement database persistence
- [x] Implement task management
- [ ] Create dashboard
- [ ] Add motivational quotes
- [ ] Implement settings
- [ ] Add file storage
- [ ] Testing and refinement

## Getting Started
1. Ensure you have .NET 8 SDK installed
2. Clone the repository
3. Open the solution in Visual Studio
4. Build and run the application

## Contributing
This project is open for contributions, especially from new C# developers looking to learn and improve their skills!

## Next Steps
1. Create a beautiful dashboard with statistics
2. Add motivational quotes feature
3. Implement user settings persistence
4. Add data backup/export functionality
5. Create comprehensive error handling and logging 