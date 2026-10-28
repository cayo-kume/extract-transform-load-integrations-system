using Application.Totvs.Services;
using Application.Totvs.Validations.Writes.People.Customers;
using Application.Totvs.Validations.Writes.Products.BarCodes;
using Application.Totvs.Validations.Writes.Products.Classifications;
using Domain.Totvs.Dtos.Writes.Products.References;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.References
{
    /// <summary>
    /// SKU da referência: nome, cor, tamanho e unidade; a classificação 6000 com uma das marcas; um fornecedor e um código padrão por SKU
    /// </summary>
    public class InsertProductValidator : AbstractValidator<InsertProduct>
    {
        public InsertProductValidator()
        {
            var brands = TotvsProductBrands.BaseBranchCodes.Values.ToList();

            RuleFor(x => x.productCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos (ou ficar fora: a Totvs gera).")
                .When(x => x.productCode is not null);

            RuleFor(x => x.productName)
                .NotEmpty()
                .WithMessage("O campo 'productName' é obrigatório.")
                .MaximumLength(60)
                .WithMessage("O campo 'productName' aceita até 60 caracteres.");

            RuleFor(x => x.colorCode)
                .NotEmpty()
                .WithMessage("O campo 'colorCode' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'colorCode' aceita até 10 caracteres.");

            RuleFor(x => x.sizeName)
                .NotEmpty()
                .WithMessage("O campo 'sizeName' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'sizeName' aceita até 10 caracteres.");

            RuleFor(x => x.measuredUnit)
                .NotEmpty()
                .WithMessage("O campo 'measuredUnit' é obrigatório.")
                .MaximumLength(3)
                .WithMessage("O campo 'measuredUnit' aceita até 3 caracteres.");

            RuleFor(x => x.ncmCode).MaximumLength(20).WithMessage("O campo 'ncmCode' aceita até 20 caracteres.");
            RuleFor(x => x.prefixEanGtin).MaximumLength(20).WithMessage("O campo 'prefixEanGtin' aceita até 20 caracteres.");
            RuleFor(x => x.cestCode).MaximumLength(20).WithMessage("O campo 'cestCode' aceita até 20 caracteres.");
            RuleFor(x => x.cstCode).IsInEnum().WithMessage("O campo 'cstCode' só aceita de 0 a 8.");

            RuleFor(x => x.weight)
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'weight' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'weight' aceita até 3 casas decimais.")
                .When(x => x.weight is not null);

            RuleFor(x => x.conservationInstructionCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'conservationInstructionCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.conservationInstructionCode is not null);

            RuleFor(x => x.classifications)
                .Must(l => l is not null && l.Any(c => c?.typeCode == TotvsProductBrands.ClassificationType && brands.Contains(c.code ?? "")))
                .WithMessage($"Cada SKU leva a classificação {TotvsProductBrands.ClassificationType} (marca) com {String.Join(" ou ", brands)}: sem ela a referência não entra na integração.")
                .Must(l => l!.Where(c => c?.typeCode is not null).GroupBy(c => c.typeCode).All(g => g.Count() == 1))
                .WithMessage("O mesmo tipo de classificação aparece mais de uma vez no SKU.")
                .When(x => x.classifications is not null, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.classifications)
                .SetValidator(new ProductClassificationItemValidator(allowBrand: true))
                .When(x => x.classifications is not null);

            RuleForEach(x => x.additionalFields)
                .ChildRules(f =>
                {
                    f.RuleFor(x => x.code)
                        .NotNull()
                        .WithMessage("O campo 'code' do campo adicional é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'code' do campo adicional deve ter de 1 a 4 dígitos.");

                    f.RuleFor(x => x.value)
                        .NotNull()
                        .WithMessage("O campo 'value' do campo adicional é obrigatório.")
                        .Must(CustomerRules.IsAdditionalValue)
                        .WithMessage("O campo 'value' aceita texto até 80 caracteres, número com até 12 inteiros e 6 decimais ou data DD/MM/YYYY.");
                })
                .When(x => x.additionalFields is not null);

            RuleFor(x => x.suppliers)
                .Must(l => l!.Count(s => s?.isMainSupplier == true) <= 1)
                .WithMessage("No máximo um fornecedor padrão ('isMainSupplier') por SKU.")
                .When(x => x.suppliers is not null);

            RuleForEach(x => x.suppliers)
                .ChildRules(s =>
                {
                    s.RuleFor(x => x.supplierCpfCnpj)
                        .NotEmpty()
                        .WithMessage("O campo 'supplierCpfCnpj' é obrigatório.")
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'supplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.");

                    s.RuleFor(x => x.originalCode).MaximumLength(60).WithMessage("O campo 'originalCode' aceita até 60 caracteres.");
                })
                .When(x => x.suppliers is not null);

            RuleForEach(x => x.manufacturers)
                .ChildRules(m =>
                {
                    m.RuleFor(x => x.manufacturerCpfCnpj)
                        .NotEmpty()
                        .WithMessage("O campo 'manufacturerCpfCnpj' é obrigatório.")
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'manufacturerCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.");

                    m.RuleFor(x => x.originalCode).MaximumLength(60).WithMessage("O campo 'originalCode' aceita até 60 caracteres.");
                })
                .When(x => x.manufacturers is not null);

            RuleFor(x => x.barCodes)
                .Must(l => l!.Count(b => b?.isMainCode == true) <= 1)
                .WithMessage("No máximo um código padrão ('isMainCode') por SKU.")
                .When(x => x.barCodes is not null);

            RuleForEach(x => x.barCodes)
                .SetValidator(new BarCodeInsertValidator())
                .When(x => x.barCodes is not null);

            RuleForEach(x => x.locations)
                .ChildRules(l =>
                {
                    l.RuleFor(x => x.branchCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCode' da localização é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCode' da localização deve ter de 1 a 4 dígitos.");

                    l.RuleFor(x => x.locationCode)
                        .NotEmpty()
                        .WithMessage("O campo 'locationCode' é obrigatório.")
                        .MaximumLength(15)
                        .WithMessage("O campo 'locationCode' aceita até 15 caracteres.");
                })
                .When(x => x.locations is not null);

            RuleFor(x => x.values)
                .Empty()
                .WithMessage("Preço e custo no SKU ('values') estão bloqueados até a liberação das escritas de preço (PR7, PR11).");

            RuleForEach(x => x.images)
                .SetValidator(new ProductImageInsertValidator())
                .When(x => x.images is not null);
        }
    }
}
