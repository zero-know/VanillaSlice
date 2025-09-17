namespace {{ProjectName}}.Framework
{
    public interface ISelectListDataService<TSelectModel, TFilterModel>
    {
        Task<List<TSelectModel>> GetSelectListAsync(TFilterModel filter);
    }
}
