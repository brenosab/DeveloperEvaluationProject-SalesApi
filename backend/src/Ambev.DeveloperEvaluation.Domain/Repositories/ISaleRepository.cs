using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Filters;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository : IBaseRepository<Sale>
{
    /// <summary>
    /// Returns a paginated list of sales
    /// </summary>
    /// <param name="filter">The paged filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged result of sales</returns>
    Task<PagedResult<Sale>> GetPagedAsync(SaleFilter filter, CancellationToken cancellationToken = default);
}
