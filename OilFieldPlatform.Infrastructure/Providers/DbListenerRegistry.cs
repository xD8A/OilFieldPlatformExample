#pragma warning disable S1450 // _logger field used for future extensibility
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NHibernate.Event;

namespace OilFieldPlatform.Infrastructure.Providers;

/// <summary>
/// Реестр NHibernate EventListener'ов для регистрации в конфигурации сессии.
/// Каждое свойство — массив листенеров соответствующего типа события.
/// </summary>
public class DbListenerRegistry
{
    private readonly ILogger _logger;

    /// <summary>Листенеры перед вставкой (PreInsert).</summary>
    public IPreInsertEventListener[] PreInsertListeners { get; }

    /// <summary>Листенеры после вставки (PostInsert).</summary>
    public IPostInsertEventListener[] PostInsertListeners { get; }

    /// <summary>Листенеры перед обновлением (PreUpdate).</summary>
    public IPreUpdateEventListener[] PreUpdateListeners { get; }

    /// <summary>Листенеры после обновления (PostUpdate).</summary>
    public IPostUpdateEventListener[] PostUpdateListeners { get; }

    /// <summary>Листенеры перед удалением (PreDelete).</summary>
    public IPreDeleteEventListener[] PreDeleteListeners { get; }

    /// <summary>Листенеры после удаления (PostDelete).</summary>
    public IPostDeleteEventListener[] PostDeleteListeners { get; }

    /// <summary>Листенеры сброса (Flush).</summary>
    public IFlushEventListener[] FlushListeners { get; }

    /// <summary>Листенеры слияния (Merge).</summary>
    public IMergeEventListener[] MergeListeners { get; }

    private int TotalListenerCount =>
        PreInsertListeners.Length + PostInsertListeners.Length +
        PreUpdateListeners.Length + PostUpdateListeners.Length +
        PreDeleteListeners.Length + PostDeleteListeners.Length +
        FlushListeners.Length + MergeListeners.Length;

    /// <summary>
    /// Конструктор. Принимает коллекции листенеров для каждого типа события.
    /// </summary>
    /// <param name="preInsertListeners">Листенеры PreInsert.</param>
    /// <param name="postInsertListeners">Листенеры PostInsert.</param>
    /// <param name="preUpdateListeners">Листенеры PreUpdate.</param>
    /// <param name="postUpdateListeners">Листенеры PostUpdate.</param>
    /// <param name="preDeleteListeners">Листенеры PreDelete.</param>
    /// <param name="postDeleteListeners">Листенеры PostDelete.</param>
    /// <param name="flushListeners">Листенеры Flush.</param>
    /// <param name="mergeListeners">Листенеры Merge.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public DbListenerRegistry(
        IEnumerable<IPreInsertEventListener> preInsertListeners,
        IEnumerable<IPostInsertEventListener> postInsertListeners,
        IEnumerable<IPreUpdateEventListener> preUpdateListeners,
        IEnumerable<IPostUpdateEventListener> postUpdateListeners,
        IEnumerable<IPreDeleteEventListener> preDeleteListeners,
        IEnumerable<IPostDeleteEventListener> postDeleteListeners,
        IEnumerable<IFlushEventListener> flushListeners,
        IEnumerable<IMergeEventListener> mergeListeners,
        ILogger<DbListenerRegistry>? logger = null)
    {
        PreInsertListeners = preInsertListeners.ToArray();
        PostInsertListeners = postInsertListeners.ToArray();
        PreUpdateListeners = preUpdateListeners.ToArray();
        PostUpdateListeners = postUpdateListeners.ToArray();
        PreDeleteListeners = preDeleteListeners.ToArray();
        PostDeleteListeners = postDeleteListeners.ToArray();
        FlushListeners = flushListeners.ToArray();
        MergeListeners = mergeListeners.ToArray();
        _logger = (ILogger?)logger ?? NullLogger.Instance;

        _logger.LogInformation("DbListenerRegistry initialized with {Count} total listeners", TotalListenerCount);
    }
}
