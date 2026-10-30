using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class InstructionItemValidator : AbstractValidator<InstructionItem>
    {
        public InstructionItemValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(200)
                .WithMessage("O campo 'description' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.grouper)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.grouper));

            RuleFor(x => x.grouperDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'grouperDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.grouperDescription));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
