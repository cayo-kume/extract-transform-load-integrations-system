using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOrdemServicoExternaDavValidator : AbstractValidator<LinxOrdemServicoExternaDav>
    {
        public LinxOrdemServicoExternaDavValidator()
        {
            RuleFor(x => x.id_vendas_pos_tmp_ordem_servico_externa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_tmp_ordem_servico_externa));
            RuleFor(x => x.id_vendas_pos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
            RuleFor(x => x.codigo_ordem_servico_externa).MaximumLength(20).WithMessage("O campo 'codigo_ordem_servico_externa' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ordem_servico_externa));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
