# Computer Club

This is a simple desktop application for managing a small computer club. The project is a WPF app built on .NET 10. It contains basic models for clients, computers, games, sessions, orders and tariffs.

The text below explains what the project does, how to build and run it, and where to look to change common settings.

## What it is

- A friendly WPF desktop app to track club clients and computer usage.
- Keeps data about clients, computers, games and orders.
- Includes a login window and the main management window.

## Main features

- Client management (add, edit, list)
- Computer and hall management
- Game and product catalog
- Orders and session tracking
- Simple tariff / pricing support

## Requirements

- Windows
- .NET 10 SDK (download from https://dotnet.microsoft.com)
- Visual Studio 2022/2026 or another IDE that supports WPF and .NET 10

## Build and run (quick)

1. Open solution file `computerclub.slnx` in Visual Studio and run the project.
2. Or use the dotnet CLI in a terminal:
   - dotnet build
   - dotnet run --project computerclub.csproj

The app will open a login window on start. If the project uses a database, make sure the connection string is configured.

## Database and configuration

- Check `Models/ComputerClubContext.cs` for database setup and default connection.
- If there is an `appsettings.json`, check it for connection strings.
- If the project uses Entity Framework Core, run migrations after changing models.

## Common changes

- Change startup or app behavior: check `App.xaml` and `App.xaml.cs`.
- Edit UI: open XAML files in the root and `Controls` folder (for example, `MainWindow.xaml`, `LoginWindow.xaml`).
- Update models: edit files in the `Models` folder.

## Contributing

- This looks like a small test project. If you want to help: fork or clone the repo, create a branch, make changes and open a pull request.

## License

- No license file is included. Add a `LICENSE` file if you want to publish under a specific license.

## Notes

- Remove any secrets or local passwords before publishing. Do not add build artifacts (`bin/`, `obj/`) to the repository.

