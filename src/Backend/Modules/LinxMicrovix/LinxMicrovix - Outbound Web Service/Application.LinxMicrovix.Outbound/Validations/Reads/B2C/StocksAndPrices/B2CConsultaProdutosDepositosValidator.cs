using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosDepositosValidator : AbstractValidator<B2CConsultaProdutosDepositos>
    {
        public B2CConsultaProdutosDepositosValidator()
        {
            RuleFor(x => x.id_deposito)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_deposito' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_deposito)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_deposito' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_deposito));

            RuleFor(x => x.disponivel)
                .MaximumLength(1)
                .WithMessage("O campo 'disponivel' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.disponivel));

            RuleFor(x => x.disponivel_transferencia)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.disponivel_transferencia));

            RuleFor(x => x.disponivel_franquias)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.disponivel_franquias));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.outlet)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.outlet));
        }
    }
}
