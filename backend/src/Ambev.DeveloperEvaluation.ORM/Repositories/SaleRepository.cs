using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Filters;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAsync(Guid id, Sale sale, CancellationToken cancellationToken = default)
    {
        var recordedSale = await GetByIdAsync(id, cancellationToken);
        if (recordedSale == null)
            return false;

        recordedSale.SaleNumber = sale.SaleNumber;
        recordedSale.SaleDate = sale.SaleDate;
        recordedSale.CustomerId = sale.CustomerId;
        recordedSale.CustomerName = sale.CustomerName;
        recordedSale.Branch = sale.Branch;
        recordedSale.TotalAmount = sale.TotalAmount;
        recordedSale.Cancelled = sale.Cancelled;

        recordedSale.Items = sale.Items;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Common.Pagination.PagedResult<Sale>> GetPagedAsync(SaleFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = BuildQuery(filter);
        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(filter.OrderBy))
        {
            query = query.OrderBy(filter.OrderBy);
        }
        else
        {
            query = query.OrderByDescending(s => s.SaleDate);
        }

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new Common.Pagination.PagedResult<Sale>(items, totalCount, filter.Page, filter.PageSize);
    }

    /// <summary>
    /// Build the base query with the filters applied.
    /// </summary>
    private IQueryable<Sale> BuildQuery(SaleFilter filter)
    {
        IQueryable<Sale> query = _context.Sales
            .Include(s => s.Items)
            .AsQueryable();
        
        if (filter.MinSaleDate.HasValue)
            query = query.Where(s => s.SaleDate >= filter.MinSaleDate.Value);

        if (filter.MaxSaleDate.HasValue)
            query = query.Where(s => s.SaleDate <= filter.MaxSaleDate.Value);

        if (!string.IsNullOrWhiteSpace(filter.SaleNumber))
            query = query.Where(s => s.SaleNumber.Contains(filter.SaleNumber));

        if (!string.IsNullOrWhiteSpace(filter.CustomerName))
            query = query.Where(s => s.CustomerName.Contains(filter.CustomerName));
        
        if (!string.IsNullOrWhiteSpace(filter.Branch))
            query = query.Where(s => s.Branch.Contains(filter.Branch));

        if (!string.IsNullOrWhiteSpace(filter.ItemDescription))
            query = query.Where(s => s.Items.Any(i => i.Description.Contains(filter.ItemDescription)));

        if (!string.IsNullOrWhiteSpace(filter.ItemCategory))
            query = query.Where(s => s.Items.Any(i => i.Category == filter.ItemCategory));
        
        return query;
    }
}
