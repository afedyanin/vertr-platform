# 2025-07-15

## Channels

- https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
- https://saigontechnology.com/blog/implement-producerconsumer-patterns-using-channel-in-c/
- https://habr.com/ru/articles/508726/
- https://www.youtube.com/watch?v=lHC38t1w9Nc
- 

## TPL DataFlow

- [Dataflow (Task Parallel Library)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library)

## Redis pub/sub

- [Redis Pub/sub](https://redis.io/docs/latest/develop/pubsub/)

- [Efficient Microservices Communication: Redis Pub/Sub Message Broker Implementation Guide](https://github.com/Vahidalizadeh7070/RedisMessageBrokerMedium)
- https://blog.devops.dev/efficient-microservices-communication-redis-pub-sub-message-broker-implementation-guide-6e5a5f491e18

- [Implementing Redis Pub-Sub in .NET: A Step-by-Step Guide](https://medium.com/@muhammadzakaria616/implementing-redis-pub-sub-in-net-a-step-by-step-guide-9823202507d0)

- [Simple Messaging in .NET With Redis Pub/Sub](https://www.milanjovanovic.tech/blog/simple-messaging-in-dotnet-with-redis-pubsub)
- [Publish/Subscribe messaging pattern in Redis](https://github.com/ServiceStack/ServiceStack.Examples/blob/master/src/Docs/redis-client/redis-pubsub.md)
- [Redis for .NET Developers – Redis Pub Sub](https://taswar.zeytinsoft.com/redis-pub-sub/)
- [The Redis Feature You Didn't Know You Needed](https://www.youtube.com/watch?v=jRTChmMRCIA)

## Redis streams

- [Redis Streams](https://redis.io/docs/latest/develop/data-types/streams/)
- [How to use Redis Streams with .NET](https://redis.io/learn/develop/dotnet/streams/stream-basics)
- [Redis Stream — надёжность и масштабируемость ваших систем сообщений](https://habr.com/ru/articles/456270/)
- [Building a High-Performance Message Queue with Redis Streams](https://goatreview.com/building-a-high-performance-message-queue-with-redis-streams/)
- [Redis Streams With .NET Examples](https://github.com/redis-developer/redis-streams-with-dotnet/tree/main)




# 2025-07-08

## MDS

Назначение:
1) обеспечивает работу стратегий, предоставляя информацию по свечам
2) предоставляет рыночную цену для марект ордера в случае рабты с  order execution stub
3) первичная информация для формирования синтетических данных по свечам

- обеспечить получение текущей цены для исполнения ордера (в заглушке order excution)
- обеспечить сохранение свечи, принятой из гейтвея, в хранилище маркетных данных
- обеспечить доступ к текущим интрадей свечам из API
- предварительная загрузка интрадей свечей при старте
- механизм загрузки свечей для бэктеста при использовании исторических данных
- сигнал в систему о получении новой свечи (для модуля стратегий)

## Strategies Module

- получает сингал от MDS (Handler)
- вызывает API MDS для получения маркетных данных
- обращается к оракулу для получения торгового сигнала
- хранит в БД торговые сигналы в привязке к свечам
- общащается к Order Execution, передавая в API торговый сигнал

- первоначальная загрузка и настройка стратегий (на уровне БД)
- управление подписками на маркет дату
- идентификация стратегии по сигналу
- определение объекта стратегии и CRUD операции по нему

- где хранить объекты стратегий?
- как активировать при получении рыночных данных?


## BackTest

- необходимо обеспечить установку заданной текущей даты, отличной от Now
  - при создании сущностей (ордера, позиции, портфолио)
  - при работе с маркет датой


# 2025-07-01

## Order Execution

- [x] реализовать оверрайды по позициям - с учетом субакаунтов?
- протестировать учет по субакаунтам
- вычищать позиции с нулевым балансом из портфеля
- при запросе портфеля без субакаунта агрегировать все субакаунты

- [x] реализовать эмулятор Order Exec Engine

## Market Data

- собирать и восстанавливать интрадей свечи по инструментам
- генерить сигналы по приходу новой интрадей свечи

## Backtest

- (!) конролировать дату создания операций и трейдов - не должно быть UtcNow 
- по историческим свечам генерить сигналы
- подключить эмулятор ордеров
- на выходе - смотреть портфолио по эмулированным операциям

# 2025-06-29

- [x] проверить, что происходит в связки OrderTrades ->  TradeOperations
- [x] запустить и прогнать тест по портфелю - открытие позиции, реверс позиции (сейчас не работает)

# 2025-06-26

## Current

- [x] реализовать PortfolioRepository
- [x] управление активными акаунтами (для получения инфомрации о трейдах из гейтвея) - CRUD операции
- [x] реализовать пересчет портфеля по операциям
- [x] реализовать бутстрап портфелей по операциям при старте
- [x] юнит тесты на идентити портфеля и инструмента
- [x] юнит тесты на обработку операций по позициям
- [x] тесты - забрать операции из гейтвея, преобразовать в доменные, сравнить портфолио
- [x] реализовать работу с акаунтом - при внесенни суммы - отражать в операциях
- реализовать раздельный учет по книгам для портфеля
- привести в порядок гейтвеи - изолировать домен от специфики гейтвея

- реализовать тест-кейсы
  - открытие акаунта и внесение депозита на книжку - должно отражаться в операциях и портфолио
  - открытие позиции
  - реверс позиции несколько раз
  - сравнить портфолио из гейтвея с рассчитанным портфелем (без разделения по книгам)
  - сравнить портфолио из гейтвея с рассчитанным портфелем (с разделением по книгам)


## Next
- реализовать хранение интрадей свечей в маркет дате
- реализовать эмулятор OrderEngine

# 2025-06-24

1. Реализовать операции над портфолио (extension)
- сравнение двух портфолио с формированием дифов по позициям
- мерж (схлопывание) нескольких портфолио в одно (например, несколько книг в одну)

2. Выключить фид по изменению портфеля из TInvest
- [x] Запрашивать Tinvest портфель по необходимости через гейтвей
- [x] Не хранить историю портфеля - ее можно восстанвить по событиям - удалить таблицы с портфелем и позициям
- [x] В order engine убрать вызов Make Snapshot - заменить на GetLast

3. Реализовать расчет и хранение портфеля в Redis
- [x] при старте приложения запрашивать активные акаунты в MDS и восстанавливать по ним портфели через Replay
- [x] при получении события запрашивать портфель (если нет - создавать) и аплаить событие в портфель (extension method)
- [x] реализовать Redis репозиторий для хранения портфеля и позиций (Get, Set)
- [x] продумать схему ключей для портфеля и событий
- [x] реализовать PUB для событий по изменению поотфелей\позиций
- [x] иметь возможность очистки событий по трейдам и ордерам (по книге, акаунту)
- иметь возможность сгенерить историю изменения портфеля (набор снепшотов)

4. [x] Реализовать сценарий валидации расчитанного поотфеля
 - схлопнуть портфель по книжкам
 - запросить эталон из Tinvest
 - сравить портфели, учитывая Initial Amount
 - вывести дифф по позициям

5. Реализовать эмулятор Order Exec Engine
- не выводить заявку в гейтвей, а исполнять по текущей рыночной цене (MDS)
- генерить события по трейдам и ордеру
- эмулировать задержку и исполнение заявки по нескольким ценам
- расчитывать комиссию по исполнению ордера (задавать значение тарифа в конфиге)
- заменять реальный Exec Engine через флажок в конфиге

6. Расчет PNL и статистики по событиям из портфеля и позиций (SUB) часть
- хранение метрик портфеля в REDIS
- алгоритмы расчета
- общий интерфейс доступа
- подключить дашборд Grafana

7. Market Data
- политика хранения ключей
- хранение интрадей свечей
- хранение текущих цен по инструментам (для paper order exec)
- хранение инструментов
- хранение активных акаунтов

8. [x] Перейти на расчеты инструментов по тикеру или ISIN вместо GUID-а



### Сценарий работы хендлера по обработке событий

- десериализовать событие в TradeOperation
- найти существующий портфель по акаунту+книга или создать пустой портфель
- вызвать екстеншен в портфеле по обработке события
    - определить тип операции
    - найти (создать) позицию по инструменту
    - вызвать екстеншен по поизции - зааплаить сумму (количество) по позиции
    - пересчитать метрики портфеля при необходимости (PNL например)
- обновить портфель и/или его позиции в Redis
- сгенерить и опубликовать сигнал по изменнию портфеля\позиции - PUB


# 2025-06-22

## MVP 3.1

- маркет дата сохраняется в Redis
- расчет портфеля и позиция по книгам вручную (без участия tinvest)
- интеграционные, системные тесты по позициям, сверка с tinvest

### Сценарии интеграционных тестов

#### Кейс с одной книгой

- создать акаунт
- завести депозит
- открыть позицию
-- проверить расчет портфеля по операциям
- развернуть позицию
-- проверить расчет портфеля по операциям
- закрыть позицию
-- проверить расчет портфеля по операциям
- закрыть акаунт
- почистить БД

#### Кейс с несколькими книгами по разным инструментам

...

#### Кейс с несколькими книгами по одному инструменту (?)

....

# 2025-06-21

## MDS Controller

- отобразить спиоск инструментов из redis
- отобразить список свечей из redis


# 2025-06-18

## MD Use cases

### Подписка на стримы

- МД хостит подписки для свечей (Тикер.КлассКод + Интервал) в конфиге и кэширует их в Redis?
- Гейтвей при старте запрашивает у МД подписки и подписывается на стрим по нужным свечам
- При получении свечи, гейтвей вызывает хендлер из маркет даты через медиатор
- МД Хендлер сохраняет свечу в БД (Редис?) и нотифицирует о новой свече через броадкаст? (PUB Redis)

* Подписка уникальна для символа. Не может быть двух подписок с одним символом и разными интервалами. *

### Статика по инструментам

- Гейтвей при старте запрашивает у МД подписки 
- Гейтвей запрашивает статику по инструментам из подписки
- Гейтвей вызвает хендлер из маркет даты через медиатор
- МД Хендлер сохраняет(обновляет) статику в БД (Redis?)

### Intraday Market Data

- МД хостит полученные интрадей свечи в Redis 
- МД нотифицирует об изменении маркет даты в PUB Redis
- МД предоставляет API для снепшота интрадей маркет даты из Redis

### Out of Scope 3.1

- Динамическое обновление подписок на стороне Гейтвея
- Синтетика
- Композитные инструменты
- Хостинг и поддержка истории свечей


# 2025-06-16

## MVP 3.1

- [x] работающая машинка без стратегий и автоматических сигналов - на ручном приводе
- [x] архитектура в модульном монолите - без выделенных хостов - один хост на все приложение
- [x] работающий хост с набором контроллеров: гейтвей, портфолио, ордера, маркет дата
- [x] к стрим сервисам подключены хендлеры, в которых логируются сообщения и сохраняются в БД
- [x] ручные операции (постинг ордеров) - проходят по флоу и сохраняются в БД
- маркет дата сохраняется в Redis


# 2025-06-11

```shell
docker-compose pull
docker-compose up --force-recreate --build -d
docker image prune -f
```


# 2025-06-01

## Задачи релиза 3.0

1. Запускать текущие стратегии в микросервисной архитектуре (инфраструктуре)
2. Вести самостоятельный раздельный учет портфеля без использования возможностей TInvest
3. Запусать бэктесты на истории и вести учет портфеля (книжки) по БТ
4. Генерить и использовать синтетические данные (как для БТ так и в реальной торговле - спреды)
5. Управлять стратегиями в динамике 
        - добавлять, удалять
        - запускать, останавливать
        - вести отдельный учет по книжке, привязанной к стратегии
6. Расчитывать и визуализировать KPI портфеля для БТ и в реальной торговле.



# 2025-04-25
- Отключение пулинга для снапшотов операций, портфелей и позиций
- Включение стримов для портфеля, позиций, трейдов и операций (лишнее убрать)

# 2025-03-24

## V3

## Доработка стратегий - питоновская часть

- TF - трешхолд по дисперсии цен на заданном интервале
- TF - питоновский векторный бэктест на интервалах 10min, 5min, 1min - оценка качества модели
- использование показателей объема в предикторах?
- реализация простой регрессионной стратегии (линейной, нелинейной) - по книге
- использование нейронки для регрессии?
- использование логистической регрессии?
- апроксимация нелинейных функций нейронными сетями
- аппроксимация нелинейных функций полиномиальной регрессией
- статистические методы работы с временными рядами? ARIMA? VAR?

## Избавиться от RW

- выключить RW
- вместо RW собирать индикативные данные (индексы) и вычислять коэф шарпа для стратегии по ним

## UI dashboard

- разработать UI концепцию на основе Fluent UI - моки экранов, навигация и т.п.
- управление акаунтами из UI
- аналитика по портфелю в UI
- маркетные данные в UI (персистентные и неперсистентные - Redis?)

## Пайплайн на основе стримов

- уйти от модели расписания
- абстрагировать T-стримы - чтобы можно было, например использовать реквест модель в стримах - с нее можно начать
- подключить T-стримы
- реализовать Event-Driven модель
- нарисовать схему работы со стримами 

## Комбинированная работа sb и live акаунта

- проверить работу системы при комбинированном наборе тестовых и живого акаунта
- выставление ордеров
- получение операций, позиций и т.п.


# 2025-03-??

- [] дашборд - концепция
- [] дописать readme
- [] доработка предиктора - чтобы возвращал данные по другим стратегиям
- [] доработка предиктора - запуск обуечения (в т.ч. по новым алгоритмам) через API
- [] добавить хелсчеки и метрики 


# 2025-03-07

- [x] подключить старую графану к новомой системе и проверить работу системы
- [x] погасить старую систему, удалить акаунт 
- [x] вычистить репозиторий vertr-ml от старого кода
- [-] вернуть тестовый контроллер 
- [x] регресионные тесты
- [x] запуск прод версии ml и платформы и контроль работы по логам


# 2025-03-05

- [x] перевести таблицу свечей в EF Core
- [x] написать скрипт(тесты) по заполнению свечей
- [x] вынести в конфиг настройки предиктора
- [x] сформировать и протестировать конфиг
- [x] запуск под докером совместно с предиктором
- [x] поднять в докере предиктор и торговую систему
 

# 2025-03-02

- [x] пофиксить тесты
- [x] дописать OrderApprovementHandler
- [x] поправить расписание

# 205-03-01

- [x] маппинг AccountStrategySettings - тесты, использовать в хендлерах через инжект IOptions
- [x] протестировать DAL Trading Signal - мапинг статик классов на сущности БД - переделать на енумы

# 2025-02-28

## реализовать оболочку джобы по риск-енжину
- [x] репозиторий и DAL по ордерам (реквест, респонс, ID сигнала)
- считывать таймстамп последнего ордера
- забирать новые сигналы после таймстампа последнего ордера из репозитория
- проверять что ID сигнала нет среди уже отправленных ордеров
- нужен мапинг источника/типа сигнала и акаунта: много сигналов (algo+symbol+interval) - один акаунт (multi-asset) 
- считывать позиции из акаунта и определять направление ордера и количество
- генерить ордер на основе сигнала 
- сохранять реквест, респонс и ID сигнала
- уникальный констрейн в ордерах: ID сигнала + ID акаунта

## MartenDB

- [Understanding Event Sourcing with Marten](https://martendb.io/events/learning.html)
- [Github](https://github.com/JasperFx/marten)
- [EventSourcing .NET](https://github.com/oskardudycz/EventSourcing.NetCore)


# 2025-02-27

- [x] написать тесты для хендлеров портфолио и операций
- [x] продумать политику апдейта операций в бд


# 2025-02-26

## EF Core

- [Getting Started with EF Core](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)
- [NpgSql](https://www.npgsql.org/efcore/)
- [Введение в Entity Framework Core](https://metanit.com/sharp/efcore/1.1.php)

### Migrations

```shell
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate

dotnet ef database update

## revert to 
dotnet ef database update InitialCreate 
```


# 2025-02-25 

- продумать трекинг трейдов от ордера до операций и портфеля (сейчас при исполнении ордера трейды не возвращаются)
- продумать схему хранения акаунта, портфеля, позиций и операций в базе данных
- реализовать джобу по сбору операций
- реализовать джобу по сбору снапшотов портфеля и позиций


# 2025-02-24

- [x] написать тесты на имплементацию новых методов тинвест
- [x] проверить правильность маппинга енумов и сущностей


# 2025-02-17

## TODO

- - tinvest extensions
- - get positions
  - get operations
  - exec order

# 2025-02-15

- [x] signal generator + job

# 2025-02-14

- [x] prediction API client: models+refit

## Http files

- [Use .http files in Visual Studio 2022](https://learn.microsoft.com/en-us/aspnet/core/test/http-files?view=aspnetcore-9.0)
- [Использование HTTP-файлов в Visual Studio 2022](https://learn.microsoft.com/ru-ru/aspnet/core/test/http-files?view=aspnetcore-9.0)
- [What is Api.http file in .NET 8](https://stackoverflow.com/questions/78119582/what-is-api-http-file-in-net-8)
- [.http Files Explained](https://www.rahulpnath.com/blog/http-files-asp-net-core-dotnet/)

## Docker

- [Запуск приложения ASP.NET Core в контейнерах Docker](https://learn.microsoft.com/ru-ru/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-9.0)
- [dotnet-docker](https://github.com/dotnet/dotnet-docker)
- [Tutorial: Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows&pivots=dotnet-9-0)
- [Run an ASP.NET Core app in Docker containers](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-9.0)
- [Containerize a .NET app with dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/containers/sdk-publish)
- [Compose sample application: ASP.NET with MS SQL server database](https://github.com/docker/awesome-compose/tree/master/aspnet-mssql)
- [Use containers for .NET development](https://docs.docker.com/guides/dotnet/develop/)
- [Building a Multi-Container .NET App Using Docker Desktop](https://www.docker.com/blog/building-multi-container-net-app-using-docker-desktop)

### TODO

- [x] собрать докер имидж
- [x] конфигурация и переменные окружения, порты
- [x] собрать docker compose

``` schell
docker network connect vertr-ml_default infra-pgsql-1
```


# 2025-02-13

## TODO

- добавить конфиг секкцию для джобов
- - для свечей в конфиге указывать список тикеров и интервал
  - забирать данные из конфига при формировании медиатор реквеста

- [x] tinvest options
- - [x] добавить раздел с маппингами символов в uuid-ы
  - [x] использовать сеттинги для загрузки свечей
  - [x] юнит тесты на маппинги
- appliaction:
- - [x] add repo
  - [x] get symbols from config
  - [x] add tinvest gateway
  - [x] get last candle
  - [x] request candles
  - [x] load candles


 
# 2025-02-12

## Quartz

- [GitHub](https://github.com/quartznet/quartznet)
- [Docs](https://www.quartz-scheduler.net/)
- [DI integration](https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html)

### Cron
- https://www.freeformatter.com/cron-expression-generator-quartz.html
- http://www.cronmaker.com/
- 


## Dapper

- [github](https://github.com/DapperLib/Dapper)
- [Dapper: опыт применения](https://habr.com/ru/articles/665836/)
- [PostgreSQL CRUD operations with C# and Dapper](https://www.code4it.dev/blog/postgres-crud-dapper/)
- [How to handle postgresql db connections with dapper using dependency injection](https://stackoverflow.com/questions/58929543/how-to-handle-postgresql-db-connections-with-dapper-using-dependency-injection-i)
- [Connect to Postgres in C# with Dapper and ADO.NET](https://jasonwatmore.com/net-7-postgres-connect-to-postgresql-database-with-dapper-in-c-and-aspnet-core)
- [Configuring Dapper to map snake_case results](https://andrewlock.net/using-snake-case-column-names-with-dapper-and-postgresql/#configuring-dapper-to-map-snake_case-results)

# 2025-02-10

## Tinvest

- [T-Invest API](https://github.com/RussianInvestments/investAPI)
- [docs](https://russianinvestments.github.io/investAPI/)
- [csharp-sdk](https://github.com/RussianInvestments/invest-api-csharp-sdk)



# 2025-02-09

### Modular monolith

- [booking-modular-monolith](https://github.com/meysamhadeli/booking-modular-monolith)
- [A Practical Guide to Modular Monoliths with .NET](https://chrlschn.dev/blog/2024/01/a-practical-guide-to-modular-monoliths/)
- [modular-monolith-with-ddd](https://github.com/kgrzybek/modular-monolith-with-ddd)
- [mm-articles](https://awesome-architecture.com/modular-monolith/#articles)
- [RiverBooks](https://github.com/ardalis/RiverBooks)


## Market data service
- клиент Т-инвест API
- бэкграунд сервис для загрузки свечей
- конфигурация сервиса - интервалы, символы, расписание
- модель данных
- DAL на Dapper (EF Core?)
- юнит и интеграционные тесты
- aspire для хостинга
- развертывание в Docker

## навести порядок в обвязке солюшена
- editor.confg
- правила форматирования
- props в проектах

# 2025-01-15

## Microsoft MarS

- [MarS: A Financial Market Simulation Engine](https://github.com/microsoft/MarS)

## Microsoft Qlib

- [Qlib: An AI-oriented Quantitative Investment Platform](https://arxiv.org/abs/2009.11189)
- [Qlib: Github](https://github.com/microsoft/qlib)

# 2025-01-13

## ASP.NET Aspire

- https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview
- https://github.com/dotnet/aspire
- https://github.com/dotnet/aspire-samples
- [.NET Aspire — империя дотнета наносит ответный удар](https://habr.com/ru/articles/818907/)
- [Переход на .NET Aspire из отдельных проектов. Часть 1. Перевод приложений в Aspire](https://habr.com/ru/articles/820371/)
- [Переход на .NET Aspire из отдельных проектов. Часть 2. Локальное развертывание с помощью Aspire](https://habr.com/ru/articles/820435/)








