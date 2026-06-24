using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Domain.Projections.Calculation;
using OilFieldPlatform.Infrastructure.Repositories.Calculation;

namespace OilFieldPlatform.Calculation.Core.Services;

/// <summary>Сервис получения списка расчётных проектов.</summary>
public class ListProjectService
{
    private readonly CalcProjectReadRepository _repository;
    private readonly ILogger _logger;

    /// <summary>Конструктор.</summary>
    /// <param name="repository">Репозиторий чтения расчётных проектов.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public ListProjectService(CalcProjectReadRepository repository, ILogger<ListProjectService>? logger = null)
    {
        _repository = repository;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
    }

    /// <summary>Получить проекции всех расчётных проектов.</summary>
    /// <returns>Список проекций проектов.</returns>
    public IList<CalcProjectProjection> GetAll()
    {
        _logger.LogDebug("Getting all calc project projections");
        var result = _repository.GetProjections().ToList();
        _logger.LogDebug("Retrieved {Count} calc project projections", result.Count);
        return result;
    }

    /// <summary>Получить проекции всех расчётных проектов (асинхронно).</summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций проектов.</returns>
    public async Task<IList<CalcProjectProjection>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Getting all calc project projections (async)");
        var result = await _repository.GetProjectionsAsync(ct).ConfigureAwait(false);
        var list = result.ToList();
        _logger.LogDebug("Retrieved {Count} calc project projections (async)", list.Count);
        return list;
    }
}
