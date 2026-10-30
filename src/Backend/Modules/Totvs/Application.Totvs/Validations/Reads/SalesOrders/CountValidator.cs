using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class CountValidator : AbstractValidator<Count>
    {
        public CountValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.statusCount)
                .MaximumLength(20)
                .WithMessage("O campo 'statusCount' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.statusCount));

            RuleFor(x => x.packagingTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.packagingTypeCode));

            RuleFor(x => x.packagingTypeDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'packagingTypeDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.packagingTypeDescription));

            RuleFor(x => x.separator)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.separator));

            RuleFor(x => x.separatorName)
                .MaximumLength(60)
                .WithMessage("O campo 'separatorName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.separatorName));
        }
    }
}
