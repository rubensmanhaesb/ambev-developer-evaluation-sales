using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product line inside a sale. Product data is stored using the
/// External Identities pattern: the external product id and denormalized
/// product description are kept with the sale item.
/// </summary>
public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private SaleItem()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice) : this()
    {
        ProductId = productId;
        ProductName = productName;
        Update(quantity, unitPrice);
    }

    public void Update(int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new DomainException("Cannot update a cancelled item.");

        if (ProductId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (string.IsNullOrWhiteSpace(ProductName))
            throw new DomainException("Product name is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("It is not possible to sell more than 20 identical items.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = CalculateDiscountPercentage(quantity);

        var grossAmount = Quantity * UnitPrice;
        DiscountAmount = Math.Round(grossAmount * DiscountPercentage, 2, MidpointRounding.AwayFromZero);
        TotalAmount = grossAmount - DiscountAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static decimal CalculateDiscountPercentage(int quantity)
    {
        return quantity switch
        {
            > 20 => throw new DomainException("It is not possible to sell more than 20 identical items."),
            >= 10 => 0.20m,
            >= 4 => 0.10m,
            _ => 0m
        };
    }
}
