# EventPlatform Back-end 

Back-end version of event platform project

## Table of content
- [Installation](#installation)
- [Usage](#usage)
- [Structure](#project-structure)
- [Environment](#environment)
- [License](#license)
## Installation

The project required [.NET](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) SDK 8

**NOTE:** Install dotnet ef core (If already installed, skip this section)
```csharp
dotnet tool install --global dotnet-ef
```

## Usage

- Clone the repository
```bash
git clone https://github.com/kleqing/EventPlatform-Back-end.git
```

- Build
```bash
dotnet build
```

- Create database from model (required dotnet ef core)
```bash
dotnet ef migrations add "Initial" --project EventPlatform.Infrastructure  --startup-project EventPlatform.WebApi --context ApplicationDbContext
dotnet ef database update --project EventPlatform.Infrastructure  --startup-project EventPlatform.WebApi --context ApplicationDbContext
```

*API Endpoint:* https://localhost:7063/swagger/index.html

## Project Structure

- TBA

## Environment

- MSSQL 22
- Jetbrains Rider (Other IDE still work)
- TablePlus (SQL Management)

## License

This project is licensed under the [MIT License](https://github.com/kleqing/EventPlatform-Back-end/blob/main/LICENSE)
