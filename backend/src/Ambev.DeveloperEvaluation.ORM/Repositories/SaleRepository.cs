using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
        return await _context.Sales.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
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
}
