namespace {{ProjectName}}.Framework
{
    public interface IListingDataService<TListingModel, TFilterModel>
         where TFilterModel : BaseFilterBusinessObject
    {
        public Task<PagedDataList<TListingModel>> GetPaginatedItemsAsync(TFilterModel filterViewModel);
    }
      
    public class PagedDataList<TListViewModel>
    {
        public int TotalRows { get; set; }
        public int TotalPages { get; set; } 
        public List<TListViewModel>? Items { get; set; }
    }
}
