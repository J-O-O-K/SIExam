using LendingService.Domain.Entities;

namespace LendingService.Application.Contracts.Persistence;

public interface ILendingRepository
{
    Task<Loan?> GetByIdAsync(int id);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
    Task<Loan?> GetActiveLoanByBookIdAsync(int bookId);
    Task<Loan> CreateAsync(Loan loan);
    Task<Loan> UpdateAsync(Loan loan);
    Task<bool> ExistsAsync(int id);
}