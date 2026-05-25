using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SalesEventLogger :
    INotificationHandler<SaleCreatedNotification>,
    INotificationHandler<SaleModifiedNotification>,
    INotificationHandler<SaleCancelledNotification>,
    INotificationHandler<ItemCancelledNotification>
{
    private readonly ILogger<SalesEventLogger> _logger;

    public SalesEventLogger(ILogger<SalesEventLogger> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleCreated event published. SaleId: {SaleId}; SaleNumber: {SaleNumber}", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleModified event published. SaleId: {SaleId}; SaleNumber: {SaleNumber}", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleCancelled event published. SaleId: {SaleId}; SaleNumber: {SaleNumber}", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(ItemCancelledNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ItemCancelled event published. SaleId: {SaleId}; ItemId: {ItemId}; SaleNumber: {SaleNumber}", notification.SaleId, notification.ItemId, notification.SaleNumber);
        return Task.CompletedTask;
    }
}
