using Microsoft.EntityFrameworkCore;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.Server.Data
{
    public abstract class ListingServerDataService<TListingModel, TFilterBusinessObject> : IListingDataService<TListingModel, TFilterBusinessObject>
       where TFilterBusinessObject : BaseFilterBusinessObject
    {
        public async Task<PagedDataList<TListingModel>> GetPaginatedItemsAsync(TFilterBusinessObject filterBusinessObject)
        {
            return await MaterializeQueryAsync(filterBusinessObject, GetQuery(filterBusinessObject), GetTotalRows());
        }

        public async Task<PagedDataList<TListingModel>> MaterializeQueryAsync(TFilterBusinessObject filterBusinessObject, IQueryable<TListingModel> query, int totalRows)
        {
            var resultBusinessObject = new PagedDataList<TListingModel>();
            if (query != null)
            {
                if (filterBusinessObject.UsePagination)
                {
                    resultBusinessObject.TotalRows = totalRows;

                    if (resultBusinessObject.TotalRows == -1)
                        resultBusinessObject.TotalRows = query.Count();

                    resultBusinessObject.TotalPages = Convert.ToInt32(Math.Ceiling(resultBusinessObject.TotalRows / (double)filterBusinessObject.RowsPerPage));

                    query = query.Skip((filterBusinessObject.CurrentIndex - 1) * filterBusinessObject.RowsPerPage).Take(filterBusinessObject.RowsPerPage); 
                }

                if (query is IAsyncEnumerable<TListingModel>)
                {
                    resultBusinessObject.Items = await query.ToListAsync();
                }
                else
                {
                    resultBusinessObject.Items = query.ToList();
                }
            }
             
            return resultBusinessObject;
        }

        public abstract IQueryable<TListingModel> GetQuery(TFilterBusinessObject filterBusinessObject);

        public virtual int GetTotalRows()
        {
            return -1;
        }
    }
}
