# Market Data

## Назначение

- загрузка и хранение исторических данных (свечи)
- генерация и хранение синтетических данных (через Python Fast API и, возможно Kafka)
- получение и публикация интрадей данных и обеспечение их целостности
- управлять справочниками по инструментам (лотность)
- управлять конфигурацией загрузки свечей (для TinvestGateway)

Чтобы:
- обеспечивать генерацию торговых сигналов стратегий в реальном времени
- обеспечивать обучение оракула на обновленных исторических данныъ
- обеспечивать прогон бэктестов на истории

## BACKLOG

### 2025-06-11

#### Redis TimeSeries

- [GitHub](https://github.com/RedisTimeSeries/RedisTimeSeries)
- [Using Redis Timeseries to store and analyze timeseries data](https://medium.com/datadenys/using-redis-timeseries-to-store-and-analyze-timeseries-data-c22c9e74ff46)


### 2025-06-06

#### Интрадей

- При старте - проверить наличие свечей на глубину. Если недостаточно - загрузить
- Слушать кафку и при получении данных опубликовать свечу в Redis
- Сформировать и опубликовать в Кафку сигнал о приходе новых маркет данных для стратегии
- Поддерживать целостность набора маркет данных в Redis
- Optional - отображение в Grafana маркет данных из Redis

#### TODO

- C# клиент для Redis - тесты
- схема нейминга ключей для хранения маркет данных
- консьюмер свечей из TInvestGateway:
    - формирование и публикация сигнала для стратегий
    - публикация свечи в Redis 
- восстановление данных при старте системы
- очистка данных и экспирация ключей

#### Redis

- [NRedisStack guide (C#/.NET)](https://redis.io/docs/latest/develop/clients/dotnet/)
- [How to connect to Redis in a C# .NET project using the NRedisStack client library](https://redis.io/kb/doc/1z6deydshs/how-to-connect-to-redis-in-a-c-net-project-using-the-nredisstack-client-library)
- [NRedisStack](https://github.com/redis/NRedisStack)
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)
- [C#/.NET guide](https://redis.github.net.cn/docs/latest/develop/connect/clients/dotnet/)

#### Habr

- [REDIS: такой простой и такой сложный](https://habr.com/ru/companies/stm_labs/articles/841792/)


### 2025-06-01

- Статик дата: информация по инструментам
- Статик дата: конфигурация для загрузки свечей для TinvestGateway
- Свечи: получение и загрузка в БД (Kafka consumer)
- Свечи: получение недостающих или пропущенных свечей и загрузка в БД 
- Синтетика: запрос на генерацию и загрузка в БД
- API: доступные свечи, интервалы и диапазоны дат по ним
- API: реплей свечей через продюсер Кафки (backtest)

### Идеи

- хранить отдельно исторические данные и интрадей
- интрадей загружать и обновлять отдельно от исторических данных




