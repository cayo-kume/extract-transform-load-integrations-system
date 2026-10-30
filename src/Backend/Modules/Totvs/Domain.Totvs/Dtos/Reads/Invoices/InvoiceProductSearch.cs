namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// consulta de produtos das notas (POST fiscal/v2/invoice-products/search, InvoiceProductSearchQuery), recebida pela rota e enviada à
    /// Totvs como veio (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.9). Os enums vão pelo nome do swagger: o número dá 400 InvalidValue (testado)
    /// </summary>
    public class InvoiceProductSearch
    {
        public InvoiceProductSearchFilter? filter { get; init; }
        public InvoiceProductSearchOption? option { get; init; }
        public int page { get; init; } = 1;
        public int pageSize { get; init; } = 100;

        // "batchItems"
        public string? expand { get; init; }

        // "branchCode", "invoiceSequence", "invoiceDate" (com "-" para descendente)
        public string? order { get; init; }
    }

    /// <summary>
    /// filtro da consulta (InvoiceProductFilterModel); ProductCodeList e BatchBarCodeList na grafia do swagger, com maiúscula, e
    /// acessKeyList como lá (sem o segundo "c")
    /// </summary>
    public class InvoiceProductSearchFilter
    {
        public List<int>? branchCodeList { get; init; }
        public List<int>? ProductCodeList { get; init; }
        public List<string>? BatchBarCodeList { get; init; }
        public List<int>? invoiceCodeList { get; init; }
        public string? operationType { get; init; }
        public string? modality { get; init; }
        public string? origin { get; init; }
        public List<string>? invoiceStatusList { get; init; }
        public List<int>? personCodeList { get; init; }
        public List<int>? ignorePersonCodeList { get; init; }
        public List<int>? operationCodeList { get; init; }
        public List<string>? personCpfCnpjList { get; init; }
        public List<string>? ignorePersonCpfCnpjList { get; init; }
        public DateTime? startIssueDate { get; init; }
        public DateTime? endIssueDate { get; init; }
        public List<string>? acessKeyList { get; init; }
    }

    /// <summary>
    /// opção da consulta (InvoiceProductOptionModel): isReturn traz a quantidade já devolvida de cada produto
    /// </summary>
    public class InvoiceProductSearchOption
    {
        public bool? isReturn { get; init; }
    }
}
