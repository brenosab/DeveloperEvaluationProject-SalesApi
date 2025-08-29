using MediatR;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Filters;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;
public class GetSalesQueryHandler : IRequestHandler<GetSalesCommand, PagedResult<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetSaleResult>> Handle(GetSalesCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetSalesCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var filter = _mapper.Map<SaleFilter>(command);
        var pagedSales = await _saleRepository.GetPagedAsync(filter, cancellationToken);

        return _mapper.Map<PagedResult<GetSaleResult>>(pagedSales);
    }
}
