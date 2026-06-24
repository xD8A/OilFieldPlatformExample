namespace OilFieldPlatform.Domain.Enums;

/// <summary>Тип насосной станции.</summary>
public enum ClusterStationType
{
    /// <summary>Дожимная насосная станция (ДНС). Сепарация и транспорт продукции скважин.</summary>
    DNS = 1,

    /// <summary>Кустовая насосная станция (КНС). Закачка воды в пласт для ППД.</summary>
    KNS = 2,

}
