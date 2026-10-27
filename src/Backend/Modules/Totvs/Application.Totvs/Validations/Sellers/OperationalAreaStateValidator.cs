using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Sellers
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Seller não dá tamanho dos textos (colunas do
    /// Script042_Create_Totvs_Operational_Areas_Tables)
    /// </summary>
    public class OperationalAreaStateValidator : AbstractValidator<OperationalAreaState>
    {
        public OperationalAreaStateValidator()
        {
            RuleFor(x => x.operationalAreaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'operationalAreaCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.stateCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'stateCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.stateName)
                .MaximumLength(60)
                .WithMessage("O campo 'stateName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateName));
        }
    }
}
