using Application.DataWarehouse.Abstractions.Handlers.Commands;

namespace Application.DataWarehouse.Handlers.Commands
{
    public class DataWarehouseCommandHandler : IDataWarehouseCommandHandler
    {
        public string CreateGetLandingSourcesQuery()
        {
            // só as ligadas: a de saldos nasce desligada até a carga inicial na Totvs terminar (is_enabled em dw_control.LandingSource)
            return "select source_schema, source_table, detect_missing, history_frequency from [dw_control].LandingSource where is_enabled = 1 order by source_schema, source_table";
        }

        public string CreateGetLandingSourcesBySchemaQuery()
        {
            return "select source_schema, source_table, detect_missing, history_frequency from [dw_control].LandingSource where source_schema = @sourceSchema order by source_table";
        }

        public string CreateCountRowsQuery(string viewName)
        {
            // o nome chega validado pelo repositório ([schema].[nome], só letras, números e sublinhado)
            return $"select count(*) from {viewName}";
        }

        public string CreateInsertCheckResultQuery()
        {
            return "insert into [dw_control].CheckResult (check_name, executed_at, error_rows, passed) values (@check_name, @executed_at, @error_rows, @passed)";
        }
    }
}
