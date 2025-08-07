using System.Data;
using Dapper;

namespace Vertr.MarketData.DataAccess.Mappers;
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
    {
        // Handle potential null values or other types if necessary
        if (value is DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
        throw new InvalidCastException($"Cannot cast {value.GetType()} to DateOnly.");
    }

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date; // Or DbType.DateTime if your database column stores DateTime
        parameter.Value = value.ToDateTime(TimeOnly.MinValue); // Convert DateOnly to DateTime for database
    }
}
