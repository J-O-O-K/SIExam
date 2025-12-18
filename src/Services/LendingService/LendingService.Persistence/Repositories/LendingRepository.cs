using LendingService.Application.Contracts.Persistence;
using LendingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendingService.Persistence.Repositories;

public class LendingRepository : ILendingRepository
{
    private readonly LendingDbContext _context;

    public LendingRepository(LendingDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _context.Loans.FindAsync(id);
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await _context.Loans.ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
    {
        return await _context.Loans.Where(l => l.UserId == userId).ToListAsync();
    }

    public async Task<Loan?> GetActiveLoanByBookIdAsync(int bookId)
    {
        return await _context.Loans.FirstOrDefaultAsync(l => l.BookId == bookId && l.IsActive);
    }

    public async Task<Loan> CreateAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan> UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Loans.AnyAsync(l => l.Id == id);
    }
}