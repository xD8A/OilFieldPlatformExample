# ENTRYPOINT

## О проекте

**OilFieldPlatform** — пример платформы для работы с базами данных нефтегазовых месторождений. Реализует базовый слой: справочники (месторождения, скважины, объекты разработки, кустовые станции), инфраструктуру доступа к данным (NHibernate, PostgreSQL/SQLite), репозитории, аудит, AutoMapper.

**OilFieldPlatform.Calculation** — пример модуля расчётов на этой платформе: модель проекта, реактивные пробы воды с `BehaviorSubject`/`IObservable`, расчёт эквивалентных концентраций ионов (мг-экв/л), WebSocket-сервер, Redis-сессии, Vue.js-клиент.

## Технологический стек

| Технология | Назначение | Версия |
|---|---|---|
| .NET | Платформа | 9.0 |
| C# | Язык | 12+ |
| ASP.NET Core | Web-сервер + WebSocket | 9.0 |
| NHibernate + FluentNHibernate | ORM | 5.6.1 / 3.4.1 |
| PostgreSQL / SQLite | СУБД | — |
| Redis | Кеширование / сессии | — |
| StackExchange.Redis | Клиент Redis | 2.8.31 |
| AutoMapper | Маппинг объектов | 15.1.1 |
| System.Reactive (Rx.NET) | Реактивное программирование | 6.1.0 |
| DynamicData | Реактивные коллекции | 9.4.31 |
| NLog | Структурированное логирование | 5.4.0 |
| SonarAnalyzer | Статический анализ кода | 10.9.0 |
| Vue 3 + Vite | Фронтенд | — |
| TypeScript | Типизация фронтенда | — |

## Архитектура

Платформа разделена на слои:

```
OilFieldPlatform.Domain                  # Слой домена: сущности, интерфейсы, перечисления, проекции
    ↑
OilFieldPlatform.Infrastructure          # Слой инфраструктуры: NHibernate-маппинги, репозитории, настройки
    ↑
OilFieldPlatform.Calculation.Core        # Бизнес-логика расчётного модуля
    ↑
OilFieldPlatform.Calculation.Server      # ASP.NET Core хост: WebSocket, контроллеры, Redis-сессии
    ↑
OilFieldPlatform.Calculation.WebClient   # Vue 3 + TypeScript фронтенд
```

- **Domain** — чистый .NET, без внешних зависимостей. Сущности с `virtual`-свойствами (NHibernate lazy loading), интерфейсы `IEntity<T>`, `IAuditable`, `ICationSample`, `IAnionSample`, `IRecord`, перечисления `WaterType`, `ClusterStationType`.
- **Infrastructure** — NHibernate-маппинги (`ClassMap<T>`), репозитории (базовый `ABCRepository<T>` с CQRS-light: ReadRepository / Repository), провайдеры БД (SQLite InMemory, SQLite файловая, PostgreSQL), NHibernate-слушатели для аудита (`AuditableListener`), настройки БД/Redis.
- **Calculation.Core** — реактивные модели (`ProjectModel`, `WaterSampleModel`, `WaterSampleEquivalentData`), сервисы (`ManageProjectService`, `ListProjectService`, `WaterSampleEquivalentCalculator`), AutoMapper-профили (`ProjectProfile`), прокси-модели для UI (`ProjectProxyModel`, `WaterSampleProxyModel`), состояние приложения (`ApplicationState`).
- **Calculation.Server** — ASP.NET Core minimal API, WebSocket-эндпоинт (`/ws`), контроллеры запросов/ответов (`ApplicationController`, `WaterSamplePageController`), сериализуемые JSON Schema (typed request/response через `IWebSocketResponse` с `JsonDerivedType`), сервис управления сессиями (`AppStateLoader`), логгирование в WebSocket (`LoggerForwarder`).
- **Calculation.WebClient** — Vite + Vue 3 + TypeScript, typed API-слой (`api/ws.ts`), frontend-схемы запросов/ответов (`api/schemas/`), компоненты (`WaterSampleTable`, `LogPanel`), страницы (`WaterSampleCalcPage`).

## Начало работы

### Требования

- [Visual Studio Code](https://code.visualstudio.com/) с расширением **Dev Containers**
- [Docker](https://www.docker.com/products/docker-desktop/)

### Запуск в Dev Container

Рекомендованный способ разработки — через **VS Code Dev Container**. Конфигурация в `.devcontainer/`.

1. Открыть проект в VS Code
2. Выполнить команду: **Dev Containers: Reopen in Container**
3. Контейнер автоматически:
   - Развернёт .NET 9 SDK, Redis, PostgreSQL
   - Создаст БД и схему `acme` с пользователем `calculation_module`
   - Выполнит `dotnet restore`

После запуска контейнера Redis и PostgreSQL стартуют автоматически.

### Альтернативный запуск (без Dev Container)

```bash
# Установка .NET 9 SDK (если отсутствует)
# https://dotnet.microsoft.com/download/dotnet/9.0

# Восстановление зависимостей
dotnet restore

# Сборка
dotnet build
```

### Настройка Git Hooks

```bash
git config core.hooksPath .githooks
```

Хук `pre-commit` автоматически запускает `dotnet format` и `dotnet build` перед каждым коммитом.

## Структура проекта

```
OilFieldPlatform.sln
Directory.Build.props          # Общие настройки сборки (warnings as errors, SonarAnalyzer, XML doc)
.editorconfig                  # Стиль кода
.githooks/pre-commit           # Pre-commit hook

OilFieldPlatform.Domain/
├── Entities/
│   ├── ABC/
│   │   ├── ABCEntity.cs              # Базовый класс: Id (Guid)
│   │   └── ABCNamedEntity.cs         # Базовый класс: Id + Name
│   ├── Common/
│   │   ├── OilFieldEntity.cs         # Месторождение
│   │   ├── WellEntity.cs             # Скважина
│   │   ├── DevTargetEntity.cs        # Объект разработки
│   │   ├── WaterSampleEntity.cs      # Проба воды (катионы/анионы/тип)
│   │   └── ClusterStationEntity.cs   # КНС / ДНС
│   └── Calculation/
│       ├── CalcProjectEntity.cs      # Проект расчёта
│       ├── CalcWaterSampleEntity.cs  # Проба воды в проекте
│       └── Data/
│           └── CalcWaterSampleEquivalentRecord.cs  # Эквивалентные концентрации
├── Enums/
│   ├── ClusterStationType.cs         # KNS / DNS
│   └── WaterType.cs                  # Reservoir / Injection
├── Interfaces/                       # IEntity<T>, IAuditable, IAnionSample, ICationSample, IRecord и др.
└── Projections/                      # DTO для read-only запросов
    ├── Common/                       # OilFieldProjection, WellProjection и др.
    └── Calculation/                  # CalcProjectProjection

OilFieldPlatform.Infrastructure/
├── Extensions/
├── Mapping/                          # NHibernate ClassMap<T> для всех сущностей
│   ├── Common/                       # OilFieldMap, WellMap и др.
│   └── Calculation/                  # CalcProjectMap, CalcWaterSampleMap, CalcWaterSampleEquivalentRecordMap
├── Providers/
│   ├── DbConfigProvider.cs           # Фабрика SessionFactory (SQLite/PostgreSQL/InMemory)
│   ├── UserNameProvider.cs           # Определение текущего пользователя
│   ├── AuditableListener.cs          # NHibernate event listener для аудита
│   └── DbListenerRegistry.cs
├── Repositories/
│   ├── ABC/
│   │   ├── ABCRepository.cs          # Базовый CRUD
│   │   └── ABCReadRepository.cs      # Базовый read-only
│   ├── Common/                       # OilFieldRepository, WellRepository и др.
│   └── Calculation/                  # CalcProjectRepository
└── Settings/                         # DbSettings

OilFieldPlatform.Calculation.Core/
├── Mapping/
│   └── ProjectProfile.cs            # AutoMapper профиль (Domain Entity ↔ Model)
├── Models/
│   ├── OilFieldModel.cs              # Модель месторождения
│   ├── DevTargetModel.cs             # Модель объекта разработки
│   ├── WaterSampleModel.cs           # Реактивная модель пробы воды (BehaviorSubject<double?>)
│   ├── ProjectModel.cs               # Реактивная модель проекта (SourceList<WaterSampleModel>)
│   └── Data/
│       └── WaterSampleEquivalentData.cs  # Модель эквивалентов (BehaviorSubject)
├── Proxies/
│   ├── ProjectProxyModel.cs          # Прокси для UI (ApplicationHeaderState)
│   └── WaterSampleProxyModel.cs      # Прокси для UI (список проб)
├── Services/
│   ├── Calculators/
│   │   └── WaterSampleEquivalentCalculator.cs  # Расчёт мг-экв/л
│   ├── ListProjectService.cs         # Список проектов
│   └── ManageProjectService.cs       # CRUD для проектов
└── States/
    ├── ApplicationState.cs           # Состояние приложения (текущий проект)
    └── UI/
        ├── ApplicationHeaderState.cs  # UI-состояние заголовка
        └── WaterSamplePageState.cs    # UI-состояние страницы проб (SourceList<WaterSampleProxyModel>)

OilFieldPlatform.Calculation.Server/
├── Program.cs                        # Точка входа: DI, WebSocket /ws, статика
├── appsettings.json                  # Конфигурация (БД, Redis, NLog)
├── Controllers/
│   ├── IWebSocketController.cs       # Интерфейс контроллера
│   ├── ApplicationController.cs      # Контроллер приложения (проекты, состояние, isChanged)
│   └── WaterSamplePageController.cs  # Контроллер страницы проб (редактирование, расчёт, connect)
├── Schemas/
│   ├── IWebSocketRequest.cs          # Базовый интерфейс запроса
│   ├── IWebSocketResponse.cs         # Базовый интерфейс ответа (JsonDerivedType + TypeDiscriminator)
│   ├── IApplicationRequest.cs
│   ├── IApplicationResponse.cs
│   ├── IWaterSamplePageRequest.cs
│   ├── IWaterSamplePageResponse.cs
│   ├── Requests/                     # Типизированные запросы (11 файлов)
│   └── Responses/                    # Типизированные ответы (14 файлов)
└── Services/
    ├── WebSocketService.cs           # WebSocket-хост: маршрутизация, сессии, автосохранение
    ├── AppStateLoader.cs             # Сохранение/восстановление состояния через Redis
    └── LoggerForwarder.cs            # Прокси-логгер: Warning+ → WebSocket

OilFieldPlatform.Calculation.WebClient/
├── index.html
├── vite.config.ts
├── tsconfig.json
├── src/
│   ├── main.ts
│   ├── App.vue                       # Корневой компонент
│   ├── assets/main.css               # Стили (CSS custom properties)
│   ├── api/
│   │   ├── ws.ts                     # typed WebSocket API (send/on для каждого типа)
│   │   └── schemas/
│   │       ├── requests/             # 11 файлов + index.ts с discriminated union
│   │       └── responses/            # 12 файлов + index.ts с discriminated union
│   ├── composables/
│   │   └── useWebSocket.ts           # WebSocket composable (connect, send, on, sessionId)
│   ├── components/
│   │   ├── AppHeader.vue
│   │   ├── ProjectDialog.vue
│   │   ├── WaterSampleTable.vue      # Таблица проб с radio + pie-chart
│   │   └── LogPanel.vue              # Панель лога сервера
│   └── pages/
│       └── WaterSampleCalcPage.vue   # Страница расчёта проб
```

## Ключевые архитектурные решения

### Репозитории (CQRS-light)

Для каждой сущности существует два класса:

- **ReadRepository** (`OilFieldReadRepository`, `WellReadRepository` и т.д.) — только чтение: `GetAll()`, `GetById()`, `Load()`, `GetProjections()`.
- **Repository** (`OilFieldRepository`, `WellRepository` и т.д.) — наследует ReadRepository, добавляет запись: `Add()`, `Update()`, `Delete()`. Методы записи скрыты через `new`.

### Аудит

Сущности, реализующие `IAuditable`, автоматически заполняют поля `CreatedAt`/`UpdatedAt`/`CreatedBy`/`UpdatedBy` через NHibernate event listener `AuditableListener`. Пользователь определяется через `UserNameProvider` (AsyncLocal).

### Реактивные модели

`WaterSampleModel` использует `BehaviorSubject<double?>` для каждой концентрации иона. При изменении любого иона `WaterSampleEquivalentCalculator` пересчитывает эквиваленты. `ProjectModel` использует `DynamicData.SourceList<WaterSampleModel>` для коллекции проб.

### WebSocket + типизированные схемы

Сервер использует `[JsonDerivedType(TypeDiscriminatorPropertyName = "type")]` для полиморфной сериализации/десериализации запросов и ответов. Дискриминатор `type` автоматически проставляется при сериализации через `IWebSocketResponse`. На фронтенде — аналогичные discriminated union-типы в TypeScript.

### Управление сессиями

При подключении клиент передаёт `sessionId` через query-параметр `ws?sessionId=...`. Сервер восстанавливает состояние (открытый проект, пробы) из Redis. Если `sessionId` не передан — генерируется новый UUID. Состояние сохраняется:
- При изменении проекта (`ProjectAsObservable` / `IsChangedAsObservable`, throttle 2s)
- Каждую минуту в цикле `HandleAsync`
- При закрытии соединения

Сессия хранит полный слепок несохранённого проекта (OilField, DevTarget, WaterSamples, эквиваленты) через snapshot-модели.

### WebSocket-логгер

`LoggerForwarder` оборачивает `ILogger<WaterSamplePageController>` и отправляет сообщения уровня Warning+ через `IWebSocketController.PublishLog()` → `OnChanged` → WebSocket. Фронтенд показывает последние 10 записей в `LogPanel`.

### NHibernate особенности

- Генерация ID через последовательности PostgreSQL: `GeneratedBy.Sequence("oil_fields_seq")`
- One-to-one связь `CalcWaterSampleEquivalentRecord` через `GeneratedBy.Foreign()` + `PropertyRef`
- Enum-свойства маппятся через `CustomType<T>()` в `smallint`
- Каскадирование: `Cascade.AllDeleteOrphan()` для коллекций
- Audit через `IPreInsertEventListener` / `IPreUpdateEventListener`

### Маппинг (AutoMapper)

`ProjectProfile.cs` маппит доменные сущности в расчётные модели. Для маппинга NHibernate-прокси (Entity → Model) требуется передача `ISession` через `Items`-словарь контекста.

## Сборка и линтинг

```bash
# Сборка всех проектов
dotnet build

# Форматирование кода
dotnet format

# Полная проверка с SonarAnalyzer
dotnet build --no-restore

# Сборка фронтенда
cd OilFieldPlatform.Calculation/OilFieldPlatform.Calculation.WebClient
npm run build
```

В `Directory.Build.props` включены:
- `TreatWarningsAsErrors` — все предупреждения как ошибки
- `EnforceCodeStyleInBuild` — проверка стиля кода при сборке
- `GenerateDocumentationFile` — обязательные XML-комментарии (CS1591 → ошибка)
- `AnalysisLevel latest-Recommended` — максимальный уровень анализа

## Замечания

- Тестовые проекты отсутствуют.
- CI/CD не настроен.
- Требуется локальный Redis для работы сессий (конфигурация `Redis:Host`/`Redis:Port` в `appsettings.json`).
