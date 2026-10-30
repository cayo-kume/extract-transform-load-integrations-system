using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosCampanhasValidator : AbstractValidator<B2CConsultaProdutosCampanhas>
    {
        public B2CConsultaProdutosCampanhasValidator()
        {
            RuleFor(x => x.codigo_campanha)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_campanha' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_campanha)
                .MaximumLength(60)
                .WithMessage("O campo 'nome_campanha' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_campanha));

            RuleFor(x => x.vigencia_inicio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_inicio));

            RuleFor(x => x.vigencia_fim)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_fim));

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
