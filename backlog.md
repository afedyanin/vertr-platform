# Backlog

## 2026-02-20

### Metrics

- https://andrewlock.net/creating-and-consuming-metrics-with-system-diagnostics-metrics-apis/
- https://andrewlock.net/creating-strongly-typed-metics-with-a-source-generator/
- https://andrewlock.net/creating-standard-and-observable-instruments/

```
dotnet tool install -g dotnet-counters

cd C:\Users\A\Documents\GitLab\vertr-platform\tests\Vertr.Experimental.MetricsApp\bin\Debug\net10.0

.\Vertr.Experimental.MetricsApp.exe --urls https://localhost:7068

dotnet-counters monitor --counters MyApp.Products -n Vertr.Experimental.MetricsApp

```

## 2026-02-19

- [ ] ИСС выбрасывает ексепшены на старте
- [ ] на старте иногда условия гонки в коллекциях
- [ ] более детальная инфа по портфолио - аналитика, метрики
- [ ] тюнинг алгоритма арбитража, оптимизация
	- [ ] более точная справедливая цена
	- [ ] оценка входа и разворота позиции
	- [ ] подбор трешхолдов и гиперпараметров
	- [ ] более детальное и системное тестирование
	- [ ] выключение стратегий на время клиринга

## 2026-02-05

- [ ] проверить закрытие позиций при выходе
- [ ] проверка результатов торговли по портфелям
- [ ] проверить и протестировать алгоритм GetDirection для сигнала. Вынести его в общую часть
- [ ] найти оптимальное значение threshold-а для фьючей
- [ ] оптимизация алгоритма торговли
- [ ] когда закрывать позицию? 
- [x] притянуть статику по фьючам из MOEX (expDate, lots) сделать IFutureInfoProvider
- [x] притянуть ставки RUSFAR 
- [x] разобраться с QuantityLots в расчете цены и в ордер менеджменте


## 2026-02-01

### Pipeline refactoring

Упростить пайплайн и сделать его Generic

- [x] Разработать и реализовать новую модель пайплайна
- [x] Убрать Channel внутри - сделать снаружи
- [x] Передавать один параметр в пайплайн - евент (сейчас два - евент и сам айтем)
- [x] Вынести общие BGServices в отдельную сборку и сделать их универсальными
- [x] Избавиться от инжекта IServiceProvider (в хендлерах реализовать IEnumerable инжект)
- [x] Проверить работу с бэктестом


## 2026-01-30

### MOEX Adapter

- [ ] Vertr.Moex.ApiClient -  реализовать MOEX API клиент и репозитории в нем
	- [ ] Реализация репозиториев IRatesRepository, IFutureInfoRepository - вынести в Common?
	- [ ] Получение и хранение статики по фьючам FutureInfo
	- [ ] Получение и хранение ставок RUSFAR
- [ ] MoexLoaderService - сервис периодического обновления данных из MOEX
	- [ ] набор инструментов и рейтов для обновления в сеттингах
	- [ ] интервалы времени для обновления - Quartz?
	- [ ] первоначальная загрузка при старте и дамп при выключении (хранение в JSON?)

### Common

- [-] (?) Нужны ли хендлеры? Заменить стратегией?
- [-] добавить расчет средней цены актива в позиции ?
- [x] добавить в Registrar возможность выбора пайплайна и хендлеров
- [x] PortfoliosLocalStorage, PortfolioManager сделать универсальными, убрать информацию о предикторе, заменить на Key или Name


### Future Arbitrage Pipeline

- [x] IFutureArbitragePipeline
- [ ] OrderBookChangedEvent - наполнение нужными полями
- [ ] BaseAssetOrderBookHandler - получение спот цены из стакана базового актива -> Spot.MidPrice
- [ ] FuturePriceCalculationHandler - расчет теоретической цены набора фьючей по ставке RUSFAR
- [ ] FutureSignalsGenerator
	- [ ] получение спот цен из стаканов фьючерсов	
	- [ ] сравнение цен фьючей в стаканах и теоретических цен
	- [ ] генерация сигнала с учетом комиссии и threshold-а
- [?] FutureOrderExecution
	- [ ] получение открытых поциций по фьючам
	- [ ] (?) валидация сигнала - сравнение и анализ FairPrice с текущей открытой позицией (?)
	- [ ] сравнение направления сигнала с открытыми позициями
	- [ ] маркет ордера на открытие или реверс позиций

## 2026-01-26

### Пайплайн арбитража

- статистика по индикативному стакану (базовый актив) -> Spot.MidPrice
- расчет теоретической цены -> FairPrice = Future.Price(Spot.MidPrice, ...)
- получение текущей квоты из стакана -> ActualPrice
	- если есть открытая позиция - сравнение и анализ FairPrice с текущей открытой позицией (!)
- если |ActualPrice - FairPrice| > threshhold -> Сигнал на покупку/продажу
- order execution -> анализ текущей позиции и выставление маркет ордера на открытие и/или разворот


## 2026-01-25

### Common.Contracts

``` csharp

IRatesRepository
Rate[] GetAll(string ticker);
Rate GetLast(string ticker, DateTime date = null);
Task Load(Ticker[], interval?);
string ToJson()
Task FromJson();

// Implementation: Dictionary<Ticker, SortedList<DateTime>>
// Проверить и поисследовать SortedList
// Имплементация внутри MoexClient

IFutureInfoRepository
FutureInfo[] GetAll(tickerWildCard);
FutureInfo Get(string ticker);
Task Load(Ticker[]);
string ToJson()
Task FromJson();

// модели

InterestRate
- Ticker
- DateTime
- Value (or OHLC?)

FutureInfo
- Ticker
- ClassCode
- ExpDate
- LastTradeDate
- LotSize
- PriceStep?

```

### Moex.Client

- имплементация IRatesRepository, IFutureInfoRepository
- использовать HttpClient (vs HttpClientFactory)
- набор нативных DTO моделей
- конвертеры DTO моделей в модели Common контрактов
	- Rate
	- FututeInfo

### Trading Console

- backgroundservice MoexLoaderService
- quartz (12.25/12.30/12.35... по будням)
- первоначальная загрузка из JSON
- вызгузка в JSON при апдейтах репозитория
- проверить задержку MOEX при выдаче RUSFAR (12.30 -> 12.45)

### Vertr.QuantLib

- либа прайсинга

``` scharp

static decimal Future.Price(decimal spotPrice, decimal rate (rate[]?), DateTime expDate, DayCount daycount);

```



## 2026-01-16

### Trading Console

- маркет дата
	- трейды на глубину и индикаторы по ним
	- стаканы на глубину и индикаторы
	- открытый интерес
	- сбор и отсылка метрик в прометеус
- один портфель с несколькими инструментами
	- отслеживание открытой позиции
- ордера
	- отмена ордера
	- перемещение ордера
	- отслеживание неисполненных лимитных ордеров
- арбитражный пайплайн
- стратегия - логика для входов и выходов
- гиперпараметры и настройка - комиссии, задержки и т.п.
- дашборд в графане

## 2026-01-11

- https://learn.microsoft.com/ru-ru/dotnet/core/diagnostics/metrics-instrumentation
- https://habr.com/ru/companies/kaspersky/articles/826038/

## 2026-01-02

- https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder#host-shutdown
- https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder#ihostapplicationlifetime

## 2025-12-28

- Нужен сигнал об окончании бэктеста. TCS?
- [x] пофиксить предикшен гейтвей
- работа с портфелями вне консоли - отдельная консоль, файлы, конфиги?
- передача параметров портфеля в консоль, вывод метрик портфеля?
- телеметрия в графану для BT
- объеденить DAL Common и Gateway? Используется редис напрямую без Gateway?

## 2025-12-27

- [x] имплементация PredictorGateway
- [x] как передавать названия моделей для разных наборов: Stats, ML, Neural? Отдельные методы или парсер?

- [x] использовать отдельные appsettings для докера
- [x] вынести все нужные параметры в конфигурацию
- [x] preictor client api - отдельная сборка
- использовать множество портфелей в трейдинг-клиенте
- бактест с батчами
- бактест по заданному дню

## 2025-12-26

- [x] собирать контейнер для .NET
- [x] настроить сеть для видимости инфры, форкаста и гейтвея
- [x] протестировать конфигруацию и старт контейнеров
- сформировать PROD среду и стартовать контейнеры
- [x] вынести параметры в конфигурационные файлы и енв переменные
- разделить дев и прод среды

## 2025-12-18

- [x] при старте трейдинг консоли не прогружается история по свечам
- [x] при остановке не дампятся портфели
- [x] нужно выделить обвязку с каналом в отдельный класс - TradingPipeline
- нужно доставать портфель после остановки консоли - в момент остановки портфель еще не закрыт 
	-	задавать ID портфелей снаружи, забирать портфели из Редиса после оставнова консоли

## 2025-12-18

- [x] Paper trading in real time - restore trading console
- [x] BT - load candles from CSV
- [x] BT - проверить валидность работы RW - генерация ревенью
- Телеметрия в графану для BT
- Пофисксить TODO

## 2025-12-15

- Generate nornmal random prices for candles
- Unit tests for 
	- GetStats
	- thresholds 
	- Market Quotes