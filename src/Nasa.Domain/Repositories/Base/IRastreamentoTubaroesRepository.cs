using Nasa.Domain.Model;

namespace Nasa.Domain.Repositories;

public interface IRastreamentoTubaroesRepository
{
    Task<IEnumerable<RastreamentoTubaroes>> Select(RastreamentoTubaroes item);

    Task<RastreamentoTubaroes> SelectById(RastreamentoTubaroes item);
    
    Task<long> CountSelect(RastreamentoTubaroes item);
    
    // Task<int> Insert(RastreamentoTubaroes item);
}