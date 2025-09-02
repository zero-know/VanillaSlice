using Microsoft.EntityFrameworkCore;
using {{ProjectName}}.Framework;
using {{ProjectName}}.Server.Data;
using {{ProjectName}}.ServiceContracts.Features.__moduleNamespace__;

namespace {{ProjectName}}.Server.DataServices.Features.__moduleNamespace__;

internal class __ComponentPrefix__ListingServerDataService : ListingServerDataService<__ComponentPrefix__ListingBusinessModel, __ComponentPrefix__FilterBusinessModel>, I__ComponentPrefix__ListingDataService
{
    private readonly AppDbContext _context;

    public __ComponentPrefix__ListingServerDataService(AppDbContext context)
    {
        _context = context;
    }

    public override IQueryable<__ComponentPrefix__ListingBusinessModel> GetQuery(__ComponentPrefix__FilterBusinessModel filter)
    {
        // pull data from database using filter, do not use pagination, do not materialize the result
        throw new NotImplementedException();
    }
}
