using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public record ItemCancelledNotification(Guid SaleId, Guid ItemId, string SaleNumber) : INotification;
