using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMetasVendedoresValidator : AbstractValidator<LinxMetasVendedores>
    {
        public LinxMetasVendedoresValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.id_meta).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_meta));
            RuleFor(x => x.descricao_meta).MaximumLength(50).WithMessage("O campo 'descricao_meta' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_meta));
            RuleFor(x => x.data_inicial_meta).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_inicial_meta));
            RuleFor(x => x.data_final_meta).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_final_meta));
            RuleFor(x => x.valor_meta_loja).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_meta_loja));
            RuleFor(x => x.valor_meta_vendedor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_meta_vendedor));
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
