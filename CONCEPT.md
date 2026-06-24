# Архитектурная концепция: универсальная платформа для толстого и тонкого клиента

## Идея

**OilFieldPlatform** спроектирована так, чтобы один и тот же набор моделей, сервисов и инфраструктуры мог обслуживать **оба сценария развёртывания** — классическое десктопное приложение (толстый клиент, WPF) и современное веб-приложение (тонкий клиент, браузер/Vue.js). Выбор сценария определяется исключительно тем, **что подключается к `Calculation.Core`**:

```
┌──────────────────────────────────────────────────────────────┐
│                    БИЗНЕС-ЛОГИКА (Core)                       │
│  Reactive Models / Services / Calculators / State             │
│  OilFieldPlatform.Calculation.Core                            │
│                                                              │
│  Единая для обоих сценариев. Не знает о способе доставки UI. │
└──────────────────────────────────────────────────────────────┘
         ▲                            ▲
         │                            │
         │ (прямые вызовы)            │ (WebSocket)
         │                            │
┌────────┴──────────┐    ┌────────────┴──────────────┐
│  ТОЛСТЫЙ КЛИЕНТ   │    │      ТОНКИЙ КЛИЕНТ        │
│  WPF (не реализ.) │    │  Vue 3 + TypeScript SPA   │
│                   │    │  Server → WebSocket        │
│  ViewModel ← Core │    │  OilFieldPlatform.         │
│  напрямую в       │    │  Calculation.Server +      │
│  памяти процесса  │    │  Calculation.WebClient     │
└───────────────────┘    └────────────────────────────┘
```

---

## Слои, общие для обоих сценариев

### OilFieldPlatform.Domain

Чистый домен без внешних зависимостей. Сущности с `virtual`-свойствами (NHibernate lazy loading), интерфейсы (`IEntity<T>`, `IAuditable`, `IAnionSample`, `ICationSample`), перечисления (`WaterType`, `ClusterStationType`), проекции (DTO).

**Используется:** и толстым, и тонким клиентом (через Core/Infrastructure).

### OilFieldPlatform.Infrastructure

NHibernate-маппинги, репозитории с CQRS-light (ReadRepository/Repository), провайдеры БД (PostgreSQL/SQLite/InMemory), NHibernate-слушатели для аудита.

**Используется:** толстым клиентом напрямую; тонким клиентом — только на сервере, клиент не имеет доступа к БД.

### OilFieldPlatform.Calculation.Core

#### Реактивные модели

`WaterSampleModel` хранит каждую концентрацию иона в `BehaviorSubject<double?>`. `ProjectModel` управляет коллекцией проб через `DynamicData.SourceList<WaterSampleModel>`. Это позволяет:

- **В толстом клиенте** — ViewModel напрямую подписывается на `IObservable<double?>`, получает изменения мгновенно, без запросов к серверу.
- **В тонком клиенте** — сервер подписан на те же `IObservable`, и при каждом изменении отправляет snapshot через WebSocket.

#### Сервисы

- `ManageProjectService` — CRUD проектов (загрузка из БД, сохранение в БД)
- `ListProjectService` — получение списка проектов
- `WaterSampleEquivalentCalculator` — расчёт мг-экв/л для семи ионов

Эти сервисы работают одинаково в обоих сценариях. Разница только в том, кто их вызывает:
- Толстый клиент: ViewModel → сервис (в том же процессе)
- Тонкий клиент: Server Controller → сервис (по запросу через WebSocket)

#### Состояние приложения (ApplicationState)

`ApplicationState` — контейнер текущего `ProjectModel`. Публикует изменения через `IObservable<ProjectModel?>`. Это единый источник правды для текущего проекта.

---

## Сценарий 1: Толстый клиент (не реализован, архитектура предусмотрена)

### Схема подключения

```
WPF Application
├── Views (XAML)
├── ViewModels → подписываются на IObservable из Core
├── Services (вызов ManageProjectService, ListProjectService)
├── Core (прямые ссылки на сборки Core + Infrastructure)
│   ├── Reactive Models (в памяти процесса)
│   ├── Calculators
│   └── NHibernate Repositories (прямой доступ к БД)
└── Database (PostgreSQL/SQLite)
```

### Особенности

- **Всё в одном процессе.** Модели (WaterSampleModel, ProjectModel) живут в памяти приложения. Изменение иона → `BehaviorSubject.OnNext` → ViewModel получает уведомление → UI обновляется.
- **NHibernate — напрямую.** Репозитории вызываются из сервисов без промежуточного слоя.
- **Реактивность — через Rx.NET.** `IObservable` из Core связывается с INotifyPropertyChanged ViewModel через `ObserveOnDispatcher`.
- **Логика расчётов — в памяти.** `WaterSampleEquivalentCalculator` запускается в том же процессе при каждом изменении иона.

---

## Сценарий 2: Тонкий клиент (реализован)

### Схема подключения

```
Browser (Vue 3 + TypeScript)           Server (ASP.NET Core)
┌──────────────────────┐              ┌──────────────────────────┐
│  WaterSampleCalcPage │──WebSocket──▶│  WebSocketService        │
│  (Vue компонент)     │              │  ├─ ApplicationController│
│         │            │              │  ├─ WaterSamplePageCtrl  │
│  api/ws.ts (typed)   │              │  ├─ AppStateLoader       │
│         │            │              │  │    (Redis)            │
│  useWebSocket.ts     │              │  └─ LoggerForwarder      │
└──────────────────────┘              └──────┬───────────────────┘
                                             │
                                    ┌────────▼───────────────────┐
                                    │  Calculation.Core           │
                                    │  Reactive Models / Services │
                                    │  Calculators / State        │
                                    └────────┬───────────────────┘
                                             │
                                    ┌────────▼───────────────────┐
                                    │  Infrastructure             │
                                    │  NHibernate / Repositories  │
                                    │  PostgreSQL / SQLite        │
                                    └────────────────────────────┘
```

### Особенности

- **Модели — на сервере.** `ApplicationState.WaterSampleModel` живут только в памяти серверного процесса.
- **Клиент — тонкий.** Vue-компоненты не имеют прямого доступа к Core, Infrastructure или Domain. Все данные приходят через WebSocket в виде JSON.
- **Типизированные JSON-схемы.** Каждое сообщение (запрос/ответ) — отдельный C#-класс на сервере и TypeScript-интерфейс на клиенте. Полиморфная сериализация через `[JsonDerivedType(TypeDiscriminatorPropertyName = "type")]`.
- **Контроллеры-адаптеры.** `ApplicationController` и `WaterSamplePageController` принимают типизированные запросы, вызывают сервисы Core, возвращают типизированные ответы. Это прослойка, которая превращает Core-методы в WebSocket-команды.
- **Redis-сессии.** `AppStateLoader` сохраняет полный слепок несохранённого проекта (OilField, DevTarget, все пробы с эквивалентами). При перезагрузке страницы сессия восстанавливается.
- **Reactive push.** Сервер подписан на `IObservable` из Core (IsChanged, OnCalculate) и при изменениях отправляет push-уведомления через `OnChanged`-событие контроллеров.
- **Логгирование в WebSocket.** `LoggerForwarder` отправляет Warning+ сообщения клиенту в реальном времени.

---

## Сравнение сценариев

| Характеристика | Толстый клиент (WPF) | Тонкий клиент (Web) |
|---|---|---|
| **Где живут модели** | В памяти клиента | В памяти сервера |
| **Доступ к БД** | Прямой (через Infrastructure) | Только через сервер |
| **Расчёты** | На клиенте (в том же процессе) | На сервере |
| **Реактивность** | `IObservable` → ViewModel → UI | `IObservable` → WebSocket → JSON → Vue |
| **Состояние сессии** | Не требуется (всё в памяти) | Redis (snapshot проекта) |
| **Сеть** | Локальный вызов методов | WebSocket (JSON) |
| **Логирование** | NLog (файл/консоль) | NLog + WebSocket-логгер |
| **Обновления UI** | Мгновенные (подписка в памяти) | Через WebSocket (задержка ~1-10 мс) |
| **Масштабирование** | Один пользователь на процесс | Много пользователей, общий сервер |
| **Изоляция данных** | Нет (общая БД) | Сессии изолированы (Redis + сессия) |

---

## Как архитектура поддерживает оба сценария

Ключевое решение: **`Calculation.Core` не зависит от способа доставки UI.**

```csharp
// Core — ничего не знает ни о WPF, ни о WebSocket
public class WaterSampleModel
{
    public BehaviorSubject<double?> Chloride { get; }
    public BehaviorSubject<double?> Carbonate { get; }
    // …
}

public class ProjectModel
{
    public SourceList<WaterSampleModel> WaterSamples { get; }
    public IObservable<bool> IsChangedAsObservable { get; }
}

public class WaterSampleEquivalentCalculator
{
    public void Calculate(WaterSampleModel sample) { /* чистая математика */ }
}
```

Толстый клиент подключается напрямую:

```csharp
// WPF ViewModel — толстый клиент
public class WaterSampleViewModel
{
    private readonly WaterSampleModel _model;

    public double? Chloride
    {
        get => _model.Chloride.Value;
        set => _model.Chloride.OnNext(value);  // → триггерит расчёт
    }
}
```

Тонкий клиент подключается через адаптер-контроллер:

```csharp
// Server Controller — тонкий клиент
public class WaterSamplePageController : IWebSocketController
{
    public WaterSampleEditedResponse HandleEdit(WaterSampleEditRequest request)
    {
        _model.Chloride.OnNext(request.Value);  // тот же вызов, что и в WPF
        // → триггерит расчёт на сервере → OnChanged → WebSocket → клиент
    }
}
```

Оба сценария используют **одну и ту же модель**, **один и тот же калькулятор**, **одни и те же сервисы**. Разница — только в том, какой код стоит **над** Core.

---

## Что нужно сделать для реализации толстого клиента

1. Создать WPF-проект (`OilFieldPlatform.Calculation.Wpf`).
2. Добавить ссылки на `Calculation.Core`, `Infrastructure`, `Domain`.
3. Написать ViewModel, которые подписываются на `IObservable` из Core.
4. Использовать `ManageProjectService` и `ListProjectService` напрямую (без WebSocket).
5. Использовать `ApplicationState` как единый контейнер состояния.
6. Для реактивной привязки к WPF — `ObserveOnDispatcher` + `INotifyPropertyChanged` или `BindableBase`.

Никаких изменений в `Calculation.Core` не требуется — вся логика уже готова и протестирована в тонком клиенте.

---

## Вывод

Архитектура OilFieldPlatform демонстрирует, как спроектировать предметную платформу так, чтобы **один и тот же код бизнес-логики** работал в двух принципиально разных сценариях развёртывания:

- **Толстый клиент** — максимальная производительность, офлайн-режим, полный контроль над моделями.
- **Тонкий клиент** — централизованное управление, многопользовательская работа, нулевая установка.

Выбор между ними — не архитектурное, а **инфраструктурное** решение. Код Core остаётся неизменным.
