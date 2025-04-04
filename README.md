# Vertr Platform

## VERTR 

Платформа алгоритмической торговли

- vertr-ml: предсказательное ядро системы
- vertr: инфраструктура, бэкенд и управление торговлей


## Подготовка и запуск платформы

### Создать один или несколько тестовых акаунтов в песочнице

Использовать класс с тестами - TinvestSandboxRelatedTests для

- создания акаунта - CreateSandboxAccount
- создания депозита на акаунте - DepositSandboxAccount
- просмотра списка доступных акаунтов - GetAllAccounts
- закрытия акаунта CloseSandboxAccount

### Подготовить конфигурационный файл для стратегии торговли

Для каждого акаунта указывается массив стратегий с параметрами торговли.
Пример секции в appsettings.json для одного акаунта:

```json

  "AccountStrategySettings": {
    "SignalMappings": {
      "0fde9e6e-7bb6-4c73-b7ae-629791aa2cf6": [
        {
          "Symbol": "SBER",
          "Interval": "_10Min",
          "QuantityLots": 10,
          "PredictorType": "Sb3",
          "Sb3Algo": "DQN"
        }
      ]
    }
  },

```

Для заданного акаунта торговля осуществляется по единственной стратегии акциями Сбербанка на 10-минутном интервале в количестве 10 лотов.
Для торговли используется предиктор Sb3 с алгоритмом DQN.

### Поднять в докере инфраструктурный слой

В репозитории vertr-ml в командной строке перейти в каталог "\vertr-ml\infra" и запустить инфраструктурный контейнер командой:

```shell

docker compose up

```

Проверить подключение к pgSQL любым клиентом, например, pgAdmin. 

### Создание БД

- в pgsql создать пустую БД, назвав ее, например, vertr

- установить строку подключения к бд в файле миграции - VertrDbContextFactory.cs

- установить строку подключения к бд в секции ConnectionStrings/VertrDbConnection в appsettings.json

- в командой строке перейти в каталог "vertr-platform\src\Vertr.Adapters.DataAccess" и запустить скрипт миграции

```shell

dotnet tool install --global dotnet-ef

dotnet ef database update

```

### Заполнение БД историческими данными

Система периодически обновляет исторические свечи для заданных в конфиге символов и периодов.

Глубина загрузки истории при обновлении свечей задана в 5 дней.

Если необходимо прогрузить в БД исторические данные за больший период, это можно сделать вручную через запуск теста LoadHistoricCandles.cs


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






