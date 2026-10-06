using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaNFeValidator : AbstractValidator<B2CConsultaNFe>
    {
        public B2CConsultaNFeValidator()
        {
            RuleFor(x => x.id_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe));

            RuleFor(x => x.id_pedido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pedido));

            RuleFor(x => x.documento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.data_emissao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_emissao));

            RuleFor(x => x.chave_nfe)
                .MaximumLength(44)
                .WithMessage("O campo 'chave_nfe' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave_nfe));

            RuleFor(x => x.situacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.situacao));

            RuleFor(x => x.excluido)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.identificador_microvix)
                .Must(value => Guid.TryParse(value, out _))
                .WithMessage("O campo 'identificador_microvix' deve ser um GUID válido.")
                .When(x => !string.IsNullOrEmpty(x.identificador_microvix));

            RuleFor(x => x.dt_insert)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insert));

            RuleFor(x => x.valor_nota)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_nota));

            RuleFor(x => x.serie)
                .MaximumLength(10)
                .WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie));

            RuleFor(x => x.frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.frete));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.nProt)
                .MaximumLength(15)
                .WithMessage("O campo 'nProt' deve ter no máximo 15 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nProt));

            RuleFor(x => x.codigo_modelo_nf)
                .MaximumLength(3)
                .WithMessage("O campo 'codigo_modelo_nf' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_modelo_nf));

            RuleFor(x => x.justificativa)
                .MaximumLength(255)
                .WithMessage("O campo 'justificativa' deve ter no máximo 255 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.justificativa));

            RuleFor(x => x.tpAmb)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tpAmb));
        }
    }
}
