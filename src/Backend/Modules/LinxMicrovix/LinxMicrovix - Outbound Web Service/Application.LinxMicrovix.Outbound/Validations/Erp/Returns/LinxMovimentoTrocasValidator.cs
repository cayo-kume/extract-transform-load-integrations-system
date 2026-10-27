using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxMovimentoTrocasValidator : AbstractValidator<LinxMovimentoTrocas>
    {
        public LinxMovimentoTrocasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.num_vale)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.num_vale));

            RuleFor(x => x.valor_vale)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_vale));

            RuleFor(x => x.motivo)
                .MaximumLength(50)
                .WithMessage("O campo 'motivo' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motivo));

            RuleFor(x => x.doc_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.doc_origem));

            RuleFor(x => x.serie_origem)
                .MaximumLength(10)
                .WithMessage("O campo 'serie_origem' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie_origem));

            RuleFor(x => x.doc_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.doc_venda));

            RuleFor(x => x.serie_venda)
                .MaximumLength(10)
                .WithMessage("O campo 'serie_venda' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie_venda));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.desfazimento)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.desfazimento));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.vale_cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.vale_cod_cliente));

            RuleFor(x => x.vale_codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.vale_codigoproduto));

            RuleFor(x => x.id_vale_ordem_servico_externa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_vale_ordem_servico_externa));

            RuleFor(x => x.doc_venda_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.doc_venda_origem));

            RuleFor(x => x.serie_venda_origem)
                .MaximumLength(10)
                .WithMessage("O campo 'serie_venda_origem' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie_venda_origem));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.data_vencimento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_vencimento));

            RuleFor(x => x.valor_original)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_original));

            RuleFor(x => x.id_antecipacoes_financeiras)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));

            RuleFor(x => x.obs_troca)
                .MaximumLength(180)
                .WithMessage("O campo 'obs_troca' deve ter no máximo 180 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.obs_troca));
        }
    }
}
