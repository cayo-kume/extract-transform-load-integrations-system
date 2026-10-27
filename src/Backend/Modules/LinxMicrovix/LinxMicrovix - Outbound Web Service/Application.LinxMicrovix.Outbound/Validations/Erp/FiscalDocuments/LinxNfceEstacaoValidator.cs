using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxNfceEstacaoValidator : AbstractValidator<LinxNfceEstacao>
    {
        public LinxNfceEstacaoValidator()
        {
            RuleFor(x => x.id_nfce_estacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_nfce_estacao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.descricao)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.numero_pdv_tef)
                .MaximumLength(20)
                .WithMessage("O campo 'numero_pdv_tef' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_pdv_tef));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
