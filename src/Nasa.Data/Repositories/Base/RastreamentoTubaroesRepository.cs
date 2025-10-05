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
            rast.tempo as Tempo,
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

    public async Task<IEnumerable<RastreamentoTubaroes>> SelectById(RastreamentoTubaroes item)
    {
        var select = BuildSelectQuery(item);

        var result = await this.QueryAsync<RastreamentoTubaroes>(
            item.Pagination.UsesPagination(),
            select.query,
            GetParametersForSelect(item)
        );

        return result;
    }

    public async Task<long> CountSelect(RastreamentoTubaroes item)
    {
        var select = BuildSelectQuery(item, true);

        var resultado = await this.QueryAsync<long>(
            false,
            select.query,
            GetParametersForSelect(item)
        );

        return resultado.FirstOrDefault();
    }

    public async Task<int> Insert(IEnumerable<RastreamentoTubaroes> item)
    {
        const string sql = @"
                INSERT INTO rastreamento_tubaroes (
                    id, tempo, lat, lon, p_forrageio, comportamento,
                    chlor_a_ambiente, ssha_ambiente, created_at, temp_cc
                )
                VALUES (
                    @Id, @Tempo, @Lat, @Lon, @pForrageio, @Comportamento,
                    @ChlorAAmbiente,  @SshAAmbiente, Now(), @TempCc
                )";
    
        return await ExecuteAsync(sql, item);
    }

    private object GetParametersForSelect(RastreamentoTubaroes item)
    {
        return new
        {
            // PK
            item.Id,
            item.Tempo,
            // Status
            item.Lat,
            item.Lon,
            item.TempCc,
            item.PForrageio,
            item.Comportamento,
            item.ChlorAAmbiente,
            item.SshaAmbiente,
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
            ? $"SELECT COUNT(*) FROM rastreamento_tubaroes rast {whereClause}"
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

        if (item.Tempo.HasValue)
        {
            conditions.Add("rast.tempo = @Tempo");
            parameters.Add("Tempo", item.Tempo.Value);
        }
        
        if (!string.IsNullOrEmpty(item.Comportamento))
        {
            conditions.Add("rast.comportamento = @Comportamento");
            parameters.Add("Comportamento", item.Comportamento);
        }

        if (item.Lat > 0)
        {
            conditions.Add("rast.lat = @Lat");
            parameters.Add("Lat", item.Lat);
        }
        
        if (item.Lon > 0)
        {
            conditions.Add("rast.lon = @Lon");
            parameters.Add("Lon", item.Lon);
        }

        if (item.TempCc > 0)
        {
            conditions.Add("rast.temp_cc = @TempCc");
            parameters.Add("TempCc", item.TempCc);
        }
        
        if (item.PForrageio > 0)
        {
            conditions.Add("rast.p_forrageio = @PForrageio");
            parameters.Add("PForrageio", item.PForrageio);
        }
        
        if (item.ChlorAAmbiente > 0)
        {
            conditions.Add("rast.chlor_a_ambiente = @ChlorAAmbiente");
            parameters.Add("ChlorAAmbiente", item.ChlorAAmbiente);
        }
        
        if (item.SshaAmbiente > 0)
        {
            conditions.Add("rast.ssha_ambiente = @SshaAmbiente");
            parameters.Add("SshaAmbiente", item.SshaAmbiente);
        }

        // Filtro geral (busca em múltiplos campos)
        if (!string.IsNullOrEmpty(item.DefaultFilter))
        {
            var searchConditions = new[]
            {
                "CAST(rast.id AS TEXT) LIKE UPPER(@DefaultFilter)",
                "TO_CHAR(rast.tempo, 'YYYY-MM-DD HH24:MI:SS') LIKE UPPER(@DefaultFilter)",
                "CAST(rast.temp_cc AS TEXT) LIKE UPPER(@DefaultFilter)",
                "CAST(rast.comportamento AS TEXT) LIKE UPPER(@DefaultFilter)",
                "CAST(rast.lat AS TEXT) LIKE UPPER(@DefaultFilter)",
                "CAST(rast.lon AS TEXT) LIKE UPPER(@DefaultFilter)",
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
            { "Tempo", "rast.tempo" },
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