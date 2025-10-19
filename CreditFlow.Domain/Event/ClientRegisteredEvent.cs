using CreditFlow.Domain.Enum;

namespace CreditFlow.Domain.Event
{
    public record ClientRegisteredEvent(
        Guid? ClientId, 
        string? Name,
        string? Email,
        string? PhoneNumber,
        string? Document, 
        decimal? MonthlyIncome,
        ECreditProposalStatus? Status,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
        ) : BaseEvent;
}