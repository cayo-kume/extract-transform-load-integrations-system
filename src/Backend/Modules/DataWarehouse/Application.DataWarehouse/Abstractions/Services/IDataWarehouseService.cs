namespace Application.DataWarehouse.Abstractions.Services
{
    public interface IDataWarehouseService
    {
        /// <summary>
        /// sequência da 10.1 a cada hora: landing de todas as origens, snapshots horários (notas e pedidos), dado pessoal,
        /// dimensões, fatos, agregados e checagens. true concluída, false com erro (detalhe no log), null já havia uma carga rodando
        /// </summary>
        public Task<bool?> RunHourly();

        /// <summary>
        /// a mesma sequência, mais os snapshots diários (cadastros, custos e preços) e a detecção de filhas apagadas na origem
        /// </summary>
        public Task<bool?> RunDaily();

        /// <summary>
        /// ciclo curto de 3 minutos da logística (10.3): landing de pedidos, notas e clientes e o dado pessoal de entrega
        /// </summary>
        public Task<bool?> RunShortCycle();
    }
}
