using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    private readonly Faker<UpdateSaleCommand> _commandFaker;
    private readonly Faker<Sale> _saleFaker;
    private readonly Faker<UpdateSaleResult> _resultFaker;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventDispatcher);

        _commandFaker = new Faker<UpdateSaleCommand>()
            .RuleFor(c => c.Id, f => f.Random.Guid())
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .RuleFor(c => c.CustomerName, f => f.Person.FullName)
            .RuleFor(c => c.SaleDate, f => f.Date.Past(1))
            .RuleFor(c => c.Branch, f => f.Company.CompanyName())
            .RuleFor(c => c.Items, f => new List<UpdateSaleItemCommand>
            {
                new Faker<UpdateSaleItemCommand>()
                    .RuleFor(i => i.Id, f2 => f2.Random.Guid())
                    .RuleFor(i => i.ProductId, f2 => f2.Random.Guid())
                    .RuleFor(i => i.Title, f2 => f2.Commerce.ProductName())
                    .RuleFor(i => i.Price, f2 => f2.Random.Decimal(1, 1000))
                    .RuleFor(i => i.Description, f2 => f2.Commerce.ProductDescription())
                    .RuleFor(i => i.Category, f2 => f2.Commerce.Categories(1)[0])
                    .RuleFor(i => i.Image, f2 => f2.Image.PicsumUrl())
                    .RuleFor(i => i.RatingRate, f2 => f2.Random.Decimal(0, 5))
                    .RuleFor(i => i.RatingCount, f2 => f2.Random.Int(0, 1000))
                    .RuleFor(i => i.Quantity, f2 => f2.Random.Int(1, 10))
                    .RuleFor(i => i.UnitPrice, f2 => f2.Random.Decimal(1, 1000))
                    .Generate()
            });

        _saleFaker = new Faker<Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.Branch, f => f.Company.CompanyName())
            .RuleFor(s => s.Items, f => new List<SaleItem>())
            .RuleFor(s => s.TotalAmount, 1000);

        _resultFaker = new Faker<UpdateSaleResult>()
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.CustomerName, f => f.Person.FullName)
            .RuleFor(r => r.Branch, f => f.Company.CompanyName())
            .RuleFor(r => r.Items, f => new List<UpdateSaleItemResult>());
    }


    /// <summary>
    /// Tests that the handler updates the sale in the repository.
    /// </summary>
    [Fact(DisplayName = "Given valid update command When handling Then updates sale in repository")]
    public async Task Handle_ValidCommand_UpdatesSaleInRepository()
    {
        // Given
        var command = _commandFaker.Generate();
        var originalSale = _saleFaker.Generate();
        originalSale.Id = command.Id;
        var updatedSale = _saleFaker.Generate();

        _mapper.Map<Sale>(command).Returns(updatedSale);
        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>()).Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(originalSale);

        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var _ = await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the handler maps the updated sale to the expected result.
    /// </summary>
    [Fact(DisplayName = "Given valid update command When handling Then maps sale to result")]
    public async Task Handle_ValidCommand_MapsSaleToResult()
    {
        // Given
        var command = _commandFaker.Generate();
        var updatedSale = _saleFaker.Generate();
        var result = _resultFaker.Generate();

        _mapper.Map<Sale>(command).Returns(updatedSale);
        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);
        _mapper.Received(1).Map<UpdateSaleResult>(updatedSale);
    }

    /// <summary>
    /// Tests that a valid update command publishes a SaleModifiedEvent.
    /// </summary>
    [Fact(DisplayName = "Given valid update command When handling Then publishes SaleModifiedEvent")]
    public async Task Handle_ValidCommand_PublishesSaleModifiedEvent()
    {
        // Given
        var command = _commandFaker.Generate();
        // Original sale retrieved from repository
        var originalSale = _saleFaker.Generate();
        originalSale.Id = command.Id;

        // Updated sale to be persisted
        var updatedSale = _saleFaker.Generate();
        updatedSale.Id = command.Id;

        _mapper.Map<Sale>(command).Returns(updatedSale);
        // Configure repository
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(originalSale, updatedSale); // first call: original, second call: updated

        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>())
            .Returns(true);

        // Map to result
        var result = _resultFaker.Generate();
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // Event dispatcher
        _eventDispatcher
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);

        await _eventDispatcher.Received(1)
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());

        await _saleRepository.Received(2).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>());
    }
    /// <summary>
    /// Tests that a valid update command publishes a SaleCancelledEvent when the sale is cancelled.
    /// </summary>
    [Fact(DisplayName = "Given update command that cancels sale When handling Then publishes SaleCancelledEvent")]
    public async Task Handle_ValidCommand_PublishesSaleCancelledEvent()
    {
        // Given
        var command = _commandFaker.Generate();

        // Original sale not cancelled
        var originalSale = _saleFaker.Generate();
        originalSale.Id = command.Id;
        originalSale.Cancelled = false;

        // Updated sale cancelled
        var updatedSale = _saleFaker.Generate();
        updatedSale.Id = command.Id;
        updatedSale.Cancelled = true;

        _mapper.Map<Sale>(command).Returns(updatedSale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(originalSale, updatedSale);

        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = _resultFaker.Generate();
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        _eventDispatcher
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);

        await _eventDispatcher.Received(1)
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a valid update command publishes ItemCancelledEvent for items that are cancelled.
    /// </summary>
    [Fact(DisplayName = "Given update command that cancels items When handling Then publishes ItemCancelledEvent")]
    public async Task Handle_ValidCommand_PublishesItemCancelledEvent()
    {
        // Given
        var command = _commandFaker.Generate();

        // Original sale with items not cancelled
        var originalSale = _saleFaker.Generate();
        originalSale.Id = command.Id;
        originalSale.Items =
        [
            new SaleItem { Id = Guid.NewGuid(), Cancelled = false },
            new SaleItem { Id = Guid.NewGuid(), Cancelled = false }
        ];

        // Updated sale with some items cancelled
        var updatedSale = _saleFaker.Generate();
        updatedSale.Id = command.Id;
        updatedSale.Items = originalSale.Items.Select(i => new SaleItem
        {
            Id = i.Id,
            Cancelled = true // simulate items being cancelled
        }).ToList();

        _mapper.Map<Sale>(command).Returns(updatedSale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(originalSale, updatedSale);

        _saleRepository.UpdateAsync(command.Id, updatedSale, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = _resultFaker.Generate();
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        _eventDispatcher
            .PublishAsync(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);

        await _eventDispatcher.Received(originalSale.Items.Count)
            .PublishAsync(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }
}
