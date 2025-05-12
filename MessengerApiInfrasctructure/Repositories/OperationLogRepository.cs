using MessengerApiDomain.Models;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiInfrasctructure.Data;

namespace MessengerApiInfrasctructure.Repositories;

public class OperationLogRepository : IOperationLogRepository
{
    private readonly DataContext _context;

    public OperationLogRepository(DataContext context)
    {
        _context = context;
    }

    async Task IOperationLogRepository.AddAsync(OperationLog log)
    {
        await _context.OperationLogs.AddAsync(log);

        await _context.SaveChangesAsync();
    }
}
