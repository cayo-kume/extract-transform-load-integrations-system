namespace Domain.LinxMicrovix.Outbound.Enums
{
    /// <summary>
    /// as duas APIs de saída do Microvix: o Web Service de Saída Padrão (ERP) e o WS B2C. Cada uma tem a sua chave, o seu
    /// usuário e as suas lojas (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §5.1)
    /// </summary>
    public enum EnumApi
    {
        Erp = 1,
        B2C = 2
    }
}
