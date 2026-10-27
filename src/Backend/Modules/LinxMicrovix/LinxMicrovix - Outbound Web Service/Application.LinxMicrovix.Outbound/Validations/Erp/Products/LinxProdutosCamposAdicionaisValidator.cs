using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosCamposAdicionaisValidator : AbstractValidator<LinxProdutosCamposAdicionais>
    {
        public LinxProdutosCamposAdicionaisValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto));

            RuleFor(x => x.campo)
                .MaximumLength(50)
                .WithMessage("O campo 'campo' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.campo));

            RuleFor(x => x.valor)
                .MaximumLength(30)
                .WithMessage("O campo 'valor' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.id_campo_valor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_valor));
        }
    }
}
