namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Интерфейс для сущностей, содержащих концентрации анионов.
/// </summary>
public interface IAnionSample
{
    /// <summary>Cl⁻ (мг/л или мг-экв/л).</summary>
    double? Chloride { get; set; }

    /// <summary>CO₃²⁻ (мг/л или мг-экв/л).</summary>
    double? Carbonate { get; set; }

    /// <summary>HCO₃⁻ (мг/л или мг-экв/л).</summary>
    double? Bicarbonate { get; set; }

    /// <summary>SO₄²⁻ (мг/л или мг-экв/л).</summary>
    double? Sulfate { get; set; }
}
