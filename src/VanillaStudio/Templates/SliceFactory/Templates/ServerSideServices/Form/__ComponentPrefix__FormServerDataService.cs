using Microsoft.EntityFrameworkCore;
using {{ProjectName}}.Framework;
using {{ProjectName}}.Server.Data;
using {{ProjectName}}.ServiceContracts.Features.__moduleNamespace__;

namespace {{ProjectName}}.Server.DataServices.Features.__moduleNamespace__;

internal class __ComponentPrefix__FormServerDataService : I__ComponentPrefix__FormDataService
{
    private readonly AppDbContext _context;

    public __ComponentPrefix__FormServerDataService(AppDbContext context)
    {
        _context = context;
    }

    public Task<__ComponentPrefix__FormBusinessModel> GetByIdAsync(__primaryKeyType__ id)
    {
        throw new NotImplementedException();
    }

    public Task<__primaryKeyType__> CreateAsync(__ComponentPrefix__FormBusinessModel model)
    {
        throw new NotImplementedException();
    }

    public Task<__primaryKeyType__> UpdateAsync(__primaryKeyType__ id, __ComponentPrefix__FormBusinessModel model)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(__primaryKeyType__ id)
    {
        throw new NotImplementedException();
    }
}
