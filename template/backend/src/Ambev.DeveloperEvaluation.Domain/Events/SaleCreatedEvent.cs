namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, string SaleNumber);
