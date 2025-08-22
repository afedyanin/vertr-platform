# Pgsql Vertr Db Migrations

## Install EF tools

```shell
dotnet tool install --global dotnet-ef
```

## Run MarketData tables migration

### Create migration

```shell
dotnet ef migrations add MarketDataTables --context MarketDataDbContext
```

### Run migration

```shell
dotnet ef database update --context MarketDataDbContext
```

###  Cleanup
``` sql
DELETE FROM public.candles;
DELETE FROM public.candles_history;
DELETE FROM public.candle_subscriptions;
DELETE FROM public.instruments;
```


## Run Strategies tables migration

### Create migration

```shell
dotnet ef migrations add StrategiesDataTables --context StrategiesDbContext
```

### Run migration

```shell
dotnet ef database update --context StrategiesDbContext
```

###  Cleanup
``` sql
DELETE FROM public.strategies;
DELETE FROM public.trading_signals;
```

## Run OrderExecution tables migration 

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

## Run Backtest tables migration 

### Create migration

```shell
dotnet ef migrations add BacktestTables --context  BacktestDbContext
```

### Run migration

```shell
dotnet ef database update --context BacktestDbContext
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
