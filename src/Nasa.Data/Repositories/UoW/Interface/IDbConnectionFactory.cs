using System.Data;
using Nasa.Repository.Repositories.UoW;
using Npgsql;

namespace Nasa.Repository;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateDbConnection(DatabaseConnectionName connectionName);

}