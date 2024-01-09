using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.DbModels;

internal class DbModel : IDbModel
{
}

internal class DbModelOfString : IDbModel<string>
{
    public string Id { get; set; }
}

internal class DbModelOfInteger : IDbModel<int>
{
    public int Id { get; set; }
}

internal class DbModelOfDouble : IDbModel<double>
{
    public double Id { get; set; }
}

internal class DbModelOfLong : IDbModel<long>
{
    public long Id { get; set; }
}

internal class DbModelOfGuid : IDbModel<Guid>
{
    public Guid Id { get; set; }
}

internal class SqlDbModel : SqlModelBase
{
}

internal class ChildOfSqlDbModel : SqlDbModel
{
}

internal class ChildOfDbModel : DbModel
{
}

internal class OutOfIDbModelFamily
{
}