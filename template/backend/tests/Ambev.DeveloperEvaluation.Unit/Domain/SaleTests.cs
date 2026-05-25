using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public class SaleTests
{
    [Theory(DisplayName = "Given item quantity below four When creating item Then no discount is applied")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void SaleItem_QuantityBelowFour_ShouldNotApplyDiscount(int quantity)
    {
        var item = CreateItem(quantity, 100m);

        item.DiscountPercentage.Should().Be(0m);
        item.DiscountAmount.Should().Be(0m);
        item.TotalAmount.Should().Be(quantity * 100m);
    }

    [Theory(DisplayName = "Given item quantity from four to nine When creating item Then ten percent discount is applied")]
    [InlineData(4)]
    [InlineData(9)]
    public void SaleItem_QuantityBetweenFourAndNine_ShouldApplyTenPercentDiscount(int quantity)
    {
        var item = CreateItem(quantity, 100m);

        item.DiscountPercentage.Should().Be(0.10m);
        item.DiscountAmount.Should().Be(quantity * 10m);
        item.TotalAmount.Should().Be(quantity * 90m);
    }

    [Theory(DisplayName = "Given item quantity from ten to twenty When creating item Then twenty percent discount is applied")]
    [InlineData(10)]
    [InlineData(20)]
    public void SaleItem_QuantityBetweenTenAndTwenty_ShouldApplyTwentyPercentDiscount(int quantity)
    {
        var item = CreateItem(quantity, 100m);

        item.DiscountPercentage.Should().Be(0.20m);
        item.DiscountAmount.Should().Be(quantity * 20m);
        item.TotalAmount.Should().Be(quantity * 80m);
    }

    [Fact(DisplayName = "Given item quantity above twenty When creating item Then throws domain exception")]
    public void SaleItem_QuantityAboveTwenty_ShouldThrowDomainException()
    {
        var act = () => CreateItem(21, 100m);

        act.Should().Throw<DomainException>()
            .WithMessage("It is not possible to sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Given sale with items When cancelling item Then sale total is recalculated")]
    public void Sale_CancelItem_ShouldRecalculateTotal()
    {
        var sale = CreateSale();
        var firstItem = CreateItem(10, 100m);
        var secondItem = CreateItem(2, 50m);
        sale.AddItem(firstItem);
        sale.AddItem(secondItem);

        sale.CancelItem(firstItem.Id);

        firstItem.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(secondItem.TotalAmount);
    }

    [Fact(DisplayName = "Given sale with items When cancelling sale Then sale and items are cancelled")]
    public void Sale_Cancel_ShouldCancelSaleAndItems()
    {
        var sale = CreateSale();
        sale.AddItem(CreateItem(4, 100m));
        sale.AddItem(CreateItem(2, 50m));

        sale.Cancel();

        sale.IsCancelled.Should().BeTrue();
        sale.Items.Should().OnlyContain(item => item.IsCancelled);
        sale.TotalAmount.Should().Be(0m);
    }

    private static Sale CreateSale()
    {
        return new Sale(
            "SALE-0001",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "John Doe",
            Guid.NewGuid(),
            "Main Branch");
    }

    private static SaleItem CreateItem(int quantity, decimal unitPrice)
    {
        return new SaleItem(Guid.NewGuid(), "Keyboard", quantity, unitPrice);
    }
}
