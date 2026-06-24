using System.Reflection;
using AutoMapper;

namespace OilFieldPlatform.Infrastructure.Extensions;

/// <summary>Extension methods for AutoMapper mapping expressions.</summary>
public static class AutoMapperExtensions
{
    /// <summary>Игнорирует все свойства целевого типа при маппинге.</summary>
    /// <remarks>
    /// Полезно при использовании <c>ConstructUsing</c> или <c>AfterMap</c>,
    /// когда все или большинство полей destination-типа задаются вручную
    /// и не должны маппиться по соглашению.
    /// </remarks>
    /// <typeparam name="TSource">Тип источника.</typeparam>
    /// <typeparam name="TDestination">Тип приёмника.</typeparam>
    /// <param name="expression">Mapping expression для настройки.</param>
    /// <returns>Тот же mapping expression для chaining.</returns>
    public static IMappingExpression<TSource, TDestination> IgnoreAllMembers<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        foreach (var property in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            expression.ForMember(property.Name, opt => opt.Ignore());
        }

        return expression;
    }
}
