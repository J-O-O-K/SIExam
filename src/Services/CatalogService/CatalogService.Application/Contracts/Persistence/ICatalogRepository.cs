using CatalogService.Domain.Entities;

namespace CatalogService.Application.Contracts.Persistence;

public interface ICatalogRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByISBNAsync(string isbn);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    Task<bool> ExistsAsync(int id);
}