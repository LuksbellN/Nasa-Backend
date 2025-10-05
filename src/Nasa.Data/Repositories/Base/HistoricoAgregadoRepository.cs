using Dapper;
using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Nasa.Repository.Repositories;

public class HistoricoAgregadoRepository : NpgsqlDapperHelper, IHistoricoAgregadoRepository
{
    private const string selectQuery = @"
        SELECT 
            ha.id as Id,
            ha.data as Data,
            ha.hora as Hora,
            ha.lat_media as LatMedia,
            ha.lon_media as LonMedia,
            ST_AsText(ha.geom_media) as GeomMedia,
            ha.p_forrageio_media as PForrageioMedia,
            ha.comportamento_predominante as ComportamentoPredominante,
            ha.chlor_a_media as ChlorAMedia,
            ha.ssha_media as SshaMedia,
            ha.total_registros as TotalRegistros,
            ha.created_at as CreatedAt
        FROM historico_agregado ha";

    public HistoricoAgregadoRepository(IConfiguration configuration, IDbConnectionFactory dbConnectionFactory) : base(
        configuration, dbConnectionFactory)
    {
        DefaultOrder = " ha.data DESC, ha.hora DESC ";
    }

    public async Task<IEnumerable<HistoricoAgregado>> Select(HistoricoAgregado item)
    {
        var select = BuildSelectQuery(item);

        var result = await this.QueryAsync<HistoricoAgregado>(
            item.Pagination.UsesPagination(),
            select.query,
            GetParametersForSelect(item)
        );

        return result;
    }

    public async Task<IEnumerable<HistoricoAgregado>> SelectById(HistoricoAgregado item)
    {
        var select = BuildSelectQuery(item);

        var result = await this.QueryAsync<HistoricoAgregado>(
            item.Pagination.UsesPagination(),
            select.query,
            GetParametersForSelect(item)
        );

        return result;
    }

    public async Task<long> CountSelect(HistoricoAgregado item)
    {
        var select = BuildSelectQuery(item, true);

        var resultado = await this.QueryAsync<long>(
            false,
            select.query,
            GetParametersForSelect(item)
        );

        return resultado.FirstOrDefault();
    }


    private object GetParametersForSelect(HistoricoAgregado item)
    {
        return new
        {
            // PK
            item.Id,
            item.Data,
            item.Hora,
            // Filtros
            item.ComportamentoPredominante,
            // Pagination
            item.Pagination.StartRecordNumber,
            item.Pagination.ItemsPerPage,
            // Default filter
            item.DefaultFilter
        };
    }

    private (string query, object parameters) BuildSelectQuery(HistoricoAgregado item, bool countOnly = false)
    {
        var (whereClause, parameters) = BuildWhereClause(item);
        var orderByClause = BuildOrderByClause(item);

        var query = countOnly
            ? $"SELECT COUNT(*) FROM historico_agregado ha {whereClause}"
            : $"{selectQuery} {whereClause} {orderByClause}";

        return (query, parameters);
    }

    private (string whereClause, DynamicParameters parameters) BuildWhereClause(HistoricoAgregado item)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        AddPaginationParameters(item, ref parameters);

        if (item.Id != 0)
        {
            conditions.Add("ha.id = @Id");
            parameters.Add("Id", item.Id);
        }

        if (item.Data.HasValue)
        {
            conditions.Add("ha.data = @Data");
            parameters.Add("Data", item.Data.Value.Date);
        }
        
        if (item.Hora.HasValue)
        {
            conditions.Add("ha.hora = @Hora");
            parameters.Add("Hora", item.Hora.Value);
        }

        if (item.ComportamentoPredominante.HasValue && item.ComportamentoPredominante > 0)
        {
            conditions.Add("ha.comportamento_predominante = @ComportamentoPredominante");
            parameters.Add("ComportamentoPredominante", item.ComportamentoPredominante.Value);
        }

        // Filtro geral (busca em múltiplos campos)
        if (!string.IsNullOrEmpty(item.DefaultFilter))
        {
            var searchConditions = new[]
            {
                "CAST(ha.id AS TEXT) LIKE UPPER(@DefaultFilter)",
                "TO_CHAR(ha.data, 'YYYY-MM-DD') LIKE UPPER(@DefaultFilter)",
                "CAST(ha.hora AS TEXT) LIKE UPPER(@DefaultFilter)",
                "CAST(ha.comportamento_predominante AS TEXT) LIKE UPPER(@DefaultFilter)",
                "CAST(ha.total_registros AS TEXT) LIKE UPPER(@DefaultFilter)",
            };

            conditions.Add($"({string.Join(" OR ", searchConditions)})");
            parameters.Add("DefaultFilter", $"%{item.DefaultFilter}%");
        }

        var whereClause = conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : "";

        return (whereClause, parameters);
    }

    private string BuildOrderByClause(HistoricoAgregado item)
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
            { "Id", "ha.id" },
            { "Data", "ha.data" },
            { "Hora", "ha.hora" },
            { "LatMedia", "ha.lat_media" },
            { "LonMedia", "ha.lon_media" },
            { "ComportamentoPredominante", "ha.comportamento_predominante" },
            { "TotalRegistros", "ha.total_registros" },
            { "CreatedAt", "ha.created_at" },
        };

        foreach (var mapping in fieldMappings)
        {
            orderBy = orderBy.Replace(mapping.Key, mapping.Value, StringComparison.OrdinalIgnoreCase);
        }

        return orderBy;
    }
}

