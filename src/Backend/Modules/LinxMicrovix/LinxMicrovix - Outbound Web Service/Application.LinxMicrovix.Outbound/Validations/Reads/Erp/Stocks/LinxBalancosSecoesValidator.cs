using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxBalancosSecoesValidator : AbstractValidator<LinxBalancosSecoes>
    {
        public LinxBalancosSecoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_balancos_secoes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_balancos_secoes' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_balancos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_balancos));

            RuleFor(x => x.id_balancos_arquivos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_balancos_arquivos));

            RuleFor(x => x.status)
                .MaximumLength(100)
                .WithMessage("O campo 'status' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
