# VanillaSlice Bootstrapper

🚀 A comprehensive .NET project generator that creates enterprise-ready applications with multiple platform support, modern UI frameworks, and integrated services.

## Overview

VanillaSlice Bootstrapper is a web-based project generation tool that creates complete .NET solution architectures with support for web applications, MAUI mobile apps, and various UI frameworks. It follows clean architecture principles and provides a solid foundation for enterprise applications.

## Key Features

### ✅ **Core Architecture**
- **Clean Architecture Implementation** - Proper separation of concerns with dedicated layers
- **Blazor Web Applications** - Modern web UI with server-side and WebAssembly support
- **Blazor Hybrid MAUI App** - Hybrid MAUI App from shared razor components
- **Native MAUI App** - Native MAUI App from Shared backend
- **Web API Integration** - RESTful API services with OpenAPI documentation
- **Entity Framework Core** - Database integration with multiple provider support
- **Aspire Orchestration Support** - Modern cloud-native application orchestration
- **Docker Support** - Containerization ready configurations

### ✅ **Platform Support**
- **Web Applications** - Full-featured Blazor web portals
- **Web API Services** - RESTful backend services
- **Hybrid Apps** - Cross-platform applications

### ✅ **UI Framework Support**
- **Bootstrap 5** - ✅ **Fully Implemented** - Default responsive framework
- **Microsoft Fluent UI** - 🔄 **In Progress** - Microsoft's design system
- **MudBlazor** - 🔄 **In Progress** - Material Design for Blazor
- **Radzen Components** - 🔄 **In Progress** - Rich component library
- **Tailwind CSS** - 🔄 **In Progress** - Utility-first CSS framework

### ✅ **Database Support**
- **SQL Server** - ✅ **Fully Implemented**
- **SQLite** - ✅ **Fully Implemented**
- **PostgreSQL** - ✅ **Fully Implemented**
- **No Database Option** - ✅ **Fully Implemented**

### ✅ **Authentication & Security**
- **Identity Integration** - ✅ **Fully Implemented**
- **Authorization Policies** - ✅ **Fully Implemented**
- **JWT Token Support** - ✅ **Fully Implemented**

## Implementation Status Matrix

| Feature | Status | Notes |
|---------|--------|-------|
| **Core Platform** |
| Web Application | ✅ **Complete** | Blazor Server/WebAssembly hybrid |
| Web API | ✅ **Complete** | RESTful services with OpenAPI |
| MAUI Native App | 🔄 **In Progress** | Cross-platform mobile support |
| **UI Frameworks** |
| Bootstrap 5 | ✅ **Complete** | Default implementation |
| Microsoft Fluent UI | 🔄 **In Progress** | Blazor integration underway |
| MudBlazor | 🔄 **In Progress** | Material Design components |
| Radzen Components | 🔄 **In Progress** | Rich UI component library |
| Tailwind CSS | 🔄 **In Progress** | Utility-first CSS integration |
| **Services & Features** |
| Authentication | ✅ **Complete** | Identity with JWT support |
| Database Integration | ✅ **Complete** | EF Core with multiple providers |
| Dialog Services | 🔄 **In Progress** | Modal and popup management |
| Notification Services | 🔄 **In Progress** | Toast and alert systems |
| **Advanced Features** |
| Push Notifications | 📅 **TBD** | Mobile and web push support |
| Offline Data Access | 📅 **TBD** | Local storage and sync |
| Real-time Chat | 📅 **TBD** | SignalR chat implementation |
| File Upload/Management | 📅 **TBD** | Blob storage integration |
| Reporting Services | 📅 **TBD** | PDF and Excel generation |

## Quick Start

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- SQL Server (optional - SQLite available for development)

### Running the Bootstrapper

1. **Clone the repository:**
   ```bash
   git clone https://github.com/zero-know/VanillaSlice.git
   cd VanillaSlice/Bootstrapper
   ```

2. **Run the application:**
   ```bash
   cd VanillaSlice.Bootstrapper
   dotnet run
   ```

3. **Access the web interface:**
   - Open your browser to `https://localhost:5001`
   - Use the Project Wizard to configure your application

### Using the Project Wizard

1. **Project Configuration**
   - Enter your project name and root namespace
   - Choose your target platform (Web-only or Web + MAUI)
   - Select component strategy (Common library or Embedded)

2. **Platform Selection**
   - **Web Only**: Creates Blazor web application with API
   - **Web + MAUI**: Adds cross-platform mobile app support

3. **UI Framework Selection**
   - Choose from Bootstrap, Fluent UI, MudBlazor, Radzen, or Tailwind
   - Framework-specific components and styling will be configured

4. **Database Configuration**
   - Select database provider (SQL Server, PostgreSQL, SQLite, or None)
   - Configure connection strings and Entity Framework settings

5. **Additional Features**
   - Authentication and authorization setup
   - Sample components and data
   - Docker and Aspire orchestration support

6. **Generate Project**
   - Click "Generate Project" to create your solution
   - Download the generated ZIP file
   - Extract and open in your preferred IDE

## Generated Project Structure

```
YourProject/
├── src/
│   ├── YourProject.WebAPI/              # REST API services
│   ├── YourProject.WebPortal/           # Blazor web application
│   ├── YourProject.WebPortal.Client/    # Client-side components
│   ├── YourProject.HybridApp/           # MAUI hybrid app
│   ├── YourProject.Server.Data/         # Data access layer
│   ├── YourProject.Server.DataServices/ # Business logic services
│   ├── YourProject.ServiceContracts/    # Shared contracts
│   ├── YourProject.Common/              # Shared utilities
│   ├── YourProject.Client.Shared/       # Client shared components
│   └── YourProject.Framework/           # Core framework
├── YourProject.AppHost/                 # Aspire orchestration
├── YourProject.ServiceDefaults/         # Default configurations
└── YourProject.sln                      # Solution file
```

## Architecture Overview

The generated solutions follow **Clean Architecture** principles:

- **Presentation Layer**: Blazor components, Web API controllers
- **Application Layer**: Services, business logic, contracts
- **Infrastructure Layer**: Data access, external services
- **Domain Layer**: Entities, value objects, domain services

## Database Integration

### Supported Providers
- **SQL Server**: Production-ready with advanced features
- **PostgreSQL**: Open-source alternative with full feature support
- **SQLite**: Development and lightweight deployment scenarios
- **No Database**: In-memory or external data sources

### Entity Framework Features
- Code-first migrations
- Seed data configuration
- Repository pattern implementation
- Unit of work pattern
- CRUD operations with validation

## UI Framework Integration

### Bootstrap 5 (Default)
- Responsive grid system
- Modern components
- Dark/light theme support
- Custom component implementations

### Framework-Specific Features (In Progress)
- **Fluent UI**: Microsoft design system integration
- **MudBlazor**: Material Design components with theming
- **Radzen**: Rich data grids, charts, and form components
- **Tailwind**: Utility-first styling with custom components

## Contributing

We welcome contributions! Please see our contributing guidelines and help us expand the framework support and add new features.

### Priority Areas
- UI framework implementations (MudBlazor, Radzen, Tailwind, Fluent UI)
- MAUI native app templates
- Dialog and notification services
- Advanced features (push notifications, offline sync, real-time chat)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- **Issues**: Report bugs and request features on GitHub Issues
- **Documentation**: Full documentation available in the `/docs` folder
- **Community**: Join our discussions for help and contributions

---

**Status Legend:**
- ✅ **Complete**: Fully implemented and tested
- 🔄 **In Progress**: Currently under development
- 📅 **TBD**: Planned for future releases
