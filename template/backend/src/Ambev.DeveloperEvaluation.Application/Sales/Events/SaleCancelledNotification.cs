using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public record SaleCancelledNotification(Guid SaleId, string SaleNumber) : INotification;
