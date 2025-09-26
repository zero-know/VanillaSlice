using {{ProjectName}}.Framework;
using {{ProjectName}}.Framework.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;
#if ANDROID
using Microsoft.Maui.Platform; // Add this namespace for ToPlatform extension method  
#endif

namespace {{ProjectName}}.MauiShared
{
    public abstract class FormBaseMaui<TFormModel, TFormViewModel, TKey, TFormDataService> : ObservableBase
         where TFormModel : class, new()
         where TFormViewModel : class, INotifyPropertyChanged, new()
         where TFormDataService : IFormDataService<TFormModel, TKey>
    {
        private TKey? _recordId;
        protected ILogger<TFormDataService> Logger { get; }
        private TFormViewModel _model = new TFormViewModel();

        public TFormViewModel Model
        {
            get => _model;
            set
            {
                _model = value;
                if (_model != null)
                {
                    _model.PropertyChanged += Model_PropertyChanged;
                }
                NotifyPropertyChanged();
            }
        }

        private bool _isWorking = true;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking != value)
                {
                    _isWorking = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string? _error;

        public string?Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool hasError;
        public bool HasError
        {
            get => hasError;
            set
            {
                hasError = value;
                NotifyPropertyChanged();
            }
        }

        public IServiceScopeFactory ScopeFactory { get; }

        protected FormBaseMaui(IServiceScopeFactory scopeFactory, TKey? recordId = default)
        {
            if (!EqualityComparer<TKey>.Default.Equals(recordId, default))
                _recordId = recordId;

            PropertyChanged += FormBase_PropertyChanged;

            ScopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            using var scope = scopeFactory.CreateScope();

            Logger = scope.ServiceProvider.GetRequiredService<ILogger<TFormDataService>>();
             
            Task.Run(async () =>
            {
                try
                {
                    using var scope = ScopeFactory.CreateScope();
                    await LoadSelectLists(scope).ConfigureAwait(false);

                    if (IsNewRecord(_recordId))
                    {
                        var newModel = await CreateModel().ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => Model = newModel);
                    }
                    else
                    {
                        var crudService = scope.ServiceProvider.GetRequiredService<TFormDataService>();
                        var formModel = await crudService.GetByIdAsync(_recordId!).ConfigureAwait(false)
                            ?? throw new InvalidDataException(" Model is null after service.GetByIdAsync");

                        var viewModel = await ConvertBusinessModelToViewModel(formModel).ConfigureAwait(false)
                            ?? throw new InvalidDataException("Model not found");

                        MainThread.BeginInvokeOnMainThread(() => Model = viewModel);
                    }

                    MainThread.BeginInvokeOnMainThread(() => IsWorking = false);
                }
                catch (Exception ex)
                {
                    LogAndDisplayError(ex);
                }
            });
        }

        private void FormBase_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model) && Model != null)
            {
                OnModelCreated(Model);
            }
        }
      
 

        protected virtual void OnModelCreated(TFormViewModel model) { }



        private static bool IsNewRecord(TKey? id) =>
            id == null || id.ToString() is "0" or "" or "00000000-0000-0000-0000-000000000000";

        protected virtual Task<TFormViewModel?> ConvertBusinessModelToViewModel(TFormModel formModel)
        {
            return formModel == null
                ? throw new ArgumentNullException(nameof(formModel), "formModel is null while cloning")
                : Task.FromResult(formModel.Clone<TFormViewModel?>());
        }

        public virtual void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

        protected virtual Task LoadSelectLists(IServiceScope scope) => Task.CompletedTask;

        protected virtual Task<TFormViewModel> CreateModel() => Task.FromResult(new TFormViewModel());

        public virtual Task BeforeSaveAsync() => Task.CompletedTask;

        public virtual Task OnAfterSaveAsync(TKey key) => Task.CompletedTask;

        protected virtual TFormModel ConvertViewModelToBusinessModel(TFormViewModel? formViewModel)
        {
            return formViewModel == null
                ? throw new ArgumentNullException(nameof(formViewModel), "ViewModel cannot be null")
                : formViewModel.Clone<TFormModel>();
        }

        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new Command(async () =>
                    {
                        await HandleFormSubmit(null!, null!);
                    });
                }
                return _saveCommand;
            }
        }

        public async Task HandleFormSubmit(object sender, EventArgs e)
        {
            try
            {
                Logger.LogInformation("Form submitted");
                await MainThread.InvokeOnMainThreadAsync(() => IsWorking = true);
                await Task.Delay(100);
                using var scope = ScopeFactory.CreateScope();
                var formService = scope.ServiceProvider.GetRequiredService<TFormDataService>();

                //if (Model is IValidateable validateable)
                //{
                //    validateable.Validate();
                //}

                var businessModel = ConvertViewModelToBusinessModel(Model)
                    ?? throw new InvalidDataException("Business model is null before calling SaveAsync");

                await BeforeSaveAsync();
                TKey id = default!;
                await BeforeSaveAsync();
                if (EqualityComparer<TKey>.Default.Equals(_recordId, default))
                {
                    id = await formService.CreateAsync(businessModel);
                }
                else
                {
                    id = await formService.UpdateAsync(_recordId!, businessModel);
                }
                await OnAfterSaveAsync(id);
            }
            catch (Exception ex)
            {
                LogAndDisplayError(ex);
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsWorking = false);
            }
        }

        private void LogAndDisplayError(Exception ex)
        {
            Logger.LogError(ex, "An error occurred");
            //Crashes.TrackError(ex);
            Error = ex.Message;
        }

    }


}