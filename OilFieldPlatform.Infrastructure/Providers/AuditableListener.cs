using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NHibernate.Event;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Infrastructure.Providers;

/// <summary>
/// NHibernate-листенер для автоматического заполнения полей аудита (IAuditable).
/// Срабатывает перед вставкой (OnPreInsert) и перед обновлением (OnPreUpdate).
/// </summary>
public class AuditableListener : IPreInsertEventListener, IPreUpdateEventListener
{
    private readonly UserNameProvider _userNameProvider;
    private readonly ILogger _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userNameProvider">Провайдер имени текущего пользователя.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public AuditableListener(UserNameProvider userNameProvider, ILogger<AuditableListener>? logger = null)
    {
        _userNameProvider = userNameProvider;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// Заполняет CreatedAt/UpdatedAt/CreatedBy/UpdatedBy перед вставкой.
    /// </summary>
    /// <param name="event">Событие вставки.</param>
    /// <returns>false (операция не отменяется).</returns>
    public bool OnPreInsert(PreInsertEvent @event)
    {
        if (@event.Entity is IAuditable auditable)
        {
            var now = DateTime.UtcNow;
            var user = _userNameProvider.UserName;
            auditable.CreatedAt = now;
            auditable.UpdatedAt = now;
            auditable.CreatedBy = user;
            auditable.UpdatedBy = user;

            Set(@event.State, @event.Persister.PropertyNames, "CreatedAt", now);
            Set(@event.State, @event.Persister.PropertyNames, "UpdatedAt", now);
            Set(@event.State, @event.Persister.PropertyNames, "CreatedBy", user);
            Set(@event.State, @event.Persister.PropertyNames, "UpdatedBy", user);

            _logger.LogDebug("Audit fields set on insert for {EntityType} by {User}",
                @event.Entity.GetType().Name, user);
        }
        else
        {
            _logger.LogWarning("Entity {EntityType} does not implement IAuditable on insert",
                @event.Entity.GetType().Name);
        }

        return false;
    }

    /// <summary>
    /// Обновляет UpdatedAt/UpdatedBy перед обновлением.
    /// </summary>
    /// <param name="event">Событие обновления.</param>
    /// <returns>false (операция не отменяется).</returns>
    public bool OnPreUpdate(PreUpdateEvent @event)
    {
        if (@event.Entity is IAuditable auditable)
        {
            var now = DateTime.UtcNow;
            var user = _userNameProvider.UserName;

            auditable.UpdatedAt = now;
            auditable.UpdatedBy = user;

            Set(@event.State, @event.Persister.PropertyNames, "UpdatedAt", now);
            Set(@event.State, @event.Persister.PropertyNames, "UpdatedBy", user);

            _logger.LogDebug("Audit fields updated for {EntityType} by {User}",
                @event.Entity.GetType().Name, user);
        }
        else
        {
            _logger.LogWarning("Entity {EntityType} does not implement IAuditable on update",
                @event.Entity.GetType().Name);
        }

        return false;
    }

    private static void Set(object[] state, string[] propertyNames, string propertyName, object value)
    {
        var index = Array.IndexOf(propertyNames, propertyName);
        if (index >= 0)
            state[index] = value;
    }

    /// <inheritdoc />
    public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
    {
        OnPreInsert(@event);
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
    {
        OnPreUpdate(@event);
        return Task.FromResult(false);
    }
}
