using Dapper;
using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Nasa.Resources;
using Microsoft.Extensions.Configuration;

namespace Nasa.Repository.Repositories;

public class RastreamentoTubaroesRepository : NpgsqlDapperHelper, IRastreamentoTubaroesRepository
{
    private const string selectQuery = @"
        SELECT 
            rast.id as Id,
            rast.time as Time,
            rast.temp_cC as TempCc,
            rast.lat as Lat,
            rast.lon as Lon,
            rast.comportamento as Comportamento,
            ST_AsText(geom) as Geometria
        FROM rastreamento_tubaroes rast";

    public RastreamentoTubaroesRepository(IConfiguration configuration, IDbConnectionFactory dbConnectionFactory) : base(
        configuration, dbConnectionFactory)
    {
        DefaultOrder = " rast.id ";
    }

    public async Task<IEnumerable<RastreamentoTubaroes>> Select(RastreamentoTubaroes item)
    {
        var select = BuildSelectQuery(item);

        var result = await this.QueryAsync<RastreamentoTubaroes>(
            item.Pagination.UsesPagination(),
            select.query,
            GetParametersForSelect(item)
        );

        return result;
    }

    public async Task<RastreamentoTubaroes> SelectById(RastreamentoTubaroes item)
    {
        var select = BuildSelectQuery(item);

        var result = await this.QueryAsync<RastreamentoTubaroes>(
            item.Pagination.UsesPagination(),
            select.query,
            select.parameters
        );

        if (result.Count() == 0)
        {
            throw new ProcessException(AppStrings.ERR_RegistroNaoEncontrado);
        }

        return result.First();
    }

    public async Task<long> CountSelect(RastreamentoTubaroes item)
    {
        var select = BuildSelectQuery(item, true);

        var resultado = await this.QueryAsync<long>(
            false,
            select.query,
            select.parameters
        );

        return resultado.FirstOrDefault();
    }

    public async Task<bool> CheckReferences(RastreamentoTubaroes obj)
    {
        const string sql =
            @"
                    SELECT 
                        (SELECT COUNT(*) FROM usuarios_paginas WHERE id_usuario = @IdUsuario) as TotalRefs
                 ";

        int result = await ExecuteScalarAsync(sql, obj);

        return result <= 0;
    }

    // public async Task<int> Insert(RastreamentoTubaroes item)
    // {
    //     const string sql = @"
    //             INSERT INTO usuarios (
    //                 nome, email, senha_hash, cpf, 
    //                 username, is_active, created_at, updated_at, updated_by
    //             )
    //             VALUES (
    //                 @Nome, @Email, @SenhaHash, @Cpf,
    //                 @Username, UPPER(@IsActive), NOW(), NOW(), @UpdatedBy
    //             )";
    //
    //     return await ExecuteAsync(sql, item);
    // }

    private object GetParametersForSelect(RastreamentoTubaroes item)
    {
        return new
        {
            // PK
            item.Id,
            item.Time,
            // Status
            item.Comportamento,
            // Columns Filters
            // Pagination.
            item.Pagination.StartRecordNumber,
            item.Pagination.ItemsPerPage,
            // Default filter.
            item.DefaultFilter
        };
    }

    private object GetParametersForSelect(FilterType filterType)
    {
        return new
        {
            // PK
            filterType.Code,
        };
    }

    private (string query, object parameters) BuildSelectQuery(RastreamentoTubaroes item, bool countOnly = false)
    {
        var (whereClause, parameters) = BuildWhereClause(item);
        var orderByClause = BuildOrderByClause(item);

        var query = countOnly
            ? $"SELECT COUNT(*) FROM rastreamentos_tubaroes rast {whereClause}"
            : $"{selectQuery} {whereClause} {orderByClause}";

        return (query, parameters);
    }

    private (string whereClause, DynamicParameters parameters) BuildWhereClause(RastreamentoTubaroes item)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        AddPaginationParameters(item, ref parameters);

        if (item.Id != 0)
        {
            conditions.Add("rast.id = @Id");
            parameters.Add("Id", item.Id);
        }

        if (!string.IsNullOrEmpty(item.Time))
        {
            conditions.Add("rast.time = @Time");
            parameters.Add("Time", item.Time);
        }
        
        if (item.Comportamento > 0)
        {
            conditions.Add("rast.comportamento = @Comportamento");
            parameters.Add("Comportamento", item.Comportamento);
        }

        if (item.Lat != 0)
        {
            conditions.Add("rast.lat = @Lat");
            parameters.Add("Lat", item.Lat);
        }
        
        if (item.Lon > 0)
        {
            conditions.Add("rast.lon = @Lon");
            parameters.Add("Lon", item.Lon);
        }

        // Filtro geral (busca em múltiplos campos)
        if (!string.IsNullOrEmpty(item.DefaultFilter))
        {
            var searchConditions = new[]
            {
                "UPPER(rast.id) LIKE UPPER(@DefaultFilter)",
                "UPPER(rast.time) LIKE UPPER(@DefaultFilter)",
                "UPPER(rast.temp_cc) LIKE UPPER(@DefaultFilter)",
                "UPPER(rast.comportamento) LIKE UPPER(@DefaultFilter)",
                "UPPER(rast.lat) LIKE UPPER(@DefaultFilter)",
                "UPPER(rast.lon) LIKE UPPER(@DefaultFilter)",
            };

            conditions.Add($"({string.Join(" OR ", searchConditions)})");
            parameters.Add("DefaultFilter", $"%{item.DefaultFilter}%");
        }

        var whereClause = conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : "";

        return (whereClause, parameters);
    }

    private void AddStringFilter(List<string> conditions, DynamicParameters parameters, string columnName,
        string paramName, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            conditions.Add($"UPPER({columnName}) LIKE UPPER(@{paramName})");
            parameters.Add(paramName, $"%{value}%");
        }
    }

    private string BuildOrderByClause(RastreamentoTubaroes item)
    {
        var orderBy = !string.IsNullOrEmpty(item.NewOrderBy)
            ? ProcessOrderBy(item.NewOrderBy)
            : DefaultOrder;

        return $"ORDER BY {orderBy}";
    }

    private string ProcessOrderBy(string orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return DefaultOrder;

        // Mapear campos do modelo para campos da tabela
        var fieldMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "rast.id" },
            { "Time", "rast.time" },
            { "TempCc", "rast.temp_cc" },
            { "Comportamento", "rast.comportamento" },
            { "Lon", "rast.lon" },
            { "Lat", "rast.lat" },
        };

        foreach (var mapping in fieldMappings)
        {
            orderBy = orderBy.Replace(mapping.Key, mapping.Value, StringComparison.OrdinalIgnoreCase);
        }

        return orderBy;
    }

    private string BuildOrderByClause(FilterType filterType)
    {
        var orderBy = !string.IsNullOrEmpty(filterType.NewOrderBy)
            ? ProcessOrderBy(filterType.NewOrderBy)
            : DefaultOrder;

        return $"ORDER BY {orderBy}";
    }

}