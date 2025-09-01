using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The update sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the updated sale</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        // 1. Validate the command
        await ValidateCommandAsync(command, cancellationToken);

        // 2. Get the original sale for comparison
        var originalSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken, s => s.Items);
        if (originalSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // 3. Build the entity and apply business rules
        var sale = BuildSaleFromCommand(command);

        // 4. Persist the update
        var success = await _saleRepository.UpdateAsync(command.Id, sale, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // 5. Get the updated entity
        var updatedSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken, s => s.Items);

        // 6. Publish domain events
        await DispatchEventsAsync(originalSale, updatedSale, cancellationToken);

        // 7. Map to result type
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }

    /// <summary>
    /// Validates the UpdateSaleCommand
    /// </summary>
    /// <param name="command">The update sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private static async Task ValidateCommandAsync(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    /// <summary>
    /// Builds the Sale entity from the command and applies business rules
    /// </summary>
    /// <param name="command">The update sale command</param>
    /// <returns>The Sale entity with business rules applied</returns>
    private Sale BuildSaleFromCommand(UpdateSaleCommand command)
    {
        var sale = _mapper.Map<Sale>(command);

        // Apply business rules inside the entity
        sale.ApplyBusinessRules();
        sale.TotalAmount = sale.Items.Sum(i => i.Total);
        return sale;
    }

    /// <summary>
    /// Dispatches domain events related to the updated sale, including cancellation and item cancellation events
    /// </summary>
    /// <param name="original">The original Sale entity before update</param>
    /// <param name="updated">The updated Sale entity after update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task DispatchEventsAsync(Sale? original, Sale? updated, CancellationToken cancellationToken)
    {
        if (updated == null) return;

        // Publish general modification event
        await _eventDispatcher.PublishAsync(new SaleModifiedEvent(updated.Id, DateTime.UtcNow), cancellationToken);

        // Publish sale cancellation event
        if (original != null && !original.Cancelled && updated.Cancelled)
        {
            await _eventDispatcher.PublishAsync(new SaleCancelledEvent(updated.Id, DateTime.UtcNow), cancellationToken);
        }

        // Publish item cancellation events
        if (original != null)
        {
            var originalItems = original.Items.ToDictionary(i => i.Id);
            foreach (var item in updated.Items)
            {
                if (originalItems.TryGetValue(item.Id, out var origItem))
                {
                    if (!origItem.Cancelled && item.Cancelled)
                    {
                        await _eventDispatcher.PublishAsync(new ItemCancelledEvent(updated.Id, item.Id, DateTime.UtcNow), cancellationToken);
                    }
                }
            }
        }
    }
}
