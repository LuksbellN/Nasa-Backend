using Nasa.Domain.Model;

namespace Nasa.Domain.Repositories;

public interface IRastreamentoTubaroesRepository
{
    Task<IEnumerable<RastreamentoTubaroes>> Select(RastreamentoTubaroes item);

    Task<IEnumerable<RastreamentoTubaroes>> SelectById(RastreamentoTubaroes item);
    
    Task<long> CountSelect(RastreamentoTubaroes item);
    
    Task<int> Insert(IEnumerable<RastreamentoTubaroes> item);
    
    Task<IEnumerable<RastreamentoTubaroes>> SelectLatestPositions();
}