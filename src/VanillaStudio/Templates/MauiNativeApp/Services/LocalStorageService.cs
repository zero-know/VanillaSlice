using {{ProjectName}}.Framework;
using System.Runtime.CompilerServices;

namespace {{ProjectName}}.NativeMauiApp.Services;

public class LocalStorageService : ILocalStorageService
{
    public async Task<string?> GetValue([CallerMemberName] string memberName = "")
    {
        return await SecureStorage.Default.GetAsync($"__{memberName}__");
    }

    public async Task SetValue(string? value, [CallerMemberName] string memberName = "")
    {
        if (string.IsNullOrEmpty(value))
        {
            SecureStorage.Default.Remove($"__{memberName}__");
        }
        else
        {
            await SecureStorage.Default.SetAsync($"__{memberName}__", value);
        }
    }

    public Task RemoveValue([CallerMemberName] string memberName = "")
    {
        SecureStorage.Default.Remove($"__{memberName}__");
        return Task.CompletedTask;
    }
}