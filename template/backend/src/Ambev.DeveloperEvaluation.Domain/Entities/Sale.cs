using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale aggregate root. Customer, branch and product references
/// follow the External Identities pattern with denormalized descriptions.
/// </summary>
public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = [];

    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private Sale()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Sale(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName) : this()
    {
        SetHeader(saleNumber, saleDate, customerId, customerName, branchId, branchName);
    }

    public void Update(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<SaleItem> items)
    {
        EnsureSaleCanBeChanged();
        SetHeader(saleNumber, saleDate, customerId, customerName, branchId, branchName);

        _items.Clear();
        foreach (var item in items)
        {
            AddItem(item);
        }

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(SaleItem item)
    {
        EnsureSaleCanBeChanged();

        if (item is null)
            throw new DomainException("Sale item is required.");

        _items.Add(item);
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        foreach (var item in _items)
        {
            item.Cancel();
        }

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelItem(Guid itemId)
    {
        EnsureSaleCanBeChanged();

        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            throw new DomainException($"Item {itemId} was not found in sale {Id}.");

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecalculateTotal()
    {
        TotalAmount = _items
            .Where(item => !item.IsCancelled)
            .Sum(item => item.TotalAmount);
    }

    private void SetHeader(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number is required.");

        if (customerId == Guid.Empty)
            throw new DomainException("Customer id is required.");

        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required.");

        if (branchId == Guid.Empty)
            throw new DomainException("Branch id is required.");

        if (string.IsNullOrWhiteSpace(branchName))
            throw new DomainException("Branch name is required.");

        SaleNumber = saleNumber.Trim();
        SaleDate = saleDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(saleDate, DateTimeKind.Utc)
            : saleDate.ToUniversalTime();
        CustomerId = customerId;
        CustomerName = customerName.Trim();
        BranchId = branchId;
        BranchName = branchName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureSaleCanBeChanged()
    {
        if (IsCancelled)
            throw new DomainException("Cannot change a cancelled sale.");
    }
}
