using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Sellers
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Seller não dá tamanho dos textos (colunas do
    /// Script008_Create_Totvs_Sellers_Tables)
    /// </summary>
    public class OperationalAreaCityValidator : AbstractValidator<OperationalAreaCity>
    {
        public OperationalAreaCityValidator()
        {
            RuleFor(x => x.operationalAreaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'operationalAreaCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cityCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cityCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cityName)
                .MaximumLength(100)
                .WithMessage("O campo 'cityName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityName));

            RuleFor(x => x.stateCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.stateCode));

            // a Totvs manda a sigla com espaço à direita ("AP ", testado); a gravação já corta em 2
            RuleFor(x => x.stateAbbreviation)
                .Must(x => x!.TrimEnd().Length <= 2)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.stateName)
                .MaximumLength(60)
                .WithMessage("O campo 'stateName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateName));
        }
    }
}
