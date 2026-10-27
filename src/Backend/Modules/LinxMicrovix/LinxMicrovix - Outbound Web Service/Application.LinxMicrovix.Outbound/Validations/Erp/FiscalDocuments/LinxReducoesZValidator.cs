using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxReducoesZValidator : AbstractValidator<LinxReducoesZ>
    {
        public LinxReducoesZValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj));

            RuleFor(x => x.data_fechamento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_fechamento));

            RuleFor(x => x.numero_ecf)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_ecf));

            RuleFor(x => x.numeroserie)
                .MaximumLength(30)
                .WithMessage("O campo 'numeroserie' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numeroserie));

            RuleFor(x => x.numero_reducoes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_reducoes));

            RuleFor(x => x.cupom_inicial)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cupom_inicial));

            RuleFor(x => x.cupom_final)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cupom_final));

            RuleFor(x => x.gt_inicial)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.gt_inicial));

            RuleFor(x => x.gt_final)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.gt_final));

            RuleFor(x => x.qtde_cupons)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_cupons));

            RuleFor(x => x.venda_bruta)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.venda_bruta));

            RuleFor(x => x.descontos)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.descontos));

            RuleFor(x => x.acrescimos)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.acrescimos));

            RuleFor(x => x.cancelamentos)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.cancelamentos));

            RuleFor(x => x.cro)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cro));

            RuleFor(x => x.usuario)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.usuario));

            RuleFor(x => x.aliquota)
                .MaximumLength(4)
                .WithMessage("O campo 'aliquota' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.aliquota));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.data_mov)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_mov));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
