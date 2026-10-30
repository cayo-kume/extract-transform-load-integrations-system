using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class RepresentativeValidator : AbstractValidator<Representative>
    {
        public RepresentativeValidator()
        {
            RuleFor(x => x.representativeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'representativeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.classificationCode)
                .MaximumLength(10)
                .WithMessage("O campo 'classificationCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificationCode));

            RuleFor(x => x.classificationTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.classificationTypeCode));
        }
    }
}
