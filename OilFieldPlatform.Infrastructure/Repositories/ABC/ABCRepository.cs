#pragma warning disable CA1051, S101 // protected _session field and ABC naming for base repository
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NHibernate;
using NHibernate.Linq;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Infrastructure.Repositories.ABC;

/// <summary>
/// Базовый абстрактный репозиторий с NHibernate-операциями CRUD.
/// TEntity должен реализовывать IEntity&lt;TEntity&gt; для доступа к Id.
/// </summary>
public abstract class ABCRepository<TEntity> where TEntity : IEntity<TEntity>
{
    /// <summary>Сессия NHibernate.</summary>
    protected readonly ISession _session;

    /// <summary>Логгер.</summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate для работы с БД.</param>
    /// <param name="logger">Логгер (опционально).</param>
    protected ABCRepository(ISession session, ILogger? logger = null)
    {
        _session = session;
        _logger = logger ?? NullLogger.Instance;
    }

    private static string EntityName => typeof(TEntity).Name;

    /// <summary>
    /// Получить все сущности.
    /// </summary>
    /// <returns>Все записи TEntity.</returns>
    public virtual IEnumerable<TEntity> GetAll()
    {
        _logger.LogDebug("Getting all {EntityType}", EntityName);
        var result = _session.Query<TEntity>().AsEnumerable();
        _logger.LogDebug("Retrieved all {EntityType}", EntityName);
        return result;
    }

    /// <summary>
    /// Получить сущность по Id. Возвращает null, если не найдена.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <returns>Сущность или null.</returns>
    public virtual TEntity? GetById(long id)
    {
        _logger.LogDebug("Getting {EntityType} by Id: {Id}", EntityName, id);
        var entity = _session.Get<TEntity>(id);
        if (entity is not null)
            _logger.LogDebug("Found {EntityType} with Id: {Id}", EntityName, id);
        else
            _logger.LogDebug("{EntityType} with Id: {Id} not found", EntityName, id);
        return entity;
    }

    /// <summary>
    /// Получить прокси-ссылку по Id. Обращение к БД происходит при первом доступе к свойству.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <returns>Прокси сущности (NHibernate proxy).</returns>
    public virtual TEntity Load(long id)
    {
        _logger.LogDebug("Loading {EntityType} proxy by Id: {Id}", EntityName, id);
        return _session.Load<TEntity>(id);
    }

    /// <summary>
    /// Добавить новую сущность.
    /// </summary>
    /// <param name="entity">Новая сущность.</param>
    /// <returns>Сгенерированный Id.</returns>
    protected virtual long Add(TEntity entity)
    {
        _logger.LogDebug("Adding new {EntityType}", EntityName);
        var id = (long)_session.Save(entity);
        _logger.LogDebug("Added {EntityType} with Id: {Id}", EntityName, id);
        return id;
    }

    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    /// <param name="entity">Сущность с изменёнными данными.</param>
    protected virtual void Update(TEntity entity)
    {
        _logger.LogDebug("Updating {EntityType} with Id: {Id}", EntityName, entity.Id);
        _session.Update(entity);
    }

    /// <summary>
    /// Добавить или обновить сущность (Merge).
    /// </summary>
    /// <param name="entity">Сущность для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    protected virtual TEntity AddOrUpdate(TEntity entity)
    {
        _logger.LogDebug("Merging {EntityType} with Id: {Id}", EntityName, entity.Id);
        var merged = (TEntity)_session.Merge(entity);
        entity.Id = merged.Id;
        _logger.LogDebug("Merged {EntityType}, Id: {Id}", EntityName, merged.Id);
        return merged;
    }

    /// <summary>
    /// Удалить сущность по Id.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    protected virtual void Delete(long id)
    {
        _logger.LogDebug("Deleting {EntityType} by Id: {Id}", EntityName, id);
        var entity = _session.Load<TEntity>(id);
        _session.Delete(entity);
    }

    /// <summary>
    /// Получить все сущности (асинхронно).
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Все записи TEntity.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Getting all {EntityType} async", EntityName);
        var result = await _session.Query<TEntity>().ToListAsync(ct);
        _logger.LogDebug("Retrieved all {EntityType} async, count: {Count}", EntityName, result.Count);
        return result;
    }

    /// <summary>
    /// Получить сущность по Id (асинхронно).
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сущность или null.</returns>
    public virtual async Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting {EntityType} by Id async: {Id}", EntityName, id);
        var entity = await _session.GetAsync<TEntity>(id, ct);
        if (entity is not null)
            _logger.LogDebug("Found {EntityType} with Id: {Id}", EntityName, id);
        else
            _logger.LogDebug("{EntityType} with Id: {Id} not found", EntityName, id);
        return entity;
    }

    /// <summary>
    /// Получить прокси-ссылку (асинхронно).
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Прокси сущности.</returns>
    protected virtual async Task<TEntity> LoadAsync(long id, CancellationToken ct = default)
    {
        _logger.LogDebug("Loading {EntityType} proxy async by Id: {Id}", EntityName, id);
        return await _session.LoadAsync<TEntity>(id, ct);
    }

    /// <summary>
    /// Добавить сущность (асинхронно).
    /// </summary>
    /// <param name="entity">Новая сущность.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    protected virtual async Task<long> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        _logger.LogDebug("Adding new {EntityType} async", EntityName);
        var id = (long)await _session.SaveAsync(entity, ct);
        _logger.LogDebug("Added {EntityType} with Id: {Id} async", EntityName, id);
        return id;
    }

    /// <summary>
    /// Обновить сущность (асинхронно).
    /// </summary>
    /// <param name="entity">Сущность с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    protected virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _logger.LogDebug("Updating {EntityType} with Id: {Id} async", EntityName, entity.Id);
        await _session.UpdateAsync(entity, ct);
    }

    /// <summary>
    /// Добавить или обновить сущность (асинхронно).
    /// </summary>
    /// <param name="entity">Сущность для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    protected virtual async Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _logger.LogDebug("Merging {EntityType} with Id: {Id} async", EntityName, entity.Id);
        var merged = (TEntity)await _session.MergeAsync(entity, ct);
        entity.Id = merged.Id;
        _logger.LogDebug("Merged {EntityType}, Id: {Id} async", EntityName, merged.Id);
        return merged;
    }

    /// <summary>
    /// Удалить сущность по Id (асинхронно).
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    /// <param name="ct">Токен отмены.</param>
    protected virtual async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        _logger.LogDebug("Deleting {EntityType} by Id: {Id} async", EntityName, id);
        var entity = await _session.LoadAsync<TEntity>(id, ct);
        await _session.DeleteAsync(entity, ct);
    }
}
