# Tinvest API Gateway

## Run local server

```shell
Vertr.TinvestGateway.exe --environment Development --urls "http://localhost:5125"
```

## Open swagger

- [Tinvest swagger](http://localhost:5125/swagger/index.html)

## REST API

### Accounts 

- GetAccounts
- GetSandboxAccounts
- CreateAccount
- CloseAccount
- PayIn

### Instruments

- FindInstrument
- GetInstrumentByTicker
- GetInstrumentById

### Market Data

- GetCandles

### Operations

- GetOperations
- GetPositions
- GetPortfolio

### Orders

- PostOrder
- CancelOrder
- GetOrderState


## Kafka Services and Topics

- tinvest.candles.dev <- produce Contracts.Candle
- tinvest.orderstate.dev <- produce Contracts.OrderState
- tinvest.trades.dev <- produce Contracts.OrderTrades
- tinvest.portfolio.dev <- produce Contracts.PortfolioResponse
- tinvest.positions.dev <- produce Contracts.PositionsResponse

## Database

- No DB access



