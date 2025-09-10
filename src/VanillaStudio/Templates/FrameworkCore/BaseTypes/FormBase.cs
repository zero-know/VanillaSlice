using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace {{ProjectName}}.Framework;

public abstract class FormBase<TFormViewModel, TFormBusinessObject, TKey, TService> : FrameworkBaseComponent, INotifyPropertyChanged
      where TFormBusinessObject : class, new()
      where TFormViewModel : class, INotifyPropertyChanged, new()
      where TService : IFormDataService<TFormBusinessObject, TKey>
{
    [SupplyParameterFromQuery]
    [Parameter]
    public TKey? Id { get; set; }

    [Parameter]
    public bool KeepAlive { get; set; } = false;

    [Inject]
    protected ILogger<TService> Logger { get; set; } = null!;
    public bool IsWorking { get; set; }
    public bool IsEditMode => !EqualityComparer<TKey>.Default.Equals(Id, default);
    public string? ErrorMessage { get; set; }

    private TFormViewModel _selectedItem = new();

    [SupplyParameterFromForm]
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

    private bool IsPostRequestOnServerSide()
    {
        if (!OperatingSystem.IsBrowser())
        {
            var httpContextAccessor = ScopeFactory?.CreateScope().ServiceProvider.GetService<IHttpContextAccessor>();
            return httpContextAccessor?.HttpContext.Request.Method == "POST";
        }
        return false;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsWorking = true;
            using var scope = ScopeFactory!.CreateScope();
            if (SelectedItem == null || !IsPostRequestOnServerSide())
            {
                if (Id == null || Id.ToString() == "0" || string.IsNullOrEmpty(Id.ToString()) || Id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    SelectedItem = await CreateSelectedItem();
                }
                else
                {
                    var crudService = scope.ServiceProvider.GetRequiredService<TService>();
                    var formModel = await crudService.GetByIdAsync(Id);
                    if (formModel == null)
                        throw new InvalidDataException("Selected Item is null after service.GetItemByIdAsync");

                    SelectedItem = await ConvertBusinessModelToViewModel<TFormViewModel>(formModel);

                    if (SelectedItem == null)
                        throw new InvalidDataException("Selected Item is null after cloning SelectedItem");
                } 
            }
            await OnSelectedItemInitialized(scope); 

        }
        catch (UnauthorizedAccessException)
        {
            NavigationManager?.NavigateTo($"/identity/account/login");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogInformation(ex?.Message);
        }
        IsWorking = false;
        await base.OnInitializedAsync();
    }

    protected virtual Task OnSelectedItemInitialized(IServiceScope scope)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<TFormViewModel> CreateSelectedItem()
    {
        return Task.FromResult(new TFormViewModel());
    }

    protected virtual Task<T> ConvertBusinessModelToViewModel<T>(TFormBusinessObject model) where T : class, new()
    {
        var json = JsonConvert.SerializeObject(model);
        var viewModel = JsonConvert.DeserializeObject<T>(json);
        return Task.FromResult(viewModel ?? new T());
    }

    public async Task SaveAsync()
    {
        TKey id = default!;
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<TService>();
            var selectedItem = BeforeSaveAsync(SelectedItem);
            var businessModel = await ConvertViewModelToBusinessModel(SelectedItem);
            if (EqualityComparer<TKey>.Default.Equals(Id, default))
            {
                id = await service.CreateAsync(businessModel);
            }
            else
            {
                id = await service.UpdateAsync(Id!, businessModel);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            if (!EqualityComparer<TKey>.Default.Equals(id, default))
            {
                await OnAfterSaveAsync(id);
            }
        }
    }

    public virtual Task<TFormViewModel> BeforeSaveAsync(TFormViewModel selectedItem)
    {

        return Task.FromResult(selectedItem);
    }

    public virtual Task OnAfterSaveAsync(TKey key)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<TFormBusinessObject> ConvertViewModelToBusinessModel(TFormViewModel viewModel)
    {
        var json = JsonConvert.SerializeObject(viewModel);
        var businessModel = JsonConvert.DeserializeObject<TFormBusinessObject>(json);
        return Task.FromResult(businessModel ?? new TFormBusinessObject());
    }
}

