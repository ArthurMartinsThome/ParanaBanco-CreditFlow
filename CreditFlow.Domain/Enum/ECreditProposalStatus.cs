using System.ComponentModel;

namespace CreditFlow.Domain.Enum
{
    public enum ECreditProposalStatus
    {
        [Description("EM_AVALIACAO")]
        EmAvaliacao = 1,
        [Description("REPROVADO")]
        Reprovado = 2,
        [Description("APROVADO")]
        Aprovado = 3,
        [Description("FALHA_EMISSAO")]
        FalhaEmissao = 4
    }
}