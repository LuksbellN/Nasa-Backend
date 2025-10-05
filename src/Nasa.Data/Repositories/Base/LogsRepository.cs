using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Nasa.Repository.Repositories;

public class LogsRepository : NpgsqlDapperHelper, ILogsRepository
{
    
    public LogsRepository(IConfiguration configuration, IDbConnectionFactory dbConnectionFactory) : base(configuration,
        dbConnectionFactory)
    {
    }

    public Log Insert(Log item)
    {
        var sql =
            @"
                INSERT INTO LOGS(DES_OBJETO, DES_OPERACAO, OBS_OPERACAO, USERNAME, DTA_OPERACAO, DES_ERRO)
                VALUES(SUBSTR(:DesObjeto, 1, 50), SUBSTR(:DesOperacao, 1, 200), SUBSTR(:ObsOperacao, 1, 100), SUBSTR(:Username, 1, 100), now(), SUBSTR(:DesErro, 1, 500))
                 ";

        Execute(sql, item);

        return item;
    }

    public int Update(Log item)
    {
        var sql =
            @"
                UPDATE LOGS
                   SET DES_OBJETO   = SUBSTR(:DesObjeto, 1, 50)
                     , DES_OPERACAO = SUBSTR(:DesOperacao, 1, 200)
                     , OBS_OPERACAO = SUBSTR(:ObsOperacao, 1, 100)
                     , DES_USUARIO  = SUBSTR(:DesUsuario, 1, 100)
                     , DES_ERRO     = SUBSTR(:DesErro, 1, 500)
                 WHERE ID_LOG       = :IdLog
                 ";

        return Execute(sql, item);
    }
}