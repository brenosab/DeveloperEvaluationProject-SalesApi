using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
{
    private readonly ISaleRepository _saleRepository;

    public DeleteSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Unit> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var deleted = await _saleRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new InvalidOperationException($"Sale with id {command.Id} not found");

        return Unit.Value;
    }
}
