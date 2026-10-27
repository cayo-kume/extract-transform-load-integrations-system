using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxDesfazimentosAntecipacoesDocumentoValidator : AbstractValidator<LinxDesfazimentosAntecipacoesDocumento>
    {
        public LinxDesfazimentosAntecipacoesDocumentoValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_documento_desfazimento_optico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_documento_desfazimento_optico' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_desfazimentos_antecipacoes)
                .MaximumLength(1000)
                .WithMessage("O campo 'id_desfazimentos_antecipacoes' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id_desfazimentos_antecipacoes));

            RuleFor(x => x.id_antecipacoes_financeiras)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));

            RuleFor(x => x.cfop)
                .MaximumLength(10)
                .WithMessage("O campo 'cfop' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.valor_operacao_nao_desfeita)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_operacao_nao_desfeita));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
