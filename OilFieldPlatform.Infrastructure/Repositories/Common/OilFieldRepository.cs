using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий для OilFieldEntity (месторождение) с операциями записи.
/// </summary>
public class OilFieldRepository : OilFieldReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public OilFieldRepository(ISession session, ILogger<OilFieldRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить месторождение.</summary>
    /// <param name="entity">Новое месторождение.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(OilFieldEntity entity) => base.Add(entity);
    /// <summary>Обновить месторождение.</summary>
    public new void Update(OilFieldEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить месторождение.</summary>
    /// <param name="entity">Месторождение для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new OilFieldEntity AddOrUpdate(OilFieldEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить месторождение по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить месторождение (асинхронно).</summary>
    /// <param name="entity">Новое месторождение.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(OilFieldEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить месторождение (асинхронно).</summary>
    /// <param name="entity">Месторождение с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(OilFieldEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить месторождение (асинхронно).</summary>
    /// <param name="entity">Месторождение для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<OilFieldEntity> AddOrUpdateAsync(OilFieldEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить месторождение по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
