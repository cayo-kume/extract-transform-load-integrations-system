using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxServicosDetalhesValidator : AbstractValidator<LinxServicosDetalhes>
    {
        public LinxServicosDetalhesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.cod_servico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_servico' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.preco_custo_hn)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_custo_hn));

            RuleFor(x => x.preco_primera_hn)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_primera_hn));

            RuleFor(x => x.preco_demais_hn)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_demais_hn));

            RuleFor(x => x.preco_custo_he)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_custo_he));

            RuleFor(x => x.preco_primera_he)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_primera_he));

            RuleFor(x => x.preco_demais_he)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_demais_he));

            RuleFor(x => x.preco_custo_had)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_custo_had));

            RuleFor(x => x.preco_primera_had)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_primera_had));

            RuleFor(x => x.preco_demais_had)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_demais_had));

            RuleFor(x => x.id_config_tributaria)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_config_tributaria));

            RuleFor(x => x.desc_config_tributaria)
                .MaximumLength(100)
                .WithMessage("O campo 'desc_config_tributaria' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_config_tributaria));

            RuleFor(x => x.cod_trib_servico_municipio)
                .MaximumLength(20)
                .WithMessage("O campo 'cod_trib_servico_municipio' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_trib_servico_municipio));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
