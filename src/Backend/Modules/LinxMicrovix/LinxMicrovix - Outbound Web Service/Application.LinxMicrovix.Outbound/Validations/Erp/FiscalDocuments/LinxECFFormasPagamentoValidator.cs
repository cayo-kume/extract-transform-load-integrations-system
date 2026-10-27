using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxECFFormasPagamentoValidator : AbstractValidator<LinxECFFormasPagamento>
    {
        public LinxECFFormasPagamentoValidator()
        {
            RuleFor(x => x.id_empresa_ecf_formas_pgto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_empresa_ecf_formas_pgto' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_ecf)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_ecf));

            RuleFor(x => x.cod_forma_pgto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));

            RuleFor(x => x.indice_forma)
                .MaximumLength(53)
                .WithMessage("O campo 'indice_forma' deve ter no máximo 53 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.indice_forma));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
