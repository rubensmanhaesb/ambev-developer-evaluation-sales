using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public class SaleItemInputValidator : AbstractValidator<SaleItemInput>
{
    public SaleItemInputValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("It is not possible to sell more than 20 identical items.");
        RuleFor(item => item.UnitPrice).GreaterThan(0);
    }
}
