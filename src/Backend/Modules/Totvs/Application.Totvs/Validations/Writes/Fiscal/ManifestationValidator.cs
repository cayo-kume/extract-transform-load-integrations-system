using Domain.Totvs.Dtos.Writes.Fiscal;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Fiscal
{
    /// <summary>
    /// valida o corpo da manifestação no controller (corpo inválido volta 400): de 1 a 8 documentos (as chaves cabem nos 400 caracteres de
    /// field_key_value), chave de 44 dígitos sem repetição, CNPJs com 14 dígitos, os campos sem nullable no swagger (número, NSU, emissão, total,
    /// modelo) obrigatórios e os enums de texto pelo nome
    /// </summary>
    public class ManifestationValidator : AbstractValidator<Manifestation>
    {
        private static readonly string[] OperationTypes = ["Input", "Output"];
        private static readonly string[] InvoiceStatus = ["Authorized", "Canceled", "Denied", "Issued", "Generated", "Rejected"];

        public ManifestationValidator()
        {
            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um documento em 'items'.");

            RuleFor(x => x.items)
                .Must(l => l!.Count <= 8)
                .WithMessage("Até 8 documentos por chamada.")
                .Must(l => l!.Where(i => !String.IsNullOrEmpty(i?.accessKey)).GroupBy(i => i!.accessKey).All(g => g.Count() == 1))
                .WithMessage("A mesma chave de acesso aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x.accessKey)
                        .NotEmpty()
                        .WithMessage("O campo 'accessKey' é obrigatório.")
                        .Matches(@"^\d{44}$")
                        .WithMessage("O campo 'accessKey' deve ter 44 dígitos.");

                    i.RuleFor(x => x.branchCnpj)
                        .NotEmpty()
                        .WithMessage("O campo 'branchCnpj' é obrigatório (loja da empresa).")
                        .Matches(@"^\d{14}$")
                        .WithMessage("O campo 'branchCnpj' deve ter 14 dígitos, sem pontuação.");

                    i.RuleFor(x => x.issuerCnpj)
                        .Matches(@"^\d{14}$")
                        .WithMessage("O campo 'issuerCnpj' deve ter 14 dígitos, sem pontuação.")
                        .When(x => !String.IsNullOrEmpty(x.issuerCnpj));

                    i.RuleFor(x => x.invoiceCode)
                        .NotNull()
                        .WithMessage("O campo 'invoiceCode' é obrigatório.")
                        .GreaterThan(0)
                        .WithMessage("O campo 'invoiceCode' deve ser maior que zero.");

                    i.RuleFor(x => x.nsu)
                        .NotNull()
                        .WithMessage("O campo 'nsu' é obrigatório.")
                        .GreaterThan(0)
                        .WithMessage("O campo 'nsu' deve ser maior que zero.");

                    i.RuleFor(x => x.issueDate).NotNull().WithMessage("O campo 'issueDate' é obrigatório.");

                    i.RuleFor(x => x.totalValue)
                        .NotNull()
                        .WithMessage("O campo 'totalValue' é obrigatório.")
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O campo 'totalValue' não pode ser negativo.");

                    i.RuleFor(x => x.documentType)
                        .NotNull()
                        .WithMessage("O campo 'documentType' é obrigatório (55 NFe, 57 CTe, 59 CFe, 81 NFSe).")
                        .IsInEnum()
                        .WithMessage("O campo 'documentType' só aceita 55 (NFe), 57 (CTe), 59 (CFe) ou 81 (NFSe).");

                    i.RuleFor(x => x.operationType)
                        .Must(o => OperationTypes.Contains(o))
                        .WithMessage("O campo 'operationType' só aceita Input ou Output (pelo nome).")
                        .When(x => !String.IsNullOrEmpty(x.operationType));

                    i.RuleFor(x => x.invoiceStatus)
                        .Must(s => InvoiceStatus.Contains(s))
                        .WithMessage("O campo 'invoiceStatus' só aceita Authorized, Canceled, Denied, Issued, Generated ou Rejected (pelo nome).")
                        .When(x => !String.IsNullOrEmpty(x.invoiceStatus));
                })
                .When(x => x.items is not null);
        }
    }
}
