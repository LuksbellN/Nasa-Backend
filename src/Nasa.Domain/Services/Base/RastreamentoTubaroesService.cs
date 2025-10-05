using System.Xml.Linq;
using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Nasa.Domain.Services.Interfaces;
using Nasa.Resources;

namespace Nasa.Domain.Services;

public class RastreamentoTubaroesService : IRastreamentoTubaroesService
{
    private readonly IRastreamentoTubaroesRepository _rastreamentoTubaroesRepository;

    public RastreamentoTubaroesService(IRastreamentoTubaroesRepository rastreamentoTubaroesRepository)
    {
        _rastreamentoTubaroesRepository = rastreamentoTubaroesRepository;
    }

    public async Task<ProcessResult> Select(RastreamentoTubaroes item)
    {
        ProcessResult result = new ProcessResult();

        try
        {
            if (item.Pagination.UsesPagination())
            {
                item.Pagination.TotalRecords = await _rastreamentoTubaroesRepository.CountSelect(item);
            }

            result = new ProcessResult()
            {
                Data = await _rastreamentoTubaroesRepository.Select(item),
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

    public async Task<ProcessResult> SelectById(RastreamentoTubaroes item)
    {
        ProcessResult result = new ProcessResult();

        try
        {
            result = new ProcessResult()
            {
                Data = await _rastreamentoTubaroesRepository.SelectById(item)
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

    public async Task<ProcessResult> Insert(IEnumerable<RastreamentoTubaroes> items)
    {
        ProcessResult processResult = new ProcessResult() { ProcessMessage = AppStrings.INF_RegistroCriadoSucesso };

        try
        {
            var validItems = new List<RastreamentoTubaroes>();

            foreach (var item in items)
            {
                if (!await PkExists(item))
                {
                    validItems.Add(item);
                }
            }
            
            if (!validItems.Any())
            {
                return new ProcessResult()
                    { Success = false, ProcessMessage = AppStrings.ERR_FalhaInserirRegistro };
            }

            await _rastreamentoTubaroesRepository.Insert(validItems);
        }
        catch (ProcessException ex)
        {
            processResult.RecordException(ex, ex.Message);
        }
        catch (Exception ex)
        {
            processResult.RecordException(ex, AppStrings.ERR_FalhaInserirRegistro);
        }

        return processResult;
    }

    private bool PkInformed(RastreamentoTubaroes item)
    {
        return string.IsNullOrEmpty(item.Id.ToString()) || string.IsNullOrEmpty(item.Tempo.ToString());
    }

    private async Task<bool> PkExists(RastreamentoTubaroes item)
    {
        var query = await _rastreamentoTubaroesRepository.CountSelect(new RastreamentoTubaroes()
        {
            Id = item.Id,
            Tempo = item.Tempo,
        });

        return query > 0;
    }

    public async Task<ProcessResult> SelectLatestPositions()
    {
        ProcessResult result = new ProcessResult();

        try
        {
            result = new ProcessResult()
            {
                Data = await _rastreamentoTubaroesRepository.SelectLatestPositions()
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
}