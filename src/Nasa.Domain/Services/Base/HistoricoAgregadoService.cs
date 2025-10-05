using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Nasa.Domain.Services.Interfaces;
using Nasa.Resources;

namespace Nasa.Domain.Services;

public class HistoricoAgregadoService : IHistoricoAgregadoService
{
    private readonly IHistoricoAgregadoRepository _historicoAgregadoRepository;

    public HistoricoAgregadoService(IHistoricoAgregadoRepository historicoAgregadoRepository)
    {
        _historicoAgregadoRepository = historicoAgregadoRepository;
    }

    
    public async Task<ProcessResult> Select(HistoricoAgregado item)
    {
        ProcessResult result = new ProcessResult();

        try
        {
            if (item.Pagination.UsesPagination())
            {
                item.Pagination.TotalRecords = await _historicoAgregadoRepository.CountSelect(item);
            }

            result = new ProcessResult()
            {
                Data = await _historicoAgregadoRepository.Select(item),
                Pagination = item.Pagination
            };
        }
        catch (ProcessException ex)
        {
            result.RecordException(ex, ex.Message);
        }
        catch (Exception ex)
        {
            result.RecordException(ex, AppStrings.ERR_FalhaRecuperarRegistros);
        }

        return result;
    }
    
    
    public async Task<ProcessResult> SelectById(HistoricoAgregado item)
    {
        ProcessResult result = new ProcessResult();

        try
        {
            result = new ProcessResult()
            {
                Data = await _historicoAgregadoRepository.SelectById(item)
            };
        }
        catch (ProcessException ex)
        {
            result.RecordException(ex, ex.Message);
        }
        catch (Exception ex)
        {
            result.RecordException(ex, AppStrings.ERR_FalhaRecuperarRegistro);
        }

        return result;
    }

}

