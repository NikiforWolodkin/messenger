using MessengerApiDomain.Models;

namespace MessengerApiDomain.RepositoryInterfaces;

public interface IOperationLogRepository
{
    Task AddAsync(OperationLog log);
}
