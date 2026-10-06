using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosTabelasValidator : AbstractValidator<LinxProdutosTabelas>
    {
        public LinxProdutosTabelasValidator()
        {
            RuleFor(x => x.id_tabela).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_tabela));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.nome_tabela).MaximumLength(50).WithMessage("O campo 'nome_tabela' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_tabela));
            RuleFor(x => x.ativa).MaximumLength(1).WithMessage("O campo 'ativa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.ativa));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.tipo_tabela).MaximumLength(1).WithMessage("O campo 'tipo_tabela' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_tabela));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
        }
    }
}
