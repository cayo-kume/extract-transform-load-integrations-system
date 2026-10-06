using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class CardInformationValidator : AbstractValidator<CardInformation>
    {
        public CardInformationValidator()
        {
            RuleFor(x => x.authorizationCode)
                .MaximumLength(60)
                .WithMessage("O campo 'authorizationCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.authorizationCode));
            
            RuleFor(x => x.nsu)
                .MaximumLength(60)
                .WithMessage("O campo 'nsu' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu));
            
            RuleFor(x => x.inclusionType)
                .MaximumLength(60)
                .WithMessage("O campo 'inclusionType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inclusionType));
            
            RuleFor(x => x.cardOperatorCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cardOperatorCode));

            RuleFor(x => x.cardOperatorName)
                .MaximumLength(60)
                .WithMessage("O campo 'cardOperatorName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cardOperatorName));

            RuleFor(x => x.cardFlag)
                .MaximumLength(60)
                .WithMessage("O campo 'cardFlag' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cardFlag));

            RuleFor(x => x.cardFlagCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cardFlagCode));
        }
    }
}
