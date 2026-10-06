namespace Domain.Shared.Abstractions.Repositorys
{
    public interface ISharedRepository :
        IQueryRepository,
        IBulkInsertRepository,
        ICommandRepository,
        IMergeRepository,
        IConnectionAccessor
    { }
}
