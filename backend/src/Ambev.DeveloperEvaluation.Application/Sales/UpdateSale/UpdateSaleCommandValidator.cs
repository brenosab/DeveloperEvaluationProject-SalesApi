using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.SaleDate).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Branch).NotEmpty();
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.")
            .ForEach(itemRule => itemRule.SetValidator(new UpdateSaleItemCommandValidator()));
    }
}

public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Image).NotEmpty();
        RuleFor(x => x.RatingRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RatingCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity must be between 1 and 20.");
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}
