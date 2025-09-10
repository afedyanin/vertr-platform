# VERTR 

Платформа алгоритмической торговли

- vertr-platform: инфраструктура, бэкенд и управление торговлей
- vertr-ml: предсказательное ядро системы

## Функционал релиза 3.1

...


## Подготовка и запуск

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

Система использует API T-инвестиции в качестве источника рыночных данных и движка для выставления ордеров.

Для подкючения API необходимо

- указать ключ в конфиге
- указать режим работы в конфиге (sandbox или реальный акаунт)
- указать AccountId в конфиге

Для тестового режима работы (песочницы)

- создать sandbox акаунт
- внести сумму депозита для сэндбокс акаунта 

