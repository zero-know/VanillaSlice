using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using {{ProjectName}}.Framework.Services;

namespace {{ProjectName}}.Framework
{
    public class FrameworkBaseComponent : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public Guid? DialogId { get; set; }

        [Inject]
        protected IServiceScopeFactory ScopeFactory { get; set; } = null!;

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = null!;

        [Inject]
        public DialogService DialogService { get; set; } = null!;

        public void ShowDialog<TComponent, TKey>(string title, TKey? id, params (string Key, object? Value)[] parameters) where TComponent : IComponent
        {
            Action onclose = new Action(OnDialogClosed);
            DialogService?.ShowDialog<TComponent, TKey>(title, id, onclose, parameters);
        }

        public void CloseDialog()
        {
            DialogService.CloseDialog(DialogId);
        }

        protected (string Key, object? Value) P(object? value, [CallerArgumentExpression("value")] string? variableName = null)
        {
            if (variableName == null) throw new ArgumentNullException(nameof(variableName));
            return (variableName.Split('.').Last(), value);
        }


        public virtual void OnDialogClosed()
        {

        }
    }
}
