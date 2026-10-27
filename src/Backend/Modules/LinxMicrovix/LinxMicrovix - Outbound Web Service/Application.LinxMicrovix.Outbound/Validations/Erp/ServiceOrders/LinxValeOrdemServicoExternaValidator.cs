using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxValeOrdemServicoExternaValidator : AbstractValidator<LinxValeOrdemServicoExterna>
    {
        public LinxValeOrdemServicoExternaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.id_vale_ordem_servico_externa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_vale_ordem_servico_externa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.numero_controle)
                .MaximumLength(25)
                .WithMessage("O campo 'numero_controle' deve ter no máximo 25 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_controle));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
