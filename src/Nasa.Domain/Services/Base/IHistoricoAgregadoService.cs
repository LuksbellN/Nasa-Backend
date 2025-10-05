using Nasa.Domain.Model;

namespace Nasa.Domain.Services.Interfaces;

public interface IHistoricoAgregadoService
{
    Task<ProcessResult> Select(HistoricoAgregado item);
    Task<ProcessResult> SelectById(HistoricoAgregado item);
}

