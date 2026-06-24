# ENTRYPOINT

## О проекте

**OilFieldPlatform** — система расчёта ионного состава пластовых и закачиваемых вод по нефтегазовым месторождениям. Проект реализует предметную область нефтепромысловых лабораторных исследований: ведение справочников (месторождения, скважины, объекты разработки, кустовые станции), учёт проб воды, расчёт эквивалентных концентраций ионов (мг-экв/л) и управление проектными расчётами.

## Технологический стек

| Технология | Назначение | Версия |
|---|---|---|
| .NET | Платформа | 9.0 |
| C# | Язык | 12+ |
| NHibernate + FluentNHibernate | ORM | 3.4.1 |
| PostgreSQL / SQLite | СУБД | — |
| Redis | Кеширование | — |
| AutoMapper | Маппинг объектов | 15.1.1 |
| System.Reactive (Rx.NET) | Реактивное программирование | 6.1.0 |
| DynamicData | Реактивные коллекции | 9.4.31 |
| SonarAnalyzer | Статический анализ кода | 10.9.0 |

## Архитектура

Проект разделён на три слоя, каждый в отдельной сборке:

```
OilFieldPlatform.Domain                  # Слой домена: сущности, интерфейсы, перечисления, проекции
    ↑
OilFieldPlatform.Infrastructure          # Слой инфраструктуры: NHibernate-маппинги, репозитории, настройки
    ↑
OilFieldPlatform.Calculation.Core        # Слой бизнес-логики: сервисы, расчёты, модели, AutoMapper
```

- **Domain** — чистый .NET, без внешних зависимостей. Содержит сущности с `virtual`-свойствами (NHibernate lazy loading), интерфейсы `IEntity<T>`, `IAuditable`, `ICationSample`, `IAnionSample` и перечисления.
- **Infrastructure** — реализация доступа к данным: NHibernate-маппинги (`ClassMap<T>`), репозитории (базовый `ABCRepository<T>` с CRUD), провайдеры БД (SQLite InMemory, SQLite файловая, PostgreSQL), NHibernate-слушатели для аудита.
- **Calculation.Core** — расчётный движок: сервис управления проектами (`ManageProjectService`), калькулятор эквивалентов (`WaterSampleEquivalentCalculator`), реактивные модели с `BehaviorSubject`/`IObservable`, AutoMapper-профили.

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
│       └── CalcWaterSampleEntity.cs  # Проба воды в проекте
│       └── Data/
│           └── CalcWaterSampleEquivalentRecord.cs  # Эквивалентные концентрации
├── Enums/
│   ├── ClusterStationType.cs         # KNS / DNS
│   └── WaterType.cs                  # Reservoir / Injection
├── Interfaces/                       # IEntity<T>, IAuditable, IAnionSample, ICationSample и др.
└── Projections/                      # DTO для read-only запросов

OilFieldPlatform.Infrastructure/
├── Mapping/                          # NHibernate ClassMap<T> для всех сущностей
├── Providers/
│   ├── DbConfigProvider.cs           # Фабрика SessionFactory (SQLite/PostgreSQL/InMemory)
│   ├── UserNameProvider.cs           # Определение текущего пользователя
│   ├── AuditableListener.cs          # NHibernate event listener для аудита
│   └── DbListenerRegistry.cs
├── Repositories/                     # ABCRepository<T> + ReadRepository / Repository для каждой сущности
└── Settings/                         # AppSettings, DbSettings, RedisSettings

OilFieldPlatform.Calculation.Core/
├── Mapping/
│   └── ProjectProfile.cs            # AutoMapper профиль (Domain ↔ Model)
├── Models/
│   ├── OilField.cs                   # Модель месторождения
│   ├── DevTarget.cs                  # Модель объекта разработки
│   ├── WaterSample.cs                # Реактивная модель пробы воды
│   ├── Project.cs                    # Реактивная модель проекта
│   └── Data/
│       └── WaterSampleEquivalent.cs  # Модель эквивалентов
└── Services/
    ├── Calculators/
    │   ├── ISubjectCalculator.cs      # Интерфейс калькулятора
    │   └── WaterSampleEquivalentCalculator.cs  # Расчёт мг-экв/л
    └── ManageProjectService.cs       # CRUD для проектов
```

## Ключевые архитектурные решения

### Репозитории (CQRS-light)

Для каждой сущности существует два класса:

- **ReadRepository** (`OilFieldReadRepository`, `WellReadRepository` и т.д.) — только чтение: `GetAll()`, `GetById()`, `Load()`, `GetProjections()`.
- **Repository** (`OilFieldRepository`, `WellRepository` и т.д.) — наследует ReadRepository, добавляет запись: `Add()`, `Update()`, `Delete()`. Методы записи скрыты через `new`, так как базовый класс `ABCRepository` объявляет их как `protected`.

### Аудит

Сущности, реализующие `IAuditable`, автоматически заполняют поля `CreatedAt`/`UpdatedAt`/`CreatedBy`/`UpdatedBy` через NHibernate event listener `AuditableListener`. Пользователь определяется через `UserNameProvider` (AsyncLocal → ClaimsPrincipal → Environment.UserName).

### Реактивные модели

`WaterSample` использует `BehaviorSubject<double?>` для каждой концентрации иона. После изменения любого иона `WaterSampleEquivalentCalculator` пересчитывает эквиваленты. Подписки осуществляются через `IObservable<double?>`.

`Project` использует `DynamicData.SourceList<WaterSample>` для управления коллекцией проб.

### NHibernate особенности

- Генерация ID через последовательности PostgreSQL: `GeneratedBy.Sequence("oil_fields_seq")`
- One-to-one связь `CalcWaterSampleEquivalentRecord` через `GeneratedBy.Foreign()` + `PropertyRef`
- Enum-свойства маппятся через `CustomType<T>()` в `smallint`
- Каскадирование: `Cascade.AllDeleteOrphan()` для коллекций
- Audit через `IPreInsertEventListener` / `IPreUpdateEventListener`

### Маппинг (AutoMapper)

`ProjectProfile.cs` маппит между доменными сущностями и расчётными моделями. Для маппинга NHibernate-прокси (Entity → Model) требуется передача `ISession` через `Items`-словарь контекста маппинга.

## Сборка и линтинг

```bash
# Сборка всех проектов
dotnet build

# Форматирование кода
dotnet format

# Полная проверка с SonarAnalyzer
dotnet build --no-restore
```

В `Directory.Build.props` включены:
- `TreatWarningsAsErrors` — все предупреждения как ошибки
- `EnforceCodeStyleInBuild` — проверка стиля кода при сборке
- `GenerateDocumentationFile` — обязательные XML-комментарии (CS1591 → ошибка)
- `AnalysisLevel latest-Recommended` — максимальный уровень анализа

## Замечания

- Проектные файлы — только библиотеки классов. Хост-приложение (веб/консоль/WebSocket) пока отсутствует.
- Тестовые проекты отсутствуют.
- CI/CD не настроен.
