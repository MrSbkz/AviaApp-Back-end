# Run application
### PowerShell
Run PowerShell inside ./AviaApp/AviaApp folder and enter the following command: `dotnet run`

# Update database
### Visual Studio
- Open "Package Manager Console"(View -> Other Windows -> Package Manager Console)
- Enter command: `Update-Database`
### PowerShell
- Run PowerShell inside _./AviaApp/Data_ folder and enter the following command: `dotnet ef --startup-project ../AviaApp/ database update MigrationName`

- NOTICE! If _dotnet ef_ is not installed you need to install it using the following command: `dotnet tool install --global dotnet-ef`

# Add migration(for developers)
### Visual Studio(using "Package Manager Console")
- Enter command:  `Add-Migration MigrationName`
### PowerShell
- Enter command from _Data_ folder: `dotnet ef --startup-project ../AviaApp/ migrations add MigrationName`

# Deployment
- `heroku git:remote -a avia-app`
- `git push heroku`