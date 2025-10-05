using System.Data;
using Dapper;
using Nasa.Domain.Model;
using Nasa.Repository.Repositories.UoW;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Nasa.Repository.Repositories;

public class NpgsqlDapperHelper
{
    private readonly IConfiguration Configuration;

    public NpgsqlConnection DbConnection { get; private set; }

    public string DefaultOrder { get; set; }

    public const string QueryPagination =
        @"
          {0} -- query
          LIMIT @ItemsPerPage OFFSET @StartRecordNumber - 1
         ";

    public NpgsqlDapperHelper(IConfiguration configuration, IDbConnectionFactory dbConnectionFactory)
    {
        Configuration = configuration;
        DbConnection = dbConnectionFactory.CreateDbConnection(DatabaseConnectionName.NpgsqlDbConnection);
    }

    #region Public

    #region Transaction

    public NpgsqlTransaction GetNpgsqlTransaction()
    {
        if (DbConnection.State == ConnectionState.Closed)
            DbConnection.Open();

        return DbConnection.BeginTransaction();
    }

    #endregion Transaction

    #region Query

    public IEnumerable<T> Query<T>(bool paged, string sql, object param)
    {
        return paged
                ? QueryPaged<T>(sql, param)
                : Query<T>(sql, param)
            ;
    }

    private IEnumerable<T> Query<T>(string sql, object param)
    {
        List<T> resultado;

        var con = DbConnection;

        resultado = con.Query<T>(sql, param).ToList();

        return resultado;
    }

    public string GetOrderClause(BaseModel baseModel)
    {
        if (string.IsNullOrEmpty(DefaultOrder))
            throw new MissingFieldException("Default order by not found");

        if (baseModel != null && !string.IsNullOrEmpty(baseModel.NewOrderBy))
            return string.Format(" {0} ", baseModel.NewOrderBy);

        return DefaultOrder;
    }

    public string GetWhereCondition(string dataBaseColumnName, string paramName, string actualWhere, object itemValue)
    {
        return ConfigureWhereCondition(dataBaseColumnName, paramName, actualWhere, itemValue, "and");
    }

    public string GetWhereLikeCondition(string dataBaseColumnName, string paramName, string actualWhere,
        object itemValue)
    {
        string condition = string.Empty;

        if (CheckObject(itemValue))
        {
            condition = string.Format(" {0} UPPER({1}) like UPPER('%' || :{2} || '%') ",
                string.IsNullOrEmpty(actualWhere) ? string.Empty : " and ", dataBaseColumnName, paramName);
        }

        return condition;
    }

    public string GetWhereCondition(string dataBaseColumnName, string paramName, string actualWhere, object itemValue,
        string logicOperator)
    {
        return ConfigureWhereCondition(dataBaseColumnName, paramName, actualWhere, itemValue, logicOperator);
    }

    private string ConfigureWhereCondition(string dataBaseColumnName, string paramName, string actualWhere,
        object itemValue, string logicOperator)
    {
        var condition = string.Empty;

        if (itemValue != null && ((itemValue is string) || (itemValue is DateTime) ||
                                  ((itemValue is long @int) && @int != 0) || ((itemValue is int int1) && int1 != 0)))
            condition =
                string.Format(" {0} {1} = :{2} ",
                    string.IsNullOrEmpty(actualWhere) ? string.Empty : string.Format(" {0} ", logicOperator),
                    dataBaseColumnName,
                    paramName
                );

        return condition;
    }

    public string GetBetweenWhereCondition(string dateColumn, string dateStartName, string dateEndName,
        string actualWhere, string dateStart, string dateEnd, string dateMask = "YYYY-MM-DD")
    {
        return ConfigureBetweenWhereCondition(dateColumn, dateStartName, dateEndName, actualWhere, dateStart, dateEnd,
            dateMask);
    }

    private string ConfigureBetweenWhereCondition(string dateColumn, string dateStartName, string dateEndName,
        string actualWhere, string dateStart, string dateEnd, string dateMask)
    {
        var condition = string.Empty;


        if (string.IsNullOrEmpty(dateStart) || string.IsNullOrEmpty(dateEnd))
        {
            return condition;
        }

        condition =
            string.Format(" {0} {1} BETWEEN TO_DATE(:{2}, '{3}') AND TO_DATE(:{4}, '{3}') ",
                string.IsNullOrEmpty(actualWhere) ? string.Empty : " AND ",
                dateColumn,
                dateStartName,
                dateMask,
                dateEndName
            );

        return condition;
    }

    private IEnumerable<T> QueryPaged<T>(string sql, object param)
    {
        sql = string.Format(QueryPagination, sql);

        return Query<T>(sql, param);
    }

    #endregion Query

    #region Dml

    public int Execute(string sql, object param)
    {
        int resultado;

        var con = DbConnection;

        resultado = con.Execute(sql, param);

        return resultado;
    }

    public int ExecuteInTransaction(string sql, object param, NpgsqlTransaction npgsqlTransaction)
    {
        if (npgsqlTransaction == null)
            return -1;

        int resultado;
        var con = npgsqlTransaction.Connection;

        resultado = con.Execute(sql, param);
        return resultado;
    }

    public IEnumerable<T> QueryInTransaction<T>(string sql, object param, NpgsqlTransaction npgsqlTransaction)
    {
        if (npgsqlTransaction == null)
            return new List<T>();

        List<T> resultado;
        var con = npgsqlTransaction.Connection;

        resultado = con.Query<T>(sql, param).ToList();
        return resultado;
    }

    public dynamic ExecuteDbProc<T>(string sql, DynamicParameters parameters,
        CommandType commandType = CommandType.StoredProcedure)
    {
        dynamic result;

        var con = DbConnection;

        result =
            con.Query<T>(
                sql,
                parameters,
                commandType: commandType
            );

        return result;
    }

    public async Task<dynamic> ExecuteDbProcAsync<T>(string sql, DynamicParameters parameters,
        CommandType commandType = CommandType.StoredProcedure)
    {
        var t = await Task.Run(() => { return ExecuteDbProc<T>(sql, parameters, commandType); });

        return t;
    }

    public bool ExecuteBulkCommand<T>(string sql, List<NpgsqlParameter> parameters, IList<T> data)
    {
        bool resultado = false;
        var con = DbConnection;

        using (var command = con.CreateCommand())
        {
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            foreach (var par in parameters)
            {
                command.Parameters.Add(par);
            }

            int result = command.ExecuteNonQuery();
            if (result == data.Count)
                resultado = true;
        }

        return resultado;
    }

    public bool ExecuteBulkCommandInTransaction<T>(string sql, List<NpgsqlParameter> parameters, IList<T> data,
        NpgsqlTransaction npgsqlTransaction)
    {
        if (npgsqlTransaction == null)
            return false;

        bool resultado = false;

        using (var command = npgsqlTransaction.Connection.CreateCommand())
        {
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.Transaction = npgsqlTransaction;

            foreach (var par in parameters)
            {
                command.Parameters.Add(par);
            }

            int result = command.ExecuteNonQuery();
            if (result == data.Count)
                resultado = true;
        }

        return resultado;
    }

    #endregion Dml

    #region Useful

    public void AddPaginationParameters(BaseModel baseModel, ref DynamicParameters parameters)
    {
        if (baseModel.Pagination.UsesPagination())
        {
            parameters.Add("ItemsPerPage", baseModel.Pagination.ItemsPerPage);
            parameters.Add("StartRecordNumber", baseModel.Pagination.StartRecordNumber);
        }
    }
    
    public long GetNextSequenceValue(string sequenceName)
    {
        var sql = $" SELECT nextval({sequenceName}) ";

        var resultado = Query<long>(sql, null).FirstOrDefault();

        return resultado;
    }

    public DateTime GetDataBaseDate()
    {
        var query = @" SELECT CURRENT_TIMESTAMP ";
        var dataHoraBanco = this.Query<DateTime>(query, null).FirstOrDefault();

        return dataHoraBanco;
    }

    private bool CheckObject(object itemValue)
    {
        return
            itemValue != null &&
            (
                (itemValue is DateTime) ||
                (itemValue is string) ||
                ((itemValue is int i) && i != 0) ||
                ((itemValue is long l) && l != 0)
            );
    }

    #endregion Useful

    #region Async

    public async Task<IEnumerable<T>> QueryAsync<T>(bool paged, string sql, object param)
    {
        var t = await Task.Run(() => { return Query<T>(paged, sql, param); });

        return t;
    }

    public async Task<int> ExecuteAsync(string sql, object param)
    {
        var t = await Task.Run(() => { return Execute(sql, param); });

        return t;
    }

    public async Task<int> ExecuteInTransactionAsync(string sql, object param, NpgsqlTransaction npgsqlTransaction)
    {
        var t = await Task.Run(() => { return ExecuteInTransaction(sql, param, npgsqlTransaction); });

        return t;
    }

    public async Task<bool> ExecuteBulkCommandInTransactionAsync<T>(string sql, List<NpgsqlParameter> parameters,
        IList<T> data, NpgsqlTransaction npgsqlTransaction)
    {
        var t = await Task.Run(() =>
        {
            return ExecuteBulkCommandInTransaction<T>(sql, parameters, data, npgsqlTransaction);
        });

        return t;
    }

    public async Task<int> ExecuteScalarAsync(string sql, object param)
    {
        int result;

        var con = DbConnection;

        result = await con.ExecuteScalarAsync<int>(sql, param);

        return result;
    }

    #endregion Async

    #endregion Public
}