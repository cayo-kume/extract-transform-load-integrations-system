using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxSangriaSuprimentosValidator : AbstractValidator<LinxSangriaSuprimentos>
    {
        public LinxSangriaSuprimentosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.usuario)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.usuario));

            RuleFor(x => x.data)
                .MaximumLength(1000)
                .WithMessage("O campo 'data' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.data));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.cancelado)
                .MaximumLength(1)
                .WithMessage("O campo 'cancelado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.conferido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.conferido));

            RuleFor(x => x.cod_historico)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_historico));

            RuleFor(x => x.desc_historico)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_historico' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_historico));

            RuleFor(x => x.id_sangria_suprimentos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_sangria_suprimentos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
