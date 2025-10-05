using Nasa.Domain.Model;

namespace Nasa.Domain.Services.Interfaces;

public interface IRastreamentoTubaroesService
{
    Task<ProcessResult> Select(RastreamentoTubaroes item);

    Task<ProcessResult> SelectById(RastreamentoTubaroes item);
   
    Task<ProcessResult> Insert(IEnumerable<RastreamentoTubaroes> item);

}