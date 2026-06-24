using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий для WaterSampleEntity (проба воды) с операциями записи.
/// </summary>
public class WaterSampleRepository : WaterSampleReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WaterSampleRepository(ISession session, ILogger<WaterSampleRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить пробу воды.</summary>
    /// <param name="entity">Новая проба воды.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(WaterSampleEntity entity) => base.Add(entity);
    /// <summary>Обновить пробу воды.</summary>
    public new void Update(WaterSampleEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить пробу воды.</summary>
    /// <param name="entity">Проба воды для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new WaterSampleEntity AddOrUpdate(WaterSampleEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить пробу воды по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить пробу воды (асинхронно).</summary>
    /// <param name="entity">Новая проба воды.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(WaterSampleEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить пробу воды (асинхронно).</summary>
    /// <param name="entity">Проба воды с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(WaterSampleEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить пробу воды (асинхронно).</summary>
    /// <param name="entity">Проба воды для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<WaterSampleEntity> AddOrUpdateAsync(WaterSampleEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить пробу воды по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор пробы воды.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
