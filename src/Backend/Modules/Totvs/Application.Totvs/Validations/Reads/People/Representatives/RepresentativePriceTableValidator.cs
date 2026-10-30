using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Representatives;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.People.Representatives
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script029_Create_Totvs_Representatives_Tables
    /// </summary>
    public class RepresentativePriceTableValidator : AbstractValidator<RepresentativePriceTable>
    {
        public RepresentativePriceTableValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.name)
                .MaximumLength(100)
                .WithMessage("O campo 'name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
