using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoOrdemServicoExternaValidator : AbstractValidator<LinxMovimentoOrdemServicoExterna>
    {
        public LinxMovimentoOrdemServicoExternaValidator()
        {
            RuleFor(x => x.id_movimento_ordem_servico_externa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_movimento_ordem_servico_externa));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.codigo_ordem_servico).MaximumLength(20).WithMessage("O campo 'codigo_ordem_servico' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ordem_servico));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}