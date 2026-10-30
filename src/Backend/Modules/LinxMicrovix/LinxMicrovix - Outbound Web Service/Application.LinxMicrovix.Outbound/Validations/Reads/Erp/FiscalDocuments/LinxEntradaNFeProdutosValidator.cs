using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxEntradaNFeProdutosValidator : AbstractValidator<LinxEntradaNFeProdutos>
    {
        public LinxEntradaNFeProdutosValidator()
        {
            RuleFor(x => x.id_entrada_nfe_produtos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_entrada_nfe_produtos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_entrada_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_entrada_nfe));

            RuleFor(x => x.numero_item)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_item));

            RuleFor(x => x.codigo_produto)
                .MaximumLength(60)
                .WithMessage("O campo 'codigo_produto' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_produto));

            RuleFor(x => x.descricao_produto)
                .MaximumLength(250)
                .WithMessage("O campo 'descricao_produto' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_produto));

            RuleFor(x => x.gtin)
                .MaximumLength(25)
                .WithMessage("O campo 'gtin' deve ter no máximo 25 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.gtin));

            RuleFor(x => x.qtde_tributavel)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_tributavel));

            RuleFor(x => x.qtde_comercial)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_comercial));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
