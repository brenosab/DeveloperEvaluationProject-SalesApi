using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    private readonly Faker<CreateSaleCommand> _commandFaker;
    private readonly Faker<Sale> _saleFaker;
    private readonly Faker<CreateSaleResult> _resultFaker;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _eventDispatcher, _mapper);

        _commandFaker = new Faker<CreateSaleCommand>()
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .RuleFor(c => c.CustomerName, f => f.Person.FullName)
            .RuleFor(c => c.SaleDate, f => f.Date.Past(1))
            .RuleFor(c => c.Branch, f => f.Company.CompanyName())
            .RuleFor(c => c.Items, f => new List<CreateSaleItemCommand>
            {
                new Faker<CreateSaleItemCommand>()
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

        _resultFaker = new Faker<CreateSaleResult>()
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.CustomerName, f => f.Person.FullName)
            .RuleFor(r => r.Branch, f => f.Company.CompanyName())
            .RuleFor(r => r.Items, f => new List<CreateSaleItemResult>());
    }

    /// <summary>
    /// Tests that a valid sale command persists the sale in the repository.
    /// </summary>
    [Fact(DisplayName = "Given valid sale command When handling Then persists sale in repository")]
    public async Task Handle_ValidCommand_PersistsSaleInRepository()
    {
        // Given
        var command = _commandFaker.Generate();
        var sale = _saleFaker.Generate();
        var result = _resultFaker.Generate();

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);

        await _saleRepository.Received(1).CreateAsync(sale, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a valid sale command publishes a SaleCreatedEvent.
    /// </summary>
    [Fact(DisplayName = "Given valid sale command When handling Then publishes SaleCreatedEvent")]
    public async Task Handle_ValidCommand_PublishesSaleCreatedEvent()
    {
        // Given
        var command = _commandFaker.Generate();
        var sale = _saleFaker.Generate();
        var result = _resultFaker.Generate();

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>())
            .Returns(sale);

        _eventDispatcher
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(result);

        await _eventDispatcher.Received(1)
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}