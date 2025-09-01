# Pgsql Vertr Db Migrations

## Install EF tools

```shell
dotnet tool install --global dotnet-ef
```

## Create migrations

```shell
dotnet ef migrations add BacktestTables --context BacktestDbContext
dotnet ef migrations add MarketDataTables --context MarketDataDbContext
dotnet ef migrations add OrderExecutionTables --context OrderExecutionDbContext
dotnet ef migrations add PortfolioTables --context PortfolioDbContext
dotnet ef migrations add StrategiesDataTables --context StrategiesDbContext

```

## Run migrations

```shell
dotnet ef database update --context BacktestDbContext
dotnet ef database update --context MarketDataDbContext
dotnet ef database update --context OrderExecutionDbContext
dotnet ef database update --context PortfolioDbContext
dotnet ef database update --context StrategiesDbContext
```
