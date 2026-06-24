using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Infrastructure.Settings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using FluentNHibernate.Conventions;

namespace OilFieldPlatform.Infrastructure.Providers;

/// <summary>
/// Провайдер конфигурации NHibernate: настройка подключения, маппингов и подписка EventListener'ов.
/// </summary>
public class DbConfigProvider
{
    private readonly DbSettings _settings;
    private readonly DbListenerRegistry _listenerRegistry;
    private readonly ILogger _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="settings">Настройки подключения к БД.</param>
    /// <param name="listenerRegistry">Реестр EventListener'ов для регистрации в конфигурации.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public DbConfigProvider(DbSettings settings, DbListenerRegistry listenerRegistry, ILogger<DbConfigProvider>? logger = null)
    {
        _settings = settings;
        _listenerRegistry = listenerRegistry;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// Строка подключения, сформированная по настройкам.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            var cs = _settings.Provider switch
            {
                "Sqlite" => GetSqliteConnectionString(),
                "Postgres" => GetPostgresConnectionString(),
                _ => ""
            };
            _logger.LogDebug("Connection string resolved for provider: {Provider}", _settings.Provider);
            return cs;
        }
    }

    private string GetSqliteConnectionString()
    {
        var cs = $"Data Source={_settings.Host};";
        _logger.LogDebug("SQLite connection string: {ConnectionString}", cs);
        return cs;
    }

    private string GetPostgresConnectionString()
    {
        var cs = $"Host={_settings.Host};Port={_settings.Port};Username={_settings.User};Password=***;Database={_settings.Name}";
        _logger.LogDebug("PostgreSQL connection string: {ConnectionString}", cs);
        return $"Host={_settings.Host};Port={_settings.Port};Username={_settings.User};Password={_settings.Password};Database={_settings.Name}";
    }

    private IPersistenceConfigurer CreatePersistenceConfigurer()
    {
        _logger.LogDebug("Creating persistence configurer for provider: {Provider}", _settings.Provider);

        IPersistenceConfigurer configurer = _settings.Provider switch
        {
            "Sqlite" => SQLiteConfiguration.Standard.ConnectionString(GetSqliteConnectionString()),
            "Postgres" => PostgreSQLConfiguration.PostgreSQL82.ConnectionString(GetPostgresConnectionString()),
            _ => SQLiteConfiguration.Standard.InMemory()
        };

        _logger.LogInformation("Persistence configurer created: {Provider}", _settings.Provider);
        return configurer;
    }

    /// <summary>
    /// Создаёт базовую FluentConfiguration (БД + маппинги).
    /// </summary>
    protected FluentConfiguration CreateBaseFluentConfiguration(Action<MappingConfiguration> mappings)
    {
        return Fluently
            .Configure()
            .Database(CreatePersistenceConfigurer)
            .Mappings(mappings);
    }

    /// <summary>
    /// Создаёт полную FluentConfiguration с дополнительными настройками и EventListener'ами.
    /// </summary>
    /// <param name="mappings">Действие для конфигурации маппингов (FluentNHibernate).</param>
    /// <returns>Готовая FluentConfiguration.</returns>
    public FluentConfiguration CreateFluentConfiguration(Action<MappingConfiguration> mappings)
    {
        _logger.LogInformation("Creating NHibernate fluent configuration for provider: {Provider}", _settings.Provider);

        var config = CreateBaseFluentConfiguration(mappings);

        var nhConfig = config.BuildConfiguration();
        nhConfig.SetProperty("show_sql", "true");
        if (_settings.Schema.IsNotEmpty())
        {
            nhConfig.SetProperty("default_schema", _settings.Schema);
            _logger.LogDebug("Default schema set: {Schema}", _settings.Schema);
        }

        ApplyListeners(nhConfig);
        _logger.LogInformation("NHibernate configuration built successfully for provider: {Provider}", _settings.Provider);

        return Fluently.Configure(nhConfig);
    }

    /// <summary>
    /// Регистрирует все EventListener'ы из <see cref="DbListenerRegistry"/> в NHibernate-конфигурации.
    /// </summary>
    private void ApplyListeners(Configuration nhConfig)
    {
        var listenerCount = 0;

        if (_listenerRegistry.PreInsertListeners.Length > 0)
        {
            nhConfig.EventListeners.PreInsertEventListeners =
                [.. nhConfig.EventListeners.PreInsertEventListeners, .. _listenerRegistry.PreInsertListeners];
            listenerCount += _listenerRegistry.PreInsertListeners.Length;
        }

        if (_listenerRegistry.PostInsertListeners.Length > 0)
        {
            nhConfig.EventListeners.PostInsertEventListeners =
                [.. nhConfig.EventListeners.PostInsertEventListeners, .. _listenerRegistry.PostInsertListeners];
            listenerCount += _listenerRegistry.PostInsertListeners.Length;
        }

        if (_listenerRegistry.PreUpdateListeners.Length > 0)
        {
            nhConfig.EventListeners.PreUpdateEventListeners =
                [.. nhConfig.EventListeners.PreUpdateEventListeners, .. _listenerRegistry.PreUpdateListeners];
            listenerCount += _listenerRegistry.PreUpdateListeners.Length;
        }

        if (_listenerRegistry.PostUpdateListeners.Length > 0)
        {
            nhConfig.EventListeners.PostUpdateEventListeners =
                [.. nhConfig.EventListeners.PostUpdateEventListeners, .. _listenerRegistry.PostUpdateListeners];
            listenerCount += _listenerRegistry.PostUpdateListeners.Length;
        }

        if (_listenerRegistry.PreDeleteListeners.Length > 0)
        {
            nhConfig.EventListeners.PreDeleteEventListeners =
                [.. nhConfig.EventListeners.PreDeleteEventListeners, .. _listenerRegistry.PreDeleteListeners];
            listenerCount += _listenerRegistry.PreDeleteListeners.Length;
        }

        if (_listenerRegistry.PostDeleteListeners.Length > 0)
        {
            nhConfig.EventListeners.PostDeleteEventListeners =
                [.. nhConfig.EventListeners.PostDeleteEventListeners, .. _listenerRegistry.PostDeleteListeners];
            listenerCount += _listenerRegistry.PostDeleteListeners.Length;
        }

        if (_listenerRegistry.FlushListeners.Length > 0)
        {
            nhConfig.EventListeners.FlushEventListeners =
                [.. nhConfig.EventListeners.FlushEventListeners, .. _listenerRegistry.FlushListeners];
            listenerCount += _listenerRegistry.FlushListeners.Length;
        }

        if (_listenerRegistry.MergeListeners.Length > 0)
        {
            nhConfig.EventListeners.MergeEventListeners =
                [.. nhConfig.EventListeners.MergeEventListeners, .. _listenerRegistry.MergeListeners];
            listenerCount += _listenerRegistry.MergeListeners.Length;
        }

        _logger.LogDebug("Applied {Count} NHibernate event listeners", listenerCount);
    }
}
