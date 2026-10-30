using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ObservationNFValidator : AbstractValidator<ObservationNF>
    {
        public ObservationNFValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.observation)
                .MaximumLength(100)
                .WithMessage("O campo 'observation' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}
