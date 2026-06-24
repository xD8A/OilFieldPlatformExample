using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Calculation;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Calculation;

/// <summary>
/// Репозиторий для CalcProjectEntity (расчётный проект) с операциями записи.
/// </summary>
public class CalcProjectRepository : CalcProjectReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public CalcProjectRepository(ISession session, ILogger<CalcProjectRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить расчётный проект.</summary>
    /// <param name="entity">Новый расчётный проект.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(CalcProjectEntity entity) => base.Add(entity);
    /// <summary>Обновить расчётный проект.</summary>
    public new void Update(CalcProjectEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить расчётный проект.</summary>
    /// <param name="entity">Расчётный проект для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new CalcProjectEntity AddOrUpdate(CalcProjectEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить расчётный проект по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить расчётный проект (асинхронно).</summary>
    /// <param name="entity">Новый расчётный проект.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(CalcProjectEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить расчётный проект (асинхронно).</summary>
    /// <param name="entity">Расчётный проект с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(CalcProjectEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить расчётный проект (асинхронно).</summary>
    /// <param name="entity">Расчётный проект для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<CalcProjectEntity> AddOrUpdateAsync(CalcProjectEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить расчётный проект по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор расчётного проекта.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
