using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaLegendasCadastrosAuxiliaresValidator : AbstractValidator<B2CConsultaLegendasCadastrosAuxiliares>
    {
        public B2CConsultaLegendasCadastrosAuxiliaresValidator()
        {
            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.legenda_setor)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_setor' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_setor));

            RuleFor(x => x.legenda_linha)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_linha' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_linha));

            RuleFor(x => x.legenda_marca)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_marca' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_marca));

            RuleFor(x => x.legenda_colecao)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_colecao' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_colecao));

            RuleFor(x => x.legenda_grade1)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_grade1' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_grade1));

            RuleFor(x => x.legenda_grade2)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_grade2' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_grade2));

            RuleFor(x => x.legenda_espessura)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_espessura' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_espessura));

            RuleFor(x => x.legenda_classificacao)
                .MaximumLength(20)
                .WithMessage("O campo 'legenda_classificacao' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda_classificacao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
