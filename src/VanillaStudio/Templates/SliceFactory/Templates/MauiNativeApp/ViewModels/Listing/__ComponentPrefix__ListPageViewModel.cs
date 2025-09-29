using {{RootNamespace}}.ClientShared.Features.__ComponentPrefix__;
using {{RootNamespace}}.MauiShared;
using {{RootNamespace}}.ServiceContracts.Features.__ComponentPrefix__;

namespace {{RootNamespace}}.MauiNativeApp.ViewModels.__ComponentPrefix__;

public partial class __ComponentPrefix__ListPageViewModel : ListingBaseMaui<__ComponentPrefix__ListingViewModel, __ComponentPrefix__ListingBusinessModel,
                                __ComponentPrefix__FilterViewModel, __ComponentPrefix__FilterBusinessModel, I__ComponentPrefix__ListingDataService>
{
    public __ComponentPrefix__ListPageViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        Title = "__ComponentPrefix__s";
    }
}