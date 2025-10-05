using Nasa.Domain.Model;

namespace Nasa.Domain.Repositories;

public interface ILogsRepository
{
    Log Insert(Log item);

    int Update(Log item);

    DateTime GetDataBaseDate();
}