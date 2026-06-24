using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OilFieldPlatform.Infrastructure.Providers;

/// <summary>
/// Провайдер имени текущего пользователя.
/// Определяет имя по цепочке: AsyncLocal-контекст → внешний резолвер → Environment.UserName.
/// </summary>
public class UserNameProvider
{
    private readonly Func<string?> _userNameResolver;
    private readonly ILogger _logger;
    private static readonly AsyncLocal<string?> _contextUser = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userNameResolver">
    /// Кастомный резолвер имени пользователя (опционально).
    /// Если не указан — используется <see cref="DefaultResolver"/>.
    /// </param>
    /// <param name="logger">Логгер (опционально).</param>
    public UserNameProvider(Func<string?>? userNameResolver = null, ILogger<UserNameProvider>? logger = null)
    {
        _userNameResolver = userNameResolver ?? DefaultResolver;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// Имя текущего пользователя.
    /// Приоритет: AsyncLocal (WebSocket/фоновая задача) → внешний резолвер → "".
    /// </summary>
    public string UserName
    {
        get
        {
            var name = _contextUser.Value
                       ?? _userNameResolver()
                       ?? string.Empty;

            _logger.LogDebug("Resolved user name: {UserName}", name);
            return name;
        }
    }

    /// <summary>
    /// Установить имя пользователя для текущего async-контекста (WebSocket, SignalR, Task.Run и т.п.).
    /// Значение автоматически очищается при выходе из async-стека.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    public static void SetContextUser(string? userName) =>
        _contextUser.Value = userName;

    /// <summary>
    /// Резолвер по умолчанию: ClaimsPrincipal.Current → Environment.UserName.
    /// </summary>
    private static string? DefaultResolver()
    {
        if (ClaimsPrincipal.Current?.Identity?.Name is { Length: > 0 } name)
            return name;

        return Environment.UserName;
    }
}
