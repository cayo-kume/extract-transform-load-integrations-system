using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Representatives;
using FluentValidation;

namespace Application.Totvs.Validations.People.Representatives
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script029_Create_Totvs_Representatives_Tables
    /// </summary>
    public class RepresentativeSalesRegionValidator : AbstractValidator<RepresentativeSalesRegion>
    {
        public RepresentativeSalesRegionValidator()
        {
            RuleFor(x => x.salesRegionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'salesRegionCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.salesRegionName)
                .MaximumLength(100)
                .WithMessage("O campo 'salesRegionName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.salesRegionName));

            RuleFor(x => x.salesOrderClassificationTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'salesOrderClassificationTypeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.salesOrderClassificationTypeName)
                .MaximumLength(100)
                .WithMessage("O campo 'salesOrderClassificationTypeName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.salesOrderClassificationTypeName));

            RuleFor(x => x.salesOrderClassificationCode)
                .MaximumLength(10)
                .WithMessage("O campo 'salesOrderClassificationCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.salesOrderClassificationCode));

            RuleFor(x => x.salesOrderClassificationName)
                .MaximumLength(100)
                .WithMessage("O campo 'salesOrderClassificationName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.salesOrderClassificationName));
        }
    }
}
