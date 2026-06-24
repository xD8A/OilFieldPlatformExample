using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Calculation;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Calculation;

/// <summary>
/// Репозиторий для CalcWaterSampleEntity (проба воды в расчётном проекте) с операциями записи.
/// </summary>
public class CalcWaterSampleRepository : CalcWaterSampleReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public CalcWaterSampleRepository(ISession session, ILogger<CalcWaterSampleRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить пробу в расчётный проект.</summary>
    /// <param name="entity">Новая проба в расчётном проекте.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(CalcWaterSampleEntity entity) => base.Add(entity);
    /// <summary>Обновить пробу в расчётном проекте.</summary>
    public new void Update(CalcWaterSampleEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить пробу в расчётном проекте.</summary>
    /// <param name="entity">Проба для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new CalcWaterSampleEntity AddOrUpdate(CalcWaterSampleEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить пробу из расчётного проекта по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить пробу в расчётный проект (асинхронно).</summary>
    /// <param name="entity">Новая проба в расчётном проекте.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(CalcWaterSampleEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить пробу в расчётном проекте (асинхронно).</summary>
    /// <param name="entity">Проба с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(CalcWaterSampleEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить пробу в расчётном проекте (асинхронно).</summary>
    /// <param name="entity">Проба для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<CalcWaterSampleEntity> AddOrUpdateAsync(CalcWaterSampleEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить пробу из расчётного проекта по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор пробы.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
