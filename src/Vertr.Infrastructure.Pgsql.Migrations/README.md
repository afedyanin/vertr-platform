# Pgsql Vertr Db Migrations

## Install EF tools

```shell
dotnet tool install --global dotnet-ef
```

## Run PortfolioManager tables migration

### Create migration

```shell
dotnet ef migrations add PortfolioTables --context PortfolioDbContext
```

### Run migration

```shell
dotnet ef database update --context PortfolioDbContext
```


## Run OrderExecution tablse migration 

### Create migration

```shell
dotnet ef migrations add OrderExecutionTables --context  OrderExecutionDbContext
```

### Run migration

```shell
dotnet ef database update --context OrderExecutionDbContext
```
