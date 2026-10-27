using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Representatives;
using FluentValidation;

namespace Application.Totvs.Validations.People.Representatives
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script036_Create_Totvs_Representatives_Tables
    /// </summary>
    public class RepresentativeParameterValidator : AbstractValidator<RepresentativeParameter>
    {
        public RepresentativeParameterValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.parameterCode)
                .MaximumLength(25)
                .WithMessage("O campo 'parameterCode' deve ter no máximo 25 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.parameterCode));

            RuleFor(x => x.parameterValue)
                .MaximumLength(2000)
                .WithMessage("O campo 'parameterValue' deve ter no máximo 2000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.parameterValue));
        }
    }
}
