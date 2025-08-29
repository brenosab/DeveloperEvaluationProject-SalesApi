using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = _mapper.Map<Sale>(command);
        foreach (var item in sale.Items)
        {
            item.ApplyBusinessRules();
        }
        sale.TotalAmount = sale.Items.Sum(i => i.Total);

        var success = await _saleRepository.UpdateAsync(command.Id, sale, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        var updatedSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }
}
