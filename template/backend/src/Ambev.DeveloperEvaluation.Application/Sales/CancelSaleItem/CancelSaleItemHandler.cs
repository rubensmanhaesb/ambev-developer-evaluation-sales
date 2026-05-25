using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<SaleResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {command.SaleId} not found.");

        sale.CancelItem(command.ItemId);
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _mediator.Publish(new ItemCancelledNotification(updatedSale.Id, command.ItemId, updatedSale.SaleNumber), cancellationToken);

        return updatedSale.ToResult();
    }
}
