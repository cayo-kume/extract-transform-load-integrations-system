using Domain.DataWarehouse.Entities.Control;

namespace Domain.DataWarehouse.Abstractions.Repositorys
{
    public interface IDataWarehouseRepository
    {
        /// <summary>
        /// origens ligadas do catálogo dw_control.LandingSource
        /// </summary>
        public Task<IEnumerable<LandingSource>> GetLandingSources();

        /// <summary>
        /// todas as origens de um schema do catálogo, ligadas ou não: a carga do legado lê as do Linx, que ficam fora do job agendado
        /// </summary>
        public Task<IEnumerable<LandingSource>> GetLandingSourcesBySchema(string sourceSchema);

        /// <summary>
        /// executa uma procedure do DW que devolve @rows (as geradas e as de dimensão, fato e agregado) e devolve as linhas
        /// </summary>
        public Task<int> ExecuteProcedure(string procedureName, IDictionary<string, object?>? parameters = null);

        /// <summary>
        /// conta as linhas de uma view de checagem (datawarehouse.vw_Check_*): cada linha é um erro
        /// </summary>
        public Task<int> CountRows(string viewName);

        /// <summary>
        /// grava o resultado de uma checagem em dw_control.CheckResult
        /// </summary>
        public Task<bool> InsertCheckResult(CheckResult checkResult);
    }
}
