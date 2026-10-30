using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; sem tamanho no swagger, os das colunas do Script023_Create_Totvs_Invoices_Tables
    /// </summary>
    public class CostCenterValidator : AbstractValidator<CostCenter>
    {
        public CostCenterValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(100)
                .WithMessage("O campo 'description' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleForEach(x => x.classifications)
                .Must(x => !string.IsNullOrEmpty(x.typeCode) && !string.IsNullOrEmpty(x.code))
                .WithMessage("Classificação sem 'typeCode' ou 'code': descartada.");
        }
    }
}
