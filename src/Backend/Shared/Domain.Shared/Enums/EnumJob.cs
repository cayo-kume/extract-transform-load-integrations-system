namespace Domain.Shared.Enums
{
    /// <summary>
    /// Os jobs de integração ficam nos scripts de jobs de cada módulo. Aqui ficam só os jobs dos próprios hosts de agenda
    /// (app 140, range 14000 - 14499), cadastrados no Script007_Insert_Hosts_Jobs_Into_Jobs_Table.sql do Shared
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        /// <summary>
        /// recuperação dos WebJobs travados (host WebJob, src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 7)
        /// </summary>
        StuckJobsRecovery = 14000
    }
}
