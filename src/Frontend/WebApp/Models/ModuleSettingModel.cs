namespace WebApp.Models
{
    /// <summary>
    /// Um parâmetro de configuração de um módulo (credencial ou parâmetro global), exibido no diálogo de configurações
    /// da Visão geral. No banco, vem dos scripts de Parameters de cada módulo; na fase 1 é mock e somente leitura.
    /// </summary>
    public class ModuleSettingModel
    {
        public string? Label { get; set; }
        public string? Value { get; set; }

        /// <summary>
        /// Credencial: o valor chega e aparece mascarado.
        /// </summary>
        public bool IsSecret { get; set; }
    }
}
