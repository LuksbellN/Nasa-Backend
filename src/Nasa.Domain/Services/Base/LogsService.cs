using Nasa.Domain.Model;
using Nasa.Domain.Repositories;
using Nasa.Domain.Services.Interfaces;

namespace Nasa.Domain.Services;

public class LogsService : ILogsService
{
    private readonly ILogsRepository _logsRepository;

    public LogsService(ILogsRepository logsRepository)
    {
        _logsRepository = logsRepository;
    }

    public Log Insert(Log item)
    {
        return _logsRepository.Insert(item);
    }

    public int Update(Log item)
    {
        return _logsRepository.Update(item);
    }

    public DateTime GetDataBaseDate()
    {
        return _logsRepository.GetDataBaseDate();
    }
}