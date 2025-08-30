using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Filters;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSalesQueryHandler"/> class.
/// </summary>
public class GetSalesQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesQueryHandler _handler;

    private readonly Faker<GetSalesCommand> _commandFaker;
    private readonly Faker<Sale> _saleFaker;

    public GetSalesQueryHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesQueryHandler(_saleRepository, _mapper);

        _commandFaker = new Faker<GetSalesCommand>()
            .RuleFor(q => q.Page, f => f.Random.Int(1, 5))
            .RuleFor(q => q.PageSize, f => f.Random.Int(1, 20));

        _saleFaker = new Faker<Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.Branch, f => f.Company.CompanyName())
            .RuleFor(s => s.Items, f => new List<SaleItem>())
            .RuleFor(s => s.TotalAmount, 1000);
    }

    /// <summary>
    /// Tests that the handler calls the repository to retrieve a paged result of sales.
    /// </summary>
    [Fact(DisplayName = "Given valid query When handling Then calls repository with expected parameters")]
    public async Task Handle_ValidQuery_CallsRepository()
    {
        // Given
        var command = _commandFaker.Generate();
        var sales = _saleFaker.Generate(3);
        var pagedResult = new PagedResult<Sale>(
                Data: sales,
                TotalItems: 3,
                CurrentPage: 1,
                TotalPages: 1
        );
        var filter = new SaleFilter
        {
            Page = command.Page,
            PageSize = command.PageSize,
            Branch = command.Branch,
            CustomerName = command.CustomerName,
            ItemCategory = command.ItemCategory,
            ItemDescription = command.ItemDescription,
            MaxSaleDate = command.MaxSaleDate,
            MinSaleDate = command.MinSaleDate,
            OrderBy = command.OrderBy,
            SaleNumber = command.SaleNumber
        };
        _mapper.Map<SaleFilter>(command).Returns(filter);

        _saleRepository.GetPagedAsync(filter, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // When
        var _ = await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1)
            .GetPagedAsync(filter, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the handler maps the paged result of sales to GetSaleResult correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid query When handling Then maps repository result to response")]
    public async Task Handle_ValidQuery_MapsRepositoryResultToResponse()
    {
        // Given
        var command = _commandFaker.Generate();
        var sales = _saleFaker.Generate(3);
        var pagedResult = new PagedResult<Sale>(
            Data: sales,
            TotalItems: 3,
            CurrentPage: 1,
            TotalPages: 1
        );
        var mappedResult = new PagedResult<GetSaleResult>(
            Data: new List<GetSaleResult>(),
            TotalItems: 3,
            CurrentPage: 1,
            TotalPages: 1
        );

        var filter = new SaleFilter
        {
            Page = command.Page,
            PageSize = command.PageSize,
            Branch = command.Branch,
            CustomerName = command.CustomerName,
            ItemCategory = command.ItemCategory,
            ItemDescription = command.ItemDescription,
            MaxSaleDate = command.MaxSaleDate,
            MinSaleDate = command.MinSaleDate,
            OrderBy = command.OrderBy,
            SaleNumber = command.SaleNumber
        };
        _mapper.Map<SaleFilter>(command).Returns(filter);
        _saleRepository.GetPagedAsync(filter, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        _mapper.Map<PagedResult<GetSaleResult>>(pagedResult)
            .Returns(mappedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedResult);

        _mapper.Received(1).Map<PagedResult<GetSaleResult>>(pagedResult);
    }
}
