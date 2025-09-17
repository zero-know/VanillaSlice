using Microsoft.EntityFrameworkCore;
using {{ProjectName}}.Server.Data;
using {{ProjectName}}.ServiceContracts.Features.__moduleNamespace__;
using {{ProjectName}}.ServiceContracts.Models;

namespace {{ProjectName}}.Server.DataServices.Features.__moduleNamespace__;

internal class __ComponentPrefix__SelectListServerDataService : I__ComponentPrefix__SelectListDataService
{
    private readonly AppDbContext _context;

    public __ComponentPrefix__SelectListServerDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectOption<__selectListDataType__>>> GetSelectListAsync(__ComponentPrefix__SelectListFilterBusinessModel filter)
    {
        // TODO: Replace with actual entity query
        // Example: var query = _context.YourEntities.AsQueryable();
        // 
        // if (!string.IsNullOrEmpty(filter.SearchTerm))
        // {
        //     query = query.Where(x => x.Name.Contains(filter.SearchTerm));
        // }
        // 
        // return await query
        //     .Select(x => new SelectOption<__selectListDataType__>(x.Id, x.Name))
        //     .ToListAsync();
        
        throw new NotImplementedException("Implement the query to return select list data from your entity");
    }
}
