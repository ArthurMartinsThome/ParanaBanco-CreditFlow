using CreditFlow.Domain.Enum;

namespace CreditFlow.Domain.Dto
{
    public class ClientDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Document { get; set; }
        public ECreditProposalStatus? Status { get; set; }
        public string? CardNumber { get; set; }
        public string? FailureReason { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public decimal? Limit { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}