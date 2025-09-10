# VERTR 

Платформа алгоритмической торговли

- vertr-platform: инфраструктура, бэкенд и управление торговлей
- vertr-ml: предсказательное ядро системы

Платформа использует API T-инвестиции в качестве источника рыночных данных и движка для выставления ордеров.

## Функционал релиза 3.0

### Маркет данные

- поиск и добавление инструментов
- подписки на маркет данные в онлайн режиме
- загрузка интрадей и исторических свечей в фоновом режиме

### Стратегии

- создание, активация/деактивация стратегий в онлайн режиме
- ведение отдельного портфолио и позиций по каждой стратегии

### Ордер менеджмент

- поддержка режима симуляции исполнения ордеров
- управление позициями через выставление рыночных ордеров

### Портфели

- поддержка отдельных портфелей для каждой стратегии
- поддержка отдельных портфелей для каждого бэктеста
- индивидуальный расчет поизций по каждому портфелю

### Бэктесты

- фоновая загрузка исторических данных для бэктеста
- поддержка полного расчета позиций по бэктесту в отдельном портфеле 

### 

## Подготовка и запуск

### Поднять в докере инфраструктурный слой

В репозитории vertr-ml в командной строке перейти в каталог "\vertr-ml\infra" и запустить инфраструктурный контейнер командой:

```shell

docker compose up

```

Проверить подключение к pgSQL любым клиентом, например, pgAdmin. 

### Создание БД и структуры таблиц

- в pgsql создать пустую БД, назвав ее, например, vertr

- установить строку подключения к бд в файле миграции - https://github.com/afedyanin/vertr-platform/blob/main/src/Vertr.Infrastructure.Pgsql.Migrations/ConnectionStrings.cs

- установить строку подключения к бд в appsettings.json - https://github.com/afedyanin/vertr-platform/blob/main/src/Vertr.Platform.Host/appsettings.json

- в командой строке перейти в каталог "https://github.com/afedyanin/vertr-platform/tree/main/src/Vertr.Infrastructure.Pgsql.Migrations" и запустить скрипты миграции

```shell

dotnet tool install --global dotnet-ef

dotnet ef database update --context BacktestDbContext
dotnet ef database update --context MarketDataDbContext
dotnet ef database update --context OrderExecutionDbContext
dotnet ef database update --context PortfolioDbContext
dotnet ef database update --context StrategiesDbContext

```

### Создание БД Hangfire 

Плаформа использует hangfire для запуска фоновых задач.

Необходимо создать БД для hangfire и указать строку подключения в appsettings.json - https://github.com/afedyanin/vertr-platform/blob/main/src/Vertr.Platform.Host/appsettings.json


### Стартовать контейнер с Prediction Service

В репозитории vertr-ml в командной строке перейти в корневой каталог и запустить контейнер командой:

```shell

docker compose build

docker compose up -d

```

Потребуется установить видимость ендпоинтов

``` schell
docker network ls

docker network connect vertr_ml_v1_default infra-pgsql-1
docker network connect vertr_platform_v1_default vertr-ml
docker network connect vertr_platform_v1_default infra-pgsql-1

```

### Стартовать контейнер с сервисом платформы

В репозитории vertr-platform в командной строке перейти в корневой каталог и запустить контейнер командой:

```shell

docker compose build
docker compose up -d

```

### Подключение к T-Invest

Для подкючения API необходимо

- указать ключ в конфиге
- указать режим работы в конфиге (sandbox или реальный акаунт)
- указать AccountId в конфиге

Для тестового режима работы (песочницы)

- создать sandbox акаунт
- внести сумму депозита для сэндбокс акаунта 

Примеры можно найти в файле - 



