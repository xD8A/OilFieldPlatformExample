using OilFieldPlatform.Calculation.Server.Controllers;
using OilFieldPlatform.Calculation.Server.Schemas.Responses;

namespace OilFieldPlatform.Calculation.Server.Services;

/// <summary>Обёртка над ILogger, отправляющая сообщения уровня Warning+ через ILogEventTarget.</summary>
public sealed class LoggerForwarder : ILogger
{
    private readonly ILogger _inner;
    private readonly IWebSocketController _target;

    /// <summary>Конструктор.</summary>
    /// <param name="inner">Оригинальный логгер.</param>
    /// <param name="target">Цель для отправки лог-сообщений через WebSocket.</param>
    public LoggerForwarder(ILogger inner, IWebSocketController target)
    {
        _inner = inner;
        _target = target;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _inner.Log(logLevel, eventId, state, exception, formatter);

        if (logLevel < LogLevel.Warning)
            return;

        _target.PublishLog(new LogResponse
        {
            Level = logLevel.ToString(),
            Message = formatter(state, exception),
        });
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel);

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _inner.BeginScope(state);
}
