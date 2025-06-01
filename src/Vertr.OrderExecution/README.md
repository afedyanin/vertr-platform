# Order Execution

## Run local server

```shell
Vertr.OrderExecution.exe --environment Development --urls "http://localhost:5127"
```

## Open swagger

- [OrderExecution swagger](http://localhost:5127/swagger/index.html)

## REST API

### Order Execution Client


#### Positions

- OrderExecutionResponse OpenPosition(OpenPositionRequest request)
- OrderExecutionResponse ClosePosition(ClosePositionRequest request)
- OrderExecutionResponse RevertPosition(RevertPositionRequest request)

Вызовы используются для ручного управления позицией

### Signals

- OrderExecutionResponse Porocess(TradingSignalRequest request)

Вызов используется для автоматической обработки трейдинг сингала и выставления соответствующего ордера

## Kafka Services and Topics

- tinvest.orderstate.dev -> consume Tinvest.Contracts.OrderState -> create OrderEvent -> save OrderEvent -> create OrderOperation[] -> publish OrderOperation[]
- tinvest.trades.dev -> consume Tinvest.Contracts.OrderTrades -> create OrderEvent -> save OrderEvent -> create OrderOperation[] -> publish OrderOperation[]
- vertr.operations.dev <- produce OrderOperation[]

## Database

- OrderEvent -> table order_events

## BACKLOG





