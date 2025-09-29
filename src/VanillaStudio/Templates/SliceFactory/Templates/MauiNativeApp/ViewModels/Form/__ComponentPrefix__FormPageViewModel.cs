using {{RootNamespace}}.ClientShared.Features.__ComponentPrefix__;
using {{RootNamespace}}.MauiShared;
using {{RootNamespace}}.ServiceContracts.Features.__ComponentPrefix__;

namespace {{RootNamespace}}.MauiNativeApp.ViewModels.__ComponentPrefix__;

public partial class __ComponentPrefix__FormPageViewModel : FormBaseMaui<__ComponentPrefix__FormViewModel, __ComponentPrefix__FormBusinessModel, I__ComponentPrefix__FormDataService>
{
    public __ComponentPrefix__FormPageViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        Title = "__ComponentPrefix__ Details";
    }
}