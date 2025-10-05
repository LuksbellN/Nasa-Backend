using Nasa.Domain.Model;

namespace Nasa.Domain.Repositories;

public interface IHistoricoAgregadoRepository
{
    Task<IEnumerable<HistoricoAgregado>> Select(HistoricoAgregado item);
    Task<IEnumerable<HistoricoAgregado>> SelectById(HistoricoAgregado item);
    Task<long> CountSelect(HistoricoAgregado item);
}

