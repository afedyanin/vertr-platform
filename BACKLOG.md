# 2025-03-02

- [x] пофиксить тесты
- [] дописать OrderApprovementHandler
- [] поправить расписание
- [] сформировать и протестировать конфиг
- [] регресионные тесты
- [] запуск под докером совместно с предиктором
- [] доработка предиктора - чтобы возвращал данные по другим стратегиям
- [] доработка предиктора - запуск обуечения (в т.ч. по новым алгоритмам) через API
- [] дашборд - концепция


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








