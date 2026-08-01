using MediatR;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Contracts;
using PIMS_MS.Modules.Inventory.Database;

namespace PIMS_MS.Modules.Inventory.Features.StockManagement.EventHandlers;

public class DecreaseStockOnDispatchHandler : INotificationHandler<TransferDispatchedIntegrationEvent>
{
    private readonly InventoryDbContext _dbContext;
    public DecreaseStockOnDispatchHandler(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Handle(TransferDispatchedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        foreach (var item in notification.Items)
        {
            var stock = await _dbContext.Stocks
                .FirstOrDefaultAsync(s => s.LocationId == notification.OriginLocationId && s.SparePartId == item.SparePartId, cancellationToken);

            if (stock != null)
            {
                stock.Decrement(item.Quantity); 
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}