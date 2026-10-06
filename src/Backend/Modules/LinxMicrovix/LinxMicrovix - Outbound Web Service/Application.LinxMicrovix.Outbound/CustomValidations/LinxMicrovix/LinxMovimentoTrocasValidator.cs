using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoTrocasValidator : AbstractValidator<LinxMovimentoTrocas>
    {
        public LinxMovimentoTrocasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.num_vale).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.num_vale));
            RuleFor(x => x.valor_vale).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_vale));
            RuleFor(x => x.motivo).MaximumLength(50).WithMessage("O campo 'motivo' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.motivo));
            RuleFor(x => x.doc_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.doc_origem));
            RuleFor(x => x.serie_origem).MaximumLength(10).WithMessage("O campo 'serie_origem' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie_origem));
            RuleFor(x => x.doc_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.doc_venda));
            RuleFor(x => x.serie_venda).MaximumLength(10).WithMessage("O campo 'serie_venda' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie_venda));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.desfazimento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.desfazimento));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.vale_cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.vale_cod_cliente));
            RuleFor(x => x.vale_codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.vale_codigoproduto));
            RuleFor(x => x.id_vale_ordem_servico_externa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vale_ordem_servico_externa));
            RuleFor(x => x.doc_venda_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.doc_venda_origem));
            RuleFor(x => x.serie_venda_origem).MaximumLength(10).WithMessage("O campo 'serie_venda_origem' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie_venda_origem));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
        }
    }
}
