using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public record SaleCreatedNotification(Guid SaleId, string SaleNumber) : INotification;
