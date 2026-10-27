using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxConhecimentoFretesValidator : AbstractValidator<LinxConhecimentoFretes>
    {
        public LinxConhecimentoFretesValidator()
        {
            RuleFor(x => x.id_conhecimento)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_conhecimento' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.transportador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transportador));

            RuleFor(x => x.valor_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_frete));

            RuleFor(x => x.conhecimento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.conhecimento));

            RuleFor(x => x.serie_frete)
                .MaximumLength(10)
                .WithMessage("O campo 'serie_frete' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie_frete));

            RuleFor(x => x.vencto_frete)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vencto_frete));

            RuleFor(x => x.valor_total_notas)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_total_notas));

            RuleFor(x => x.valor_lancado)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_lancado));

            RuleFor(x => x.data_emissao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_emissao));

            RuleFor(x => x.data_entrada)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_entrada));

            RuleFor(x => x.aliq_icms)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_icms));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.base_calculo_icms)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_calculo_icms));

            RuleFor(x => x.id_cfop_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cfop_fiscal));

            RuleFor(x => x.id_modelo_nf)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_modelo_nf));

            RuleFor(x => x.id_cte_tipo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cte_tipo));

            RuleFor(x => x.valor_icms)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_icms));

            RuleFor(x => x.valor_pis)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_pis));

            RuleFor(x => x.valor_cofins)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_cofins));

            RuleFor(x => x.chave_cte)
                .MaximumLength(44)
                .WithMessage("O campo 'chave_cte' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave_cte));

            RuleFor(x => x.base_pis)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_pis));

            RuleFor(x => x.base_cofins)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_cofins));

            RuleFor(x => x.id_cst_icms_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cst_icms_fiscal));

            RuleFor(x => x.id_nfe_ibge_municipio_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe_ibge_municipio_origem));

            RuleFor(x => x.id_nfe_ibge_municipio_destino)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe_ibge_municipio_destino));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
