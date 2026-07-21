using MediatR;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Contracts;
using PIMS_MS.Modules.Inventory.Database;

namespace PIMS_MS.Modules.Inventory;

public class CheckLocationExistsHandler : IRequestHandler<CheckLocationExistsQuery, bool>
{
    private readonly InventoryDbContext _dbContext;

    public CheckLocationExistsHandler(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(CheckLocationExistsQuery request, CancellationToken cancellationToken)
    {
        // Responde true si la locación existe en su base de datos
        return await _dbContext.Locations.AnyAsync(l => l.Id == request.Location, cancellationToken);
    }
}