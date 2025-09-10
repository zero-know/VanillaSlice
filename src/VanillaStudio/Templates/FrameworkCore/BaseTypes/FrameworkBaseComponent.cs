using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace {{ProjectName}}.Framework
{
    public class FrameworkBaseComponent : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public string? OperationId { get; set; }

        [Inject]
        protected IServiceScopeFactory ScopeFactory { get; set; } = null!;

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = null!;
      
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
