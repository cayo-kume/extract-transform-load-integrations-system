using Domain.Totvs.Dtos.Writes.Products.Data;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Data
{
    /// <summary>
    /// filtro de produtos das escritas por lista: exatamente uma lista, não vazia, até 500 itens e sem repetição; groupCode e referenceCode
    /// (um grupo inteiro) ficam fora desta versão (PR9)
    /// </summary>
    public class ProductDataFilterValidator : AbstractValidator<ProductDataFilter>
    {
        public ProductDataFilterValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.productCodeList is { Count: > 0 }) != (x.barCodeList is { Count: > 0 }))
                .WithName("filter.productCodeList")
                .WithMessage("Informe exatamente uma lista, não vazia, entre 'filter.productCodeList' e 'filter.barCodeList'.");

            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.groupCode) && String.IsNullOrEmpty(x.referenceCode))
                .WithName("filter.groupCode")
                .WithMessage("'filter.groupCode' e 'filter.referenceCode' alcançam um grupo inteiro e não são aceitos nesta versão (PR9): use as listas.");

            RuleFor(x => x.productCodeList)
                .Must(l => l!.Count <= 500)
                .WithMessage("Até 500 produtos em 'filter.productCodeList'.")
                .Must(l => l!.All(c => c is >= 1 and <= 999_999_999) && l!.Distinct().Count() == l!.Count)
                .WithMessage("Os códigos de 'filter.productCodeList' têm de 1 a 9 dígitos, sem repetição.")
                .When(x => x.productCodeList is { Count: > 0 });

            RuleFor(x => x.barCodeList)
                .Must(l => l!.Count <= 500)
                .WithMessage("Até 500 códigos em 'filter.barCodeList'.")
                .Must(l => l!.All(c => !String.IsNullOrEmpty(c) && c.Length <= 40) && l!.Distinct().Count() == l!.Count)
                .WithMessage("Os códigos de 'filter.barCodeList' têm até 40 caracteres, sem repetição.")
                .When(x => x.barCodeList is { Count: > 0 });
        }
    }
}
