using VanillaSlice.Bootstrapper.Models;
using System.Text;

namespace VanillaSlice.Bootstrapper.Services
{
    public class FrameworkCoreGenerator
    {
        public List<GeneratedFile> GenerateCompleteFrameworkCoreProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // {ProjectName}.Framework.csproj
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/{config.ProjectName}.Framework.csproj",
                Content = GenerateFrameworkCoreProjectFile(config),
                Type = FileType.ProjectFile
            });

            // Base Types
            files.AddRange(GenerateBaseTypes(config));

            // Interfaces
            files.AddRange(GenerateInterfaces(config));

            // Extensions
            files.AddRange(GenerateExtensions(config));

            // Utils
            files.AddRange(GenerateUtils(config));

            return files;
        }

        private string GenerateFrameworkCoreProjectFile(ProjectConfiguration config)
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk.Razor"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <SupportedPlatform Include=""browser"" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.Extensions.DependencyInjection.Abstractions"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.Extensions.Logging.Abstractions"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.JSInterop"" Version=""9.0.8"" />
    <PackageReference Include=""Newtonsoft.Json"" Version=""13.0.3"" />
    <PackageReference Include=""MudBlazor"" Version=""7.8.0"" />
  </ItemGroup>

</Project>";
        }

        private List<GeneratedFile> GenerateBaseTypes(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // ObservableBase
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/ObservableBase.cs",
                Content = @"using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace " + config.ProjectName + @".Framework;

public class ObservableBase : INotifyPropertyChanged
{
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = """")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}",
                Type = FileType.CSharpCode
            });

            // BaseFilterBusinessObject
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/BaseFilterBusinessObject.cs",
                Content = @"namespace " + config.ProjectName + @".Framework
{
    public class BaseFilterBusinessObject
    {
        public int CurrentIndex { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public bool UsePagination { get; set; } = true;
        public string? SearchKey { get; set; }
    }
}",
                Type = FileType.CSharpCode
            });

            // BaseFilterViewModel
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/BaseFilterViewModel.cs",
                Content = GenerateBaseFilterViewModelContent(config),
                Type = FileType.CSharpCode
            });

            // FrameworkBaseComponent
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/FrameworkBaseComponent.cs",
                Content = GenerateFrameworkBaseComponentContent(config),
                Type = FileType.CSharpCode
            });

            // ServerSideListingDataService
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/ServerSideListingDataService.cs",
                Content = GenerateServerSideListingDataServiceContent(config),
                Type = FileType.CSharpCode
            });

            // ListingBase
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/ListingBase.cs",
                Content = GenerateListingBaseContent(config),
                Type = FileType.CSharpCode
            });

            // FormBase
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/BaseTypes/FormBase.cs",
                Content = GenerateFormBaseContent(config),
                Type = FileType.CSharpCode
            });

            return files;
        }

        private List<GeneratedFile> GenerateInterfaces(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // IListingDataService
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/Interfaces/IListingDataService.cs",
                Content = @"namespace " + config.ProjectName + @".Framework
{
    public interface IListingDataService<TListingModel, TFilterModel>
         where TFilterModel : BaseFilterBusinessObject
    {
        public Task<PagedDataList<TListingModel>> GetPaginatedItems(TFilterModel filterViewModel);
    }
      
    public class PagedDataList<TListViewModel>
    {
        public int TotalRows { get; set; }
        public int TotalPages { get; set; } 
        public List<TListViewModel>? Items { get; set; }
    }
}",
                Type = FileType.CSharpCode
            });

            // IFormDataService
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/Interfaces/IFormDataService.cs",
                Content = @"namespace " + config.ProjectName + @".Framework
{
    public interface IFormDataService<TFormModel, TKey>
    {
        public Task<TFormModel> GetItemByIdAsync(TKey id);
        Task<TKey> SaveAsync(TFormModel formViewModel);
    }
}",
                Type = FileType.CSharpCode
            });

            // ILocalStorageService
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/Interfaces/ILocalStorageService.cs",
                Content = @"namespace " + config.ProjectName + @".Framework
{
    public interface ILocalStorageService
    {
        Task<string> GetValue(string key);
        Task SetValue(string value, string key);
        Task RemoveValue(string key);
    }

    public class BrowserLocalStorageService : ILocalStorageService
    {
        public async Task<string> GetValue(string key)
        {
            // Implementation for browser local storage
            await Task.CompletedTask;
            return string.Empty;
        }

        public async Task SetValue(string value, string key)
        {
            // Implementation for browser local storage
            await Task.CompletedTask;
        }

        public async Task RemoveValue(string key)
        {
            // Implementation for browser local storage
            await Task.CompletedTask;
        }
    }
}",
                Type = FileType.CSharpCode
            });

            return files;
        }

        private List<GeneratedFile> GenerateExtensions(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Extensions for query string conversion
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/Extensions/ObjectExtensions.cs",
                Content = @"using Newtonsoft.Json;
using System.Reflection;
using System.Web;

namespace " + config.ProjectName + @".Framework.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetValue(obj) != null);

            var queryString = string.Join(""&"", properties.Select(p =>
                $""{HttpUtility.UrlEncode(p.Name)}={HttpUtility.UrlEncode(p.GetValue(obj)?.ToString())}""));

            return string.IsNullOrEmpty(queryString) ? string.Empty : $""?{queryString}"";
        }

        public static T Clone<T>(this object source) where T : new()
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json) ?? new T();
        }
    }
}",
                Type = FileType.CSharpCode
            });

            return files;
        }

        private List<GeneratedFile> GenerateUtils(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // SafeConvert utility
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.Framework/Utils/SafeConvert.cs",
                Content = @"namespace " + config.ProjectName + @".Framework.Utils
{
    public static class SafeConvert
    {
        public static int ToInt32(string? value, int defaultValue = 0)
        {
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public static long ToInt64(string? value, long defaultValue = 0)
        {
            return long.TryParse(value, out var result) ? result : defaultValue;
        }

        public static decimal ToDecimal(string? value, decimal defaultValue = 0)
        {
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }

        public static bool ToBoolean(string? value, bool defaultValue = false)
        {
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        public static DateTime ToDateTime(string? value, DateTime? defaultValue = null)
        {
            return DateTime.TryParse(value, out var result) ? result : defaultValue ?? DateTime.MinValue;
        }
    }
}",
                Type = FileType.CSharpCode
            });

            return files;
        }

        // Helper methods for generating complex content
        private string GenerateBaseFilterViewModelContent(ProjectConfiguration config)
        {
            return @"namespace " + config.ProjectName + @".Framework
{
    public class BaseFilterViewModel : ObservableBase
    {
        public int CurrentIndex { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public bool UsePagination { get; set; } = true;

        private int _eventId;
        public int EventId
        {
            get { return _eventId; }
            set { SetField(ref _eventId, value); }
        }

        private int _naId;
        public int NaId
        {
            get { return _naId; }
            set { SetField(ref _naId, value); }
        }

        private int _paId;
        public int PaId
        {
            get { return _paId; }
            set { SetField(ref _paId, value); }
        }

        private int _ucId;
        public int UcId
        {
            get { return _ucId; }
            set { SetField(ref _ucId, value); }
        }

        private string? _searchKey;
        public string? SearchKey
        {
            get { return _searchKey; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    CurrentIndex = 1;
                SetField(ref _searchKey, value);
            }
        }
    }
}";
        }

        private string GenerateFrameworkBaseComponentContent(ProjectConfiguration config)
        {
            return @"using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace " + config.ProjectName + @".Framework
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
}";
        }

        // Additional helper methods will be added in the next part
        private string GenerateServerSideListingDataServiceContent(ProjectConfiguration config)
        {
            return @"using Microsoft.EntityFrameworkCore;

namespace " + config.ProjectName + @".Framework
{
    public abstract class ServerSideListingDataService<TListingModel, TFilterBusinessObject> : IListingDataService<TListingModel, TFilterBusinessObject>
       where TFilterBusinessObject : BaseFilterBusinessObject
    {
        public async Task<PagedDataList<TListingModel>> GetPaginatedItems(TFilterBusinessObject filterBusinessObject)
        {
            return await MaterializeQueryAsync(filterBusinessObject, GetQuery(filterBusinessObject), GetTotalRows());
        }

        public async Task<PagedDataList<TListingModel>> MaterializeQueryAsync(TFilterBusinessObject filterBusinessObject, IQueryable<TListingModel> query, int totalRows)
        {
            var resultBusinessObject = new PagedDataList<TListingModel>();
            if (query != null)
            {
                if (filterBusinessObject.UsePagination)
                {
                    resultBusinessObject.TotalRows = totalRows;

                    if (resultBusinessObject.TotalRows == -1)
                        resultBusinessObject.TotalRows = query.Count();

                    resultBusinessObject.TotalPages = Convert.ToInt32(Math.Ceiling(resultBusinessObject.TotalRows / (double)filterBusinessObject.RowsPerPage));

                    query = query.Skip((filterBusinessObject.CurrentIndex - 1) * filterBusinessObject.RowsPerPage).Take(filterBusinessObject.RowsPerPage); 
                }

                if (query is IAsyncEnumerable<TListingModel>)
                {
                    resultBusinessObject.Items = await query.ToListAsync();
                }
                else
                {
                    resultBusinessObject.Items = query.ToList();
                }
            }
             
            return resultBusinessObject;
        }

        public abstract IQueryable<TListingModel> GetQuery(TFilterBusinessObject filterBusinessObject);

        public virtual int GetTotalRows()
        {
            return -1;
        }
    }
}";
        }

        private string GenerateListingBaseContent(ProjectConfiguration config)
        {
            // This will be a simplified version - the full ListingBase is quite complex
            return @"using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace " + config.ProjectName + @".Framework
{
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
        public bool IsBusy { get; set; }
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
            IsBusy = true;
            try
            {
                using var scope = ScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                FilterBusinessObject = FilterViewModel.Clone<TFilterBusinessObject>();
                FilterBusinessObject.UsePagination = UsePagination;
                FilterBusinessObject.EventId = EventId;
                
                var pagedItems = await service.GetPaginatedItems(FilterBusinessObject);
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
                IsBusy = false;
            }
        }

        protected abstract List<TListViewModel> ConvertListingBusinessItemsToListingViewModelItems(List<TListBusinessObject>? items);

        protected override async Task OnInitializedAsync()
        {
            await LoadItemsAsync();
        }
    }
}";
        }

        private string GenerateFormBaseContent(ProjectConfiguration config)
        {
            // Simplified FormBase implementation
            return @"using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace " + config.ProjectName + @".Framework
{
    public abstract class FormBase<TFormModel, TFormViewModel, TKey, TService> : FrameworkBaseComponent, INotifyPropertyChanged
          where TFormModel : class, new()
          where TFormViewModel : class, INotifyPropertyChanged, new()
          where TService : IFormDataService<TFormModel, TKey>
    {
        [SupplyParameterFromQuery]
        [Parameter]
        public TKey? Id { get; set; }

        [Parameter]
        public bool KeepAlive { get; set; } = false;

        [Inject]
        protected ILogger<TService> Logger { get; set; } = null!;

        public string? ValidationError { get; set; }

        private TFormViewModel _selectedItem = new();
        public TFormViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetField(ref _selectedItem, value); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (Id == null || Id.ToString() == ""0"" || string.IsNullOrEmpty(Id.ToString()))
                {
                    SelectedItem = await CreateSelectedItem();
                }
                else
                {
                    using var scope = ScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<TService>();
                    var formModel = await service.GetItemByIdAsync(Id);
                    SelectedItem = await ConvertBusinessModelToViewModel<TFormViewModel>(formModel);
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
        }

        protected abstract Task<TFormViewModel> CreateSelectedItem();
        protected abstract Task<T> ConvertBusinessModelToViewModel<T>(TFormModel model) where T : class, new();

        public async Task<bool> SaveAsync()
        {
            try
            {
                using var scope = ScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                var businessModel = await ConvertViewModelToBusinessModel(SelectedItem);
                await service.SaveAsync(businessModel);
                return true;
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                return false;
            }
        }

        protected abstract Task<TFormModel> ConvertViewModelToBusinessModel(TFormViewModel viewModel);
    }
}";
        }
    }
}
