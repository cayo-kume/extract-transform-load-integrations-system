namespace Domain.DataWarehouse.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos scripts de jobs (Script006, Script071 e Script072). App 90, faixa 9000 - 9999
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        DataWarehouseHourly = 9000,       //Script006: sequência da 10.1 a cada hora (vendas e pedidos)
        DataWarehouseDaily = 9001,        //Script006: sequência da 10.1 com os snapshots diários e a detecção de exclusões
        DataWarehouseShortCycle = 9002,   //Script006: ciclo curto de 3 minutos da logística (10.3)
        DataWarehouseMonthlyStock = 9003, //Script071: recarga manual da posição de estoque de um fim de mês (8.5)
        DataWarehouseLinxLegacy = 9004    //Script072: carga única do legado Linx Microvix (10.2)
    }
}
