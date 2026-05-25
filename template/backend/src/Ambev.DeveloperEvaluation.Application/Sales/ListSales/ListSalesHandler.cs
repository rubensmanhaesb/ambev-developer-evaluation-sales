using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, IReadOnlyCollection<SaleResult>>
{
    private readonly ISaleRepository _saleRepository;

    public ListSalesHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<IReadOnlyCollection<SaleResult>> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.ListAsync(cancellationToken);
        return sales.Select(sale => sale.ToResult()).ToList();
    }
}
