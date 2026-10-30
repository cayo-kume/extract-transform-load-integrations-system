using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaTiposCobrancaFreteValidator : AbstractValidator<B2CConsultaTiposCobrancaFrete>
    {
        public B2CConsultaTiposCobrancaFreteValidator()
        {
            RuleFor(x => x.codigo_tipo_cobranca_frete)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_tipo_cobranca_frete' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_tipo_cobranca_frete)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_tipo_cobranca_frete' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_tipo_cobranca_frete));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
