using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxLotesProdutosValidator : AbstractValidator<LinxLotesProdutos>
    {
        public LinxLotesProdutosValidator()
        {
            RuleFor(x => x.id_lote)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_lote' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_produto));

            RuleFor(x => x.lote)
                .MaximumLength(60)
                .WithMessage("O campo 'lote' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lote));

            RuleFor(x => x.identificador)
                .MaximumLength(1000)
                .WithMessage("O campo 'identificador' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.transacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao));

            RuleFor(x => x.data_fabricacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_fabricacao));

            RuleFor(x => x.data_vencimento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_vencimento));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
