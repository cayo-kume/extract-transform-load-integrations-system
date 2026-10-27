using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script010_Create_LinxMicrovix_Product_Attributes_Tables
    /// </summary>
    public class LinxClassificacoesValidator : AbstractValidator<LinxClassificacoes>
    {
        public LinxClassificacoesValidator()
        {
            RuleFor(x => x.id_classificacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_classificacao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.desc_classificacao)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_classificacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_classificacao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.codigo_integracao_ws)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
