using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public record SaleModifiedNotification(Guid SaleId, string SaleNumber) : INotification;
