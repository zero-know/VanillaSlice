namespace {{ProjectName}}.Framework
{
    public interface ILocalStorageService
    {
        Task<string?> GetValue(string key);
        Task SetValue(string value, string key);
        Task RemoveValue(string key);
    }
}
