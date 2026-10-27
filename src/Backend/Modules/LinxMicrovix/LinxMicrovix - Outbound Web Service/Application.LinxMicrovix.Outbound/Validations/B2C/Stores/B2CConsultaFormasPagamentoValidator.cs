using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaFormasPagamentoValidator : AbstractValidator<B2CConsultaFormasPagamento>
    {
        public B2CConsultaFormasPagamentoValidator()
        {
            RuleFor(x => x.cod_forma_pgto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_forma_pgto' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.forma_pgto)
                .MaximumLength(50)
                .WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
