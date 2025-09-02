using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using {{ProjectName}}.Framework.Extensions;

namespace {{ProjectName}}.Framework;
 
   public abstract class ListingBase<TListViewModel, TListBusinessObject, TFilterViewModel, TFilterBusinessObject, TService> : FrameworkBaseComponent, INotifyPropertyChanged
    where TListViewModel : class
    where TListBusinessObject : class
    where TService : IListingDataService<TListBusinessObject, TFilterBusinessObject>
    where TFilterViewModel : BaseFilterViewModel, new()
    where TFilterBusinessObject : BaseFilterBusinessObject, new()
{
    [Parameter]
    public bool UsePagination { get; set; } = true;

    public string Error { get; set; } = string.Empty;
    public bool IsWorking { get; set; }

    private bool _itemsLoaded;
    public bool ItemsLoaded
    {
        get
        {
            return _itemsLoaded;
        }
        set
        {
            if (_itemsLoaded != value)
                _itemsLoaded = value;
        }
    }
    public int TotalPages { get; set; }
    public int TotalRows { get; set; }

    private TFilterViewModel _filterViewModel = new();
    public TFilterViewModel FilterViewModel
    {
        get { return _filterViewModel; }
        set { SetField(ref _filterViewModel, value); }
    }

    private TFilterBusinessObject _filterBusinessObject = new();
    public TFilterBusinessObject FilterBusinessObject
    {
        get { return _filterBusinessObject; }
        set { SetField(ref _filterBusinessObject, value); }
    }

    private List<TListViewModel> _items = new();
    public List<TListViewModel> Items
    {
        get { return _items; }
        set { SetField(ref _items, value); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    public async Task LoadItemsAsync()
    {
        IsWorking = true;
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<TService>();
            FilterBusinessObject = FilterViewModel.Clone<TFilterBusinessObject>();
            FilterBusinessObject.UsePagination = UsePagination;
            var pagedItems = await service.GetPaginatedItemsAsync(FilterBusinessObject);
            Items = ConvertListingBusinessItemsToListingViewModelItems(pagedItems.Items);
            TotalPages = pagedItems.TotalPages;
            TotalRows = pagedItems.TotalRows;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            ItemsLoaded = true;
            IsWorking = false;
        }
    }

    protected virtual List<TListViewModel> ConvertListingBusinessItemsToListingViewModelItems(List<TListBusinessObject>? items)
    {
        if (items == null || !items.Any())
        {
            return new List<TListViewModel>();
        }
        var json = JsonConvert.SerializeObject(items);
        var viewModelItems = JsonConvert.DeserializeObject<List<TListViewModel>>(json);
        return viewModelItems ?? new List<TListViewModel>();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadItemsAsync();
    }
}
