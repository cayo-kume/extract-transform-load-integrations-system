using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class CardInformationValidator : AbstractValidator<CardInformation>
    {
        public CardInformationValidator()
        {
            RuleFor(x => x.authorizationCode)
                .MaximumLength(20)
                .WithMessage("O campo 'authorizationCode' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.authorizationCode));

            RuleFor(x => x.nsu)
                .MaximumLength(20)
                .WithMessage("O campo 'nsu' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu));

            RuleFor(x => x.inclusionType)
                .MaximumLength(10)
                .WithMessage("O campo 'inclusionType' deve ter no máximo 10 caracteres.")
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
