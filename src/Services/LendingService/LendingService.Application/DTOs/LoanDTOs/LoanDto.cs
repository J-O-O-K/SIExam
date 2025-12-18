namespace LendingService.Application.DTOs.LoanDTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsActive { get; set; }
    }
}
