using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

internal static class SalesMapper
{
    public static SaleResult ToResult(this Sale sale)
    {
        return new SaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToResult()).ToList()
        };
    }

    public static SaleItemResult ToResult(this SaleItem item)
    {
        return new SaleItemResult
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountPercentage = item.DiscountPercentage,
            DiscountAmount = item.DiscountAmount,
            TotalAmount = item.TotalAmount,
            IsCancelled = item.IsCancelled
        };
    }

    public static SaleItem ToEntity(this SaleItemInput item)
    {
        return new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
    }
}
