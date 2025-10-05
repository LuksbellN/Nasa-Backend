using Nasa.Domain.Model;

namespace Nasa.Domain.Services.Interfaces;

public interface ILogsService
{
    Log Insert(Log item);

    int Update(Log item);

    DateTime GetDataBaseDate();
}