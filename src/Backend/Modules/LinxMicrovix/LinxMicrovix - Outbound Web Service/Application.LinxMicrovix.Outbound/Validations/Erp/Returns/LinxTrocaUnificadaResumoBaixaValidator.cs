using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxTrocaUnificadaResumoBaixaValidator : AbstractValidator<LinxTrocaUnificadaResumoBaixa>
    {
        public LinxTrocaUnificadaResumoBaixaValidator()
        {
            RuleFor(x => x.portal_baixa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal_baixa));

            RuleFor(x => x.empresa_baixa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa_baixa));

            RuleFor(x => x.cnpj_empresa_baixa)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_empresa_baixa' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_empresa_baixa));

            RuleFor(x => x.id_troca_baixa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_troca_baixa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_troca_unificada_resumo_vendas_itens)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_troca_unificada_resumo_vendas_itens));

            RuleFor(x => x.data_troca_baixa)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_troca_baixa));

            RuleFor(x => x.transacao_baixa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_baixa));

            RuleFor(x => x.descricao_empresa_baixa)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao_empresa_baixa' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_empresa_baixa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
