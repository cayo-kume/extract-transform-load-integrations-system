using Domain.Shared.Entities.Base;

namespace Domain.LinxMicrovix.Outbound.Entities.Parameters
{
    /// <summary>
    /// linha de linxmicrovix.LinxMicrovixParameters: um cliente da Linx, com as credenciais de cada API.
    /// Usuário e senha são diferentes por API (linx_export no ERP, linx_b2c no B2C); um cliente sem uma das APIs deixa
    /// as colunas dela nulas, e os jobs dela pulam o cliente. As lojas vêm de LinxGrupoLojas e B2CConsultaEmpresas, não daqui
    /// </summary>
    public class Company : CompanyBase
    {
        public string? grupo { get; init; }

        public Guid? erp_chave { get; init; }
        public string? erp_user_name { get; init; }
        public string? erp_password { get; init; }

        public Guid? b2c_chave { get; init; }
        public string? b2c_user_name { get; init; }
        public string? b2c_password { get; init; }
    }
}
