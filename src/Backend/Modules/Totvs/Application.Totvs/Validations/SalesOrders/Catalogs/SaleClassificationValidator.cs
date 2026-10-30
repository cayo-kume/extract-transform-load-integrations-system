using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order e das colunas do Script017_Create_Totvs_Sales_Orders_Tables
    /// </summary>
    public class SaleClassificationValidator : AbstractValidator<SaleClassification>
    {
        public SaleClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.typeDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'typeDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeDescription));

            RuleFor(x => x.type)
                .MaximumLength(20)
                .WithMessage("O campo 'type' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório: o registro não é gravado.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
