namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Интерфейс для сущностей, содержащих концентрации катионов.
/// </summary>
public interface ICationSample
{
    /// <summary>Ca²⁺ (мг/л или мг-экв/л).</summary>
    double? Calcium { get; set; }

    /// <summary>Mg²⁺ (мг/л или мг-экв/л).</summary>
    double? Magnesium { get; set; }

    /// <summary>Na⁺ (мг/л или мг-экв/л).</summary>
    double? Sodium { get; set; }
}
