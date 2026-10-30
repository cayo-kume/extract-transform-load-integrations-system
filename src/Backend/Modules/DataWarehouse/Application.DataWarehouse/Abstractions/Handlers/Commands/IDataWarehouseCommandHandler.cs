namespace Application.DataWarehouse.Abstractions.Handlers.Commands
{
    public interface IDataWarehouseCommandHandler
    {
        public string CreateGetLandingSourcesQuery();
        public string CreateGetLandingSourcesBySchemaQuery();
        public string CreateCountRowsQuery(string viewName);
        public string CreateInsertCheckResultQuery();
    }
}
