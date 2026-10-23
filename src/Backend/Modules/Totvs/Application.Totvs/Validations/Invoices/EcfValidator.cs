using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script025_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class EcfValidator : AbstractValidator<Ecf>
    {
        public EcfValidator()
        {
            RuleFor(x => x.coupon)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.coupon));

            RuleFor(x => x.serialMachine)
                .MaximumLength(60)
                .WithMessage("O campo 'serialMachine' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialMachine));
        }
    }
}
