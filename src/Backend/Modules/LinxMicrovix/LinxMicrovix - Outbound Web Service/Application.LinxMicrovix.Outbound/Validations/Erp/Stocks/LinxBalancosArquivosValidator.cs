using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxBalancosArquivosValidator : AbstractValidator<LinxBalancosArquivos>
    {
        public LinxBalancosArquivosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_balancos_arquivos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_balancos_arquivos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.qtdeproduto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtdeproduto));

            RuleFor(x => x.lote)
                .MaximumLength(60)
                .WithMessage("O campo 'lote' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lote));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
