namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs que só executam procedures no banco, do WebJob do sistema antigo (RecurringJobs/Procedures), **comentados** até as
    /// procedures existirem neste repositório (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §4.2). Ao descomentar um: a linha em
    /// auditing.Jobs (com RunningDailyAt e Queue, e o intervalo da tabela, não o do comentário), o IdJob dela aqui e a versão. O
    /// intervalo do comentário é o que o job tinha no sistema antigo, como referência
    /// </summary>
    public static class ProceduresRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            // General (Procedures/General.cs no sistema antigo)
            // P_Volo_Integracao_Pedidos: roda [general].[p_Volo_Integracao_Pedidos] uma vez por EXECUTION de GENERAL.IT4_WMS_DOCUMENTO_CARGA; vira um método próprio, não ForProcedure (a cada 3 minutos no sistema antigo)
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_Volo_Integracao_Pedidos_Ecommerce_Totvs]", "1.0.0"), // p_Volo_Integracao_Pedidos_Ecommerce_Totvs: a cada 2 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_Volo_Integracao_Pedidos_Reposicao_Lojas_Totvs]", "1.0.0"), // p_Volo_Integracao_Pedidos_Reposicao_Lojas_Totvs: a cada 2 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Pedidos_Venda_Facil_Totvs]", "1.0.0"), // P_Volo_Integracao_Pedidos_Venda_Facil_Totvs: a cada 2 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Pedidos_Venda_Direta_Totvs]", "1.0.0"), // P_Volo_Integracao_Pedidos_Venda_Direta_Totvs: a cada 2 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Pedidos_Recebimento_Totvs]", "1.0.0"), // P_Volo_Integracao_Pedidos_Recebimento_Totvs: a cada 2 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_Volo_Integracao_Pedidos_IntegrityLock]", "1.0.0"), // p_Volo_Integracao_Pedidos_IntegrityLock: a cada 5 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Infos_NFe]", "1.0.0"), // P_Volo_Integracao_Infos_NFe: a cada 5 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Infos_NFe_Totvs]", "1.0.0"), // P_Volo_Integracao_Infos_NFe_Totvs: a cada 5 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Infos_Pedidos]", "1.0.0"), // P_Volo_Integracao_Infos_Pedidos: a cada 5 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Infos_Pedidos_Totvs]", "1.0.0"), // P_Volo_Integracao_Infos_Pedidos_Totvs: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_Volo_Integracao_Interfaces_Pedidos]", "1.0.0"), // P_Volo_Integracao_Interface_Pedidos: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Fornecedores]", "1.0.0"), // P_Volo_Integracao_Fornecedores: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Fornecedores_Totvs]", "1.0.0"), // P_Volo_Integracao_Fornecedores_Totvs: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Produtos]", "1.0.0"), // P_Volo_Integracao_Produtos: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Produtos_Totvs]", "1.0.0"), // P_Volo_Integracao_Produtos_Totvs: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "migracao.P_Insere_Produtos_De_Para", "1.0.0"), // P_Insere_Produtos_De_Para: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Transportadoras]", "1.0.0"), // P_Volo_Integracao_Transportadoras: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Clientes]", "1.0.0"), // P_Volo_Integracao_Clientes: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_Volo_Integracao_Clientes_Totvs]", "1.0.0"), // P_Volo_Integracao_Clientes_Totvs: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[dbo].[P_Server_Maintenance]", "1.0.0"), // P_Server_Maintenance: todo dia às 06:00 no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_InsertJadLogUnidadeOrigem]", "1.0.0"), // p_JadLog_Insert_Unidade_Origem: todo dia às 06:00 no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_Volo_Integracao_Infos_Pedidos_Zpl]", "1.0.0"), // P_Volo_Integracao_Infos_Pedidos_Zpl: a cada 3 minutos no sistema antigo

            // LinxMicrovix (Procedures/LinxMicrovix.cs no sistema antigo)
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[linx_microvix_erp].[p_InsertMissingLinxProdutosDetalhes]", "1.0.0"), // p_Insert_Missing_LinxProdutosDetalhes: todo dia às 06:00 no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[linx_microvix_erp].[p_InsertMissingLinxProdutosInventario]", "1.0.0"), // P_Insert_Missing_LinxProdutosInventario: todo dia às 06:00 no sistema antigo

            // Operacoes (Procedures/Operacoes.cs no sistema antigo)
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Periodo_Entrada]", "1.0.0"), // P_Canal_Movimentacao_Periodo_Entrada: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Periodo_Ecommerce]", "1.0.0"), // P_Canal_Movimentacao_Periodo_Ecommerce: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Periodo_Loja]", "1.0.0"), // P_Canal_Movimentacao_Periodo_Loja: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Captado_Periodo_Entrada]", "1.0.0"), // P_Canal_Movimentacao_Captado_Periodo_Entrada: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Captado_Periodo_Ecommerce]", "1.0.0"), // P_Canal_Movimentacao_Captado_Periodo_Ecommerce: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Canal_Movimentacao_Captado_Periodo_Loja]", "1.0.0"), // P_Canal_Movimentacao_Captado_Periodo_Loja: de hora em hora no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[operations].[P_Snapshot_Estoque]", "1.0.0"), // P_Snapshot_Estoque: todo dia às 23:57 no sistema antigo

            // Shippment (Procedures/Shippment.cs no sistema antigo)
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[p_InsertJadLogUnidadeOrigem]", "1.0.0"), // p_InsertJadLogUnidadeOrigem: a cada 3 minutos no sistema antigo
            // RecurringJobDefinition.ForProcedure(<IdJob>, "[general].[P_JadLogInsertCodigoShipmentId]", "1.0.0"), // P_JadLogInsertCodigoShipmentId: a cada 3 minutos no sistema antigo
        ];
    }
}
