using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxTrocaUnificadaResumoVendasValidator : AbstractValidator<LinxTrocaUnificadaResumoVendas>
    {
        public LinxTrocaUnificadaResumoVendasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_troca_unificada_resumo_vendas)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_troca_unificada_resumo_vendas' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.token)
                .MaximumLength(30)
                .WithMessage("O campo 'token' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.token));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.documento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.serie)
                .MaximumLength(10)
                .WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie));

            RuleFor(x => x.data_venda)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_venda));

            RuleFor(x => x.documento_cliente)
                .MaximumLength(14)
                .WithMessage("O campo 'documento_cliente' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documento_cliente));

            RuleFor(x => x.venda_cancelada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.venda_cancelada));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
