using {{ProjectName}}.Framework;
using {{ProjectName}}.Framework.Extensions;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Windows.Input;


namespace {{ProjectName}}.MauiShared
{
    public abstract class ListingBaseMaui<TListViewModel, TListBusinessObject, TFilterViewModel, TFilterBusinessObject, TService> : ObservableBase
    where TListViewModel : class
    where TListBusinessObject : class
    where TService : IListingDataService<TListBusinessObject, TFilterBusinessObject>
    where TFilterViewModel : BaseFilterViewModel, new()
    where TFilterBusinessObject : BaseFilterBusinessObject, new()

    {
        #region Index Properties
        public int TotalPages { get; set; }


        private string? _sortColumn;

        public string? SortColumn
        {
            get { return _sortColumn; }
            set
            {
                _sortColumn = value;
                NotifyPropertyChanged();
            }
        }


        private int _totalRows;
        public int TotalRows
        {
            get
            {
                return _totalRows;
            }
            set
            {
                if (_totalRows != value)
                    _totalRows = value;
            }
        }

        #endregion

        public TFilterViewModel FilterViewModel { get; set; } = new TFilterViewModel();
        public TFilterBusinessObject FilterBusinessObject { get; set; } = new TFilterBusinessObject();

        private ObservableCollection<TListViewModel> _items = new ObservableCollection<TListViewModel>();
        public ObservableCollection<TListViewModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isWorking;

        public bool IsWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; }
        }


        //protected virtual PaginationStripModel PaginationStrip { get; set; }

        private string? _error;

        public string? Error
        {
            get { return _error; }
            set
            {
                _error = value;
                NotifyPropertyChanged();
            }
        }


        public virtual int RowsPerPage { get; set; } = 10;

        public virtual bool UsePagination => true;
        public string _conponentId = Guid.NewGuid().ToString().Replace("-", "");

        public IServiceScopeFactory ScopeFactory { get; set; }

        public ListingBaseMaui(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
            FilterViewModel = new TFilterViewModel();

            //PaginationStrip = new PaginationStripModel()
            //{
            //    CurrentIndex = 1,
            //    RowsPerPage = RowsPerPage
            //};

            using var scope = ScopeFactory.CreateScope();
            _ = LoadSelectLists(scope);
            _ = LoadItems();
            //PaginationStrip.PropertyChanged += (p, q) =>
            //{
            //    if (q.PropertyName == nameof(PaginationStrip.RowsPerPage)
            //    || q.PropertyName == nameof(PaginationStrip.CurrentIndex)
            //    || q.PropertyName == nameof(SortColumn))
            //    {

            //        if (PaginationStrip.CurrentIndex < 0)
            //        {
            //            PaginationStrip.CurrentIndex = 1;
            //        }
            //        else if (TotalPages > 0 && PaginationStrip.CurrentIndex > TotalPages)
            //        {
            //            PaginationStrip.CurrentIndex = TotalPages;
            //        }

            //        FilterViewModel.RowsPerPage = PaginationStrip.RowsPerPage;
            //        FilterViewModel.CurrentIndex = TotalRows <= PaginationStrip.RowsPerPage ? 1 : PaginationStrip.CurrentIndex;

            //        _ = LoadItems();
            //    }
            //};
        }

        protected virtual Task LoadSelectLists(IServiceScope selectListService)
        {
            return Task.CompletedTask;
        }


        protected virtual async Task ItemsLoaded(TService service)
        {
            await Task.CompletedTask;
        }

        public void PageChanged(int i)
        {
            //PaginationStrip.CurrentIndex = i;
        }

        protected virtual TListViewModel[] ConvertListingBusinessItemsToListingViewModelItems(List<TListBusinessObject> listBusinessObjects)
        {
            var json = JsonConvert.SerializeObject(listBusinessObjects);
            return JsonConvert.DeserializeObject<TListViewModel[]>(json) ?? new TListViewModel[0];
        }

        public async Task LoadItems(bool showLoader = true)
        {

            if (showLoader)
            {

                MainThread.BeginInvokeOnMainThread(() =>
               {
                   IsWorking = true;
               });
            }

            using (var scope = ScopeFactory.CreateScope())
            {
                Error = string.Empty;
                try
                {
                    var crudService = scope.ServiceProvider.GetRequiredService<TService>();
                    FilterBusinessObject = FilterViewModel.Clone<TFilterBusinessObject>();
                    FilterBusinessObject.UsePagination = UsePagination;
                    var pagedItems = await crudService.GetPaginatedItemsAsync(FilterBusinessObject) ?? throw new Exception("Paginaned items object is null");
                    if (pagedItems == null || pagedItems.Items == null)
                        throw new Exception("Fetched data has no records");


                    var viewModelItems = ConvertListingBusinessItemsToListingViewModelItems(pagedItems.Items);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Items.Clear();
                        TotalPages = pagedItems.TotalPages;
                        TotalRows = pagedItems.TotalRows;
                        foreach (var item in viewModelItems)
                        {
                            Items.Add(item);
                        }
                    });

                    await ItemsLoaded(crudService);
                }
                catch (UnauthorizedAccessException)
                {
                    _ = Shell.Current.GoToAsync("//signin");
                }

                catch (Exception ex)
                {
                    //Crashes.TrackError(ex);
                    if (ex.Message.Contains("invalid_token"))
                    {
                        //todo
                        // NavigationManager.NavigateTo("/identity/account/login");
                    }
                    Error = ex.Message;

                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsWorking = false;
                });
            }


            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsWorking = false;
            });
            await OnItemsRendered();
        }

        private ICommand? _loadItemsCommand;
        public ICommand? LoadItemsCommand
        {
            get
            {
                return _loadItemsCommand = _loadItemsCommand ?? new Command(async () =>
                {
                    await LoadItems();
                });
            }
        }

        public ClaimsPrincipal? User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                //var authenticatedUser = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IAuthenticatedUser>();
                //if (authenticatedUser != null && _user != null)
                //{
                //    authenticatedUser.UserId = _user.GetUserId();
                //    authenticatedUser.UserName = _user.GetUsername();
                //    authenticatedUser.Roles = _user.GetRoles();
                //}
            }
        }

        private ClaimsPrincipal? _user;
        protected virtual Task OnItemsRendered() { return Task.CompletedTask; }

         
    }
}
