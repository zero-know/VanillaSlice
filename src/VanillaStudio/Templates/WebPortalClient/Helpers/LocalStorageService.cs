using Microsoft.JSInterop;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.WebPortal.Client
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime jSRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }
        public async Task<string> GetValue(string key)
        {
           return await jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }

        public async Task RemoveValue(string key)
        {
            await jSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task SetValue(string value, string key)
        {
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
    }
}
