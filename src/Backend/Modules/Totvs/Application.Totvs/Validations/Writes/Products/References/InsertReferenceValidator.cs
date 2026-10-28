using Application.Totvs.Services;
using Application.Totvs.Validations.Writes.People.Customers;
using Application.Totvs.Validations.Writes.Products.BarCodes;
using Application.Totvs.Validations.Writes.Products.Classifications;
using Domain.Totvs.Dtos.Writes.Products.References;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.References
{
    /// <summary>
    /// valida o corpo da inclusão de referência no controller (corpo inválido volta 400): máscara, grade, um grupo por nível, empresas e SKUs
    /// com cor e tamanho sem repetição. Cada SKU leva a classificação 6000 com uma marca de TotvsProductBrands (senão a referência nunca entra
    /// no PostSearchReferences). Preços e custos (values) são recusados até a PR7 (PR11)
    /// </summary>
    public class InsertReferenceValidator : AbstractValidator<InsertReference>
    {
        public InsertReferenceValidator()
        {
            RuleFor(x => x.referenceSettingsCode)
                .NotNull()
                .WithMessage("O campo 'referenceSettingsCode' é obrigatório (máscara).")
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'referenceSettingsCode' deve ter de 1 a 6 dígitos.");

            RuleFor(x => x.referenceName)
                .MaximumLength(100)
                .WithMessage("O campo 'referenceName' aceita até 100 caracteres.");

            RuleFor(x => x.gridCode)
                .NotNull()
                .WithMessage("O campo 'gridCode' é obrigatório.")
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'gridCode' deve ter de 1 a 6 dígitos.");

            RuleFor(x => x.groups)
                .NotEmpty()
                .WithMessage("Informe um grupo por nível da máscara em 'groups'.")
                .Must(l => l!.Where(g => g?.levelNumber is not null).GroupBy(g => g.levelNumber).All(g => g.Count() == 1))
                .WithMessage("O mesmo nível ('levelNumber') aparece mais de uma vez em 'groups'.")
                .When(x => x.groups is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.groups)
                .ChildRules(g =>
                {
                    g.RuleFor(x => x.levelNumber)
                        .NotNull()
                        .WithMessage("O campo 'levelNumber' é obrigatório.")
                        .GreaterThan(0)
                        .WithMessage("O campo 'levelNumber' deve ser maior que zero.");

                    g.RuleFor(x => x.groupCode)
                        .NotEmpty()
                        .WithMessage("O campo 'groupCode' é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'groupCode' aceita até 10 caracteres.");

                    g.RuleFor(x => x.groupName)
                        .NotEmpty()
                        .WithMessage("O campo 'groupName' é obrigatório.")
                        .MaximumLength(40)
                        .WithMessage("O campo 'groupName' aceita até 40 caracteres.");
                })
                .When(x => x.groups is not null);

            RuleFor(x => x.branchs)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma empresa em 'branchs'.")
                .Must(l => l!.Where(b => b?.branchCode is not null).GroupBy(b => b.branchCode).All(g => g.Count() == 1))
                .WithMessage("A mesma loja aparece mais de uma vez em 'branchs'.")
                .When(x => x.branchs is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.branchs)
                .ChildRules(b =>
                {
                    b.RuleFor(x => x.branchCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCode' da empresa é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCode' da empresa deve ter de 1 a 4 dígitos.");

                    b.RuleFor(x => x.spedItemType).IsInEnum().WithMessage("O campo 'spedItemType' só aceita de 0 a 10 ou 99.");
                    b.RuleFor(x => x.inspectionType).IsInEnum().WithMessage("O campo 'inspectionType' só aceita 0, 1 ou 2.");
                    b.RuleFor(x => x.batchType).IsInEnum().WithMessage("O campo 'batchType' só aceita de 1 a 5.");
                })
                .When(x => x.branchs is not null);

            RuleFor(x => x.skus)
                .NotEmpty()
                .WithMessage("Informe pelo menos um SKU em 'skus'.");

            RuleFor(x => x.skus)
                .Must(l => l!.Where(s => s is not null).GroupBy(s => (s.colorCode?.Trim().ToUpperInvariant(), s.sizeName?.Trim().ToUpperInvariant())).All(g => g.Count() == 1))
                .WithMessage("A mesma cor e tamanho aparecem mais de uma vez em 'skus'.")
                .Must(l => BarCodeRules.Unique(l!.SelectMany(s => s?.barCodes ?? []).Select(b => b?.barCode)))
                .WithMessage("O mesmo código de barras aparece mais de uma vez no corpo.")
                .When(x => x.skus is { Count: > 0 });

            RuleForEach(x => x.skus)
                .SetValidator(new InsertProductValidator())
                .When(x => x.skus is not null);

            RuleFor(x => x.values)
                .Empty()
                .WithMessage("Preço e custo na inclusão de referência ('values') estão bloqueados até a liberação das escritas de preço (PR7, PR11).");

            RuleForEach(x => x.images)
                .SetValidator(new ProductImageInsertValidator())
                .When(x => x.images is not null);

            RuleFor(x => x.referenceWeb!)
                .SetValidator(new ReferenceWebValidator())
                .When(x => x.referenceWeb is not null);
        }
    }
}
