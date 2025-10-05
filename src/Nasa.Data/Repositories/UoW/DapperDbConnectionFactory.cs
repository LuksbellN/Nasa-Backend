using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace Nasa.Repository.Repositories.UoW;

public class DapperDbConnectionFactory : IDbConnectionFactory
{
    private readonly IDictionary<DatabaseConnectionName, string> _connectionDict;

    public DapperDbConnectionFactory(IDictionary<DatabaseConnectionName, string> connectionDict)
    {
        _connectionDict = connectionDict;
    }

    private string GetConnectionString(DatabaseConnectionName connectionName)
    {
        string connectionString;
        if (_connectionDict.TryGetValue(connectionName, out connectionString!))
            return connectionString;

        throw new ArgumentNullException();
    }

    public NpgsqlConnection CreateDbConnection(DatabaseConnectionName connectionName)
    {
        var connectionString = GetConnectionString(connectionName);
        return new NpgsqlConnection(connectionString);
    }
    
}