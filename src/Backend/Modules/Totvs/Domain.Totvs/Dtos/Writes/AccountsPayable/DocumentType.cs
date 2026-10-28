namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// tipo de documento da parcela (campo "Tipo documento" do FCPFM004), com os números da descrição do swagger: vai como número (T3)
    /// </summary>
    public enum DocumentType
    {
        Duplicate = 1,   // duplicata
        InvoicePrint = 2,   // boleto bancário
        Commission = 3,   // comissão
        Guide = 4,   // guia (doc.)
        Financing = 5,   // financiamento
        Voucher = 6,   // vale
        Invoice = 7,   // nota fiscal
        AccountDiscountNote = 8,   // nota de desconto em conta
        AdministrationFee = 9,   // taxa de administração
        InterestWithoutFinancing = 10,   // juros s/ financiamento
        Bonus = 11,   // bonificação
        BankDeposit = 12,   // depósito bancário
        Compror = 13,   // compror
        Vendor = 14,   // vendor
        Receipt = 15,   // recibo
        TedDoc = 16,   // TED / DOC
        Loan = 17,   // empréstimo
        FreightKnowledge = 18,   // conhecimento de frete
        ProLabore = 19,   // pró-labore
        AdvanceMoney = 20,   // adiantamento
        DarfNormal = 21,   // guia (DARF normal)
        Gps = 22,   // guia (GPS)
        Gare = 23,   // guia (GARE)
        Gare120 = 24,   // guia (GARE 120)
        Fgts = 25,   // guia (FGTS)
        IPTU = 26,   // guia (IPTU)
        Gnre = 27,   // guia (GNRE)
        DarfSimple = 28,   // guia (DARF Simples)
        Darj = 29,   // guia (DARJ)
        GiftCheck = 30,   // cheque presente
        InvoiceThird = 31,   // fatura terceiro
        Ipva = 32,   // guia (IPVA)
        Licensing = 33,   // guia (licenciamento)
        Dpvat = 34,   // guia (DPVAT)
        GareSpIcms = 35,   // guia (GARE SP ICMS)
        GareSpDr = 36,   // guia (GARE SP DR)
        GareSpItcmd = 37,   // guia (GARE SP ITCMD)
        GareSpIcmsImport = 38,   // guia (GARE SP ICMS importação)
        DareSp = 39,   // guia (DARE SP)
        Iss = 40,   // guia (ISS)
        Daems = 41,   // guia (DAEMS)
        GnreSp = 42,   // guia (GNRE-SP)
        OutherTitle = 50,   // outro título (grafia do swagger)
        DuplicatePublic = 51,   // duplicata pública
        ConcessionLoan = 52,   // empréstimo concessão
        ReturnAdvanceCustomer = 53,   // devolução adiant. cliente
        ReturnCredevCustomer = 54,   // devolução CREDEV cliente
        IndemnityTermination = 55   // comissão (rescisão indenizatória)
    }
}
