using CreditFlow.Domain.Enum;

namespace CreditFlow.Integration.Document.Model
{
    internal class Client
    {
        public Guid? id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string? document { get; set; }
        public ECreditProposalStatus? status { get; set; }
        public string? failure_reason { get; set; }
        public string? card_number { get; set; }
        public decimal? monthly_income { get; set; } = 0.0m;
        public decimal? limit { get; set; } = 0.0m;
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}