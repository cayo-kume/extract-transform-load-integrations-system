using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// valida a consulta de produtos das notas antes de chamar a Totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.9, F10): lojas
    /// informadas, pelo menos um filtro restritivo (a Totvs exige produto, nota ou chave de acesso: 400 EnterAtLeastOne, testado), emissão
    /// com início e fim e até 31 dias, chaves com 44 dígitos e enums pelo nome do swagger. As lojas da empresa são conferidas no service
    /// </summary>
    public class InvoiceProductSearchValidator : AbstractValidator<InvoiceProductSearch>
    {
        private static readonly string[] OperationTypes = ["All", "Input", "Output"];
        private static readonly string[] Modalities = ["All", "Service", "Transfer", "Return", "SalePurchase", "Other", "Production", "Conditional", "Change", "Reissuance",
            "ThirdPartyProduction", "StreetSale", "Consignment", "IcmsIpiComplement", "FreightBill", "SingleShipmentReturn", "CiapInvoice", "FinancialAdvance"];
        private static readonly string[] Origins = ["All", "Own", "ThirdParty"];
        private static readonly string[] Statuses = ["Canceled", "Denied", "Issued", "Normal", "Deleted"];

        public InvoiceProductSearchValidator()
        {
            RuleFor(x => x.filter)
                .NotNull()
                .WithMessage("O campo 'filter' é obrigatório.");

            When(x => x.filter is not null, () =>
            {
                RuleFor(x => x.filter!.branchCodeList)
                    .NotEmpty()
                    .WithMessage("Informe ao menos uma loja em 'filter.branchCodeList'.");

                RuleForEach(x => x.filter!.branchCodeList)
                    .GreaterThan(0)
                    .WithMessage("Os códigos de 'filter.branchCodeList' devem ser maiores que zero.");

                RuleFor(x => x.filter)
                    .Must(f => f!.ProductCodeList?.Count > 0 || f.invoiceCodeList?.Count > 0 || f.acessKeyList?.Count > 0)
                    .WithMessage("Informe ao menos um entre 'filter.ProductCodeList', 'filter.invoiceCodeList' e 'filter.acessKeyList'.");

                RuleFor(x => x.filter)
                    .Must(f => f!.startIssueDate.HasValue == f.endIssueDate.HasValue)
                    .WithMessage("Informe 'filter.startIssueDate' e 'filter.endIssueDate' juntos.");

                RuleFor(x => x.filter)
                    .Must(f => f!.startIssueDate <= f.endIssueDate && (f.endIssueDate!.Value - f.startIssueDate!.Value).TotalDays <= 31)
                    .WithMessage("O intervalo de emissão deve ter início antes do fim e no máximo 31 dias.")
                    .When(x => x.filter!.startIssueDate.HasValue && x.filter.endIssueDate.HasValue);

                RuleForEach(x => x.filter!.acessKeyList)
                    .Matches(@"^\d{44}$")
                    .WithMessage("As chaves de 'filter.acessKeyList' devem ter 44 dígitos.");

                RuleForEach(x => x.filter!.personCpfCnpjList)
                    .Matches(@"^(\d{11}|\d{14})$")
                    .WithMessage("Os CPF/CNPJ de 'filter.personCpfCnpjList' devem ter 11 ou 14 dígitos.");

                RuleFor(x => x.filter!.operationType)
                    .Must(v => OperationTypes.Contains(v))
                    .WithMessage($"'filter.operationType' aceita {String.Join(", ", OperationTypes)}.")
                    .When(x => !String.IsNullOrEmpty(x.filter!.operationType));

                RuleFor(x => x.filter!.modality)
                    .Must(v => Modalities.Contains(v))
                    .WithMessage($"'filter.modality' aceita {String.Join(", ", Modalities)}.")
                    .When(x => !String.IsNullOrEmpty(x.filter!.modality));

                RuleFor(x => x.filter!.origin)
                    .Must(v => Origins.Contains(v))
                    .WithMessage($"'filter.origin' aceita {String.Join(", ", Origins)}.")
                    .When(x => !String.IsNullOrEmpty(x.filter!.origin));

                RuleForEach(x => x.filter!.invoiceStatusList)
                    .Must(v => Statuses.Contains(v))
                    .WithMessage($"'filter.invoiceStatusList' aceita {String.Join(", ", Statuses)}.");
            });

            RuleFor(x => x.page)
                .GreaterThan(0)
                .WithMessage("O campo 'page' deve ser maior que zero.");

            RuleFor(x => x.pageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("O campo 'pageSize' vai de 1 a 100.");

            RuleFor(x => x.expand)
                .Must(v => v == "batchItems")
                .WithMessage("'expand' aceita só batchItems.")
                .When(x => !String.IsNullOrEmpty(x.expand));
        }
    }
}
