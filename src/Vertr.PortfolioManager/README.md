# Portfolio Manager

## Run local server

```shell
Vertr.PortfolioManager.exe --environment Development --urls "http://localhost:5126"
```

## Open swagger

- [PortfolioManager swagger](http://localhost:5126/swagger/index.html)

## REST API

### Snapshots 

- Contracts.PortfolioSnapshot? GetLast(string accountId, Guid? bookId = null);
- Contracts.PortfolioSnapshot? GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100);
- Contracts.PortfolioSnapshot? MakeSnapshot(string accountId, Guid? bookId = null);

MakeSnapshot используется при выставлении ордера - делается снапшот портефеля и проверяется позиция, на которую делается ордер.

## Kafka Services and Topics

- tinvest.portfolio.dev -> consume Tinvest.Contracts.PortfolioResponse -> save PortfolioSnapshot
- tinvest.positions.dev - not used
- vertr.operations.dev -> consume OrderExecution.Contracts.OrderResponse -> save OperationEvent


## Database

- PortfolioSnapshot -> table portfolio_snapshots
  - PortfolioPosition -> related child table portfolio_positions
- OperationEvent -> table operation_events




## BACKLOG

### 2025-06-01

- самостоятельный расчет портфеля и позиций по событию OperationEvent с учетом bookId и сохранение его в БД (portfolio_snapshots + portfolio_positions)
- ...


