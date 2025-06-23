# Pgsql Vertr Db Migrations

## Install EF tools

```shell
dotnet tool install --global dotnet-ef
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

## Run PortfolioManager tables migration

### Create migration

```shell
dotnet ef migrations add PortfolioTables --context PortfolioDbContext
```

### Run migration

```shell
dotnet ef database update --context PortfolioDbContext
```


### Cleanup DB

```sql
DELETE FROM public.operation_events;

DELETE FROM public.order_events;

DELETE FROM public.portfolio_snapshots;
```

### Queries

```sql

SELECT * FROM public.operation_events


```
