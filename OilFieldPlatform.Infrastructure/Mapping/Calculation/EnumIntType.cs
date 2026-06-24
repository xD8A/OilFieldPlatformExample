using System.Data;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace OilFieldPlatform.Infrastructure.Mapping.Calculation;

/// <summary>
/// Пользовательский тип NHibernate для хранения enum в виде smallint.
/// </summary>
/// <typeparam name="TEnum">Тип enum.</typeparam>
public sealed class EnumIntType<TEnum> : IUserType
    where TEnum : struct, Enum
{
    /// <summary>Тип неизменяемый.</summary>
    public bool IsMutable => false;

    /// <summary>Тип возвращаемого значения.</summary>
    public System.Type ReturnedType => typeof(TEnum);

    /// <summary>Типы SQL-колонок.</summary>
    public SqlType[] SqlTypes => [new SqlType(DbType.Int16)];

    /// <summary>Сравнение.</summary>
    public new bool Equals(object? x, object? y) => object.Equals(x, y);

    /// <summary>Хэш-код.</summary>
    public int GetHashCode(object? x) => x?.GetHashCode() ?? 0;

    /// <summary>Чтение из БД.</summary>
    public object? NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object? owner)
    {
        var val = rs[names[0]];
        if (val is null or DBNull)
            return null;
        return Enum.ToObject(typeof(TEnum), Convert.ToInt16(val, CultureInfo.InvariantCulture));
    }

    /// <summary>Запись в БД.</summary>
    public void NullSafeSet(DbCommand cmd, object? value, int index, ISessionImplementor session)
    {
        if (value is null)
            ((IDbDataParameter)cmd.Parameters[index]!).Value = DBNull.Value;
        else
            ((IDbDataParameter)cmd.Parameters[index]!).Value = Convert.ToInt16(value, CultureInfo.InvariantCulture);
    }

    /// <summary>Копирование.</summary>
    public object? DeepCopy(object? value) => value;

    /// <summary>Замена.</summary>
    public object? Replace(object? original, object? target, object? owner) => original;

    /// <summary>Сборка из кэша.</summary>
    public object? Assemble(object? cached, object? owner) => cached;

    /// <summary>Разборка для кэша.</summary>
    public object? Disassemble(object? value) => value;
}
