namespace OilFieldPlatform.Infrastructure.Settings;

/// <summary>Настройки подключения к базе данных.</summary>
public class DbSettings
{
    /// <summary>Хост (адрес) сервера БД.</summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>Порт сервера БД.</summary>
    public int Port { get; set; }

    /// <summary>Имя пользователя.</summary>
    public string User { get; set; } = string.Empty;

    /// <summary>Пароль пользователя.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Имя базы данных.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Схема БД.</summary>
    public string Schema { get; set; } = string.Empty;

    /// <summary>Провайдер (Sqlite / Postgres / InMemory).</summary>
    public string Provider { get; set; } = "InMemory";
}
