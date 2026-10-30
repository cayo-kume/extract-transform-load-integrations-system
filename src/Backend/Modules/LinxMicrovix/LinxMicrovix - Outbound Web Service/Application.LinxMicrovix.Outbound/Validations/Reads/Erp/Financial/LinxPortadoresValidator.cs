using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxPortadoresValidator : AbstractValidator<LinxPortadores>
    {
        public LinxPortadoresValidator()
        {
            RuleFor(x => x.cod_portador)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_portador' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_portador)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_portador' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_portador));

            RuleFor(x => x.taxa_acrescimo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxa_acrescimo));

            RuleFor(x => x.aviso_vencimento)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.aviso_vencimento));

            RuleFor(x => x.reaviso_vencimento)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.reaviso_vencimento));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
