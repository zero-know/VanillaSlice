namespace {{ProjectName}}.Framework;

public interface IFormDataService<TFormBusinessModel, TKey>
{
    public Task<TFormBusinessModel> GetByIdAsync(TKey id);
    Task<TKey> CreateAsync(TFormBusinessModel formViewModel);

    Task<TKey> UpdateAsync(TKey id, TFormBusinessModel formViewModel);

    Task<int> DeleteAsync(TKey id);
}