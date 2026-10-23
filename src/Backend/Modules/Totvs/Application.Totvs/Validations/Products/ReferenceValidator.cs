using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script010_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class ReferenceValidator : AbstractValidator<Reference>
    {
        public ReferenceValidator()
        {
            RuleFor(x => x.referenceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'referenceId' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.ReferenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'ReferenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ReferenceCode));

            RuleFor(x => x.lastReferenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'lastReferenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lastReferenceCode));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.gridCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.gridCode));

            RuleFor(x => x.weight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.weight));

            RuleFor(x => x.height)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.height));

            RuleFor(x => x.width)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.width));

            RuleFor(x => x.length)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.length));

            RuleFor(x => x.packWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packWeight));

            RuleFor(x => x.packHeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packHeight));

            RuleFor(x => x.packWidth)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packWidth));

            RuleFor(x => x.packLength)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packLength));

            RuleForEach(x => x.details)
                .SetValidator(new DetailValidator());

            RuleForEach(x => x.composition)
                .SetValidator(new CompositionValidator());

            RuleForEach(x => x.referenceCategories)
                .SetValidator(new ReferenceCategoryValidator());

            RuleForEach(x => x.colors)
                .SetValidator(new ColorValidator());
        }
    }
}
