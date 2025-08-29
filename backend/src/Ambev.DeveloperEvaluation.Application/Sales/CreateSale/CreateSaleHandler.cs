using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSaleCommand> _validator;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IEventDispatcher eventDispatcher,
        IMapper mapper,
        IValidator<CreateSaleCommand> validator)
    {
        _saleRepository = saleRepository;
        _eventDispatcher = eventDispatcher;
        _mapper = mapper;
        _validator = validator;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The create sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the created sale</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        //Validate the command
        await ValidateCommandAsync(command, cancellationToken);

        //Build the Sale entity and apply business rules
        var sale = BuildSaleFromCommand(command);

        //Persist the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        //Dispatch domain events
        await DispatchEventsAsync(createdSale, cancellationToken);

        // Map to return type
        return _mapper.Map<CreateSaleResult>(createdSale);
    }

    /// <summary>
    /// Validates the CreateSaleCommand
    /// </summary>
    /// <param name="command">The create sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task ValidateCommandAsync(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    /// <summary>
    /// Builds the Sale entity from the command and applies business rules
    /// </summary>
    /// <param name="command">The create sale command</param>
    /// <returns>The Sale entity with business rules applied</returns>
    private Sale BuildSaleFromCommand(CreateSaleCommand command)
    {
        var sale = _mapper.Map<Sale>(command);

        // Apply business rules inside the entity
        sale.ApplyBusinessRules();

        return sale;
    }

    /// <summary>
    /// Dispatches domain events related to the created sale
    /// </summary>
    /// <param name="sale">The created Sale entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task DispatchEventsAsync(Sale sale, CancellationToken cancellationToken)
    {
        var saleCreatedEvent = new SaleCreatedEvent(sale.Id, DateTime.UtcNow);
        await _eventDispatcher.PublishAsync(saleCreatedEvent, cancellationToken);
    }
}
