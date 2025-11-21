using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;

namespace {{ProjectName}}.Framework.Services
{
    /// <summary>
    /// Service for managing application dialogs
    /// </summary>
    public class DialogService
    {
        private readonly BlockingCollection<DialogOptions> _dialogs = new();
        private int _nextZIndex = 1000;

        /// <summary>
        /// Gets the collection of active dialogs
        /// </summary>
        public IEnumerable<DialogOptions> Dialogs => _dialogs;

        /// <summary>
        /// Event raised when dialogs collection changes
        /// </summary>
        public event Action? OnDialogsChanged;

        /// <summary>
        /// Shows a dialog with the specified component type
        /// </summary>
        /// <typeparam name="TComponent">The component type to display</typeparam>
        /// <param name="title">The dialog title</param>
        /// <param name="id">The record ID to pass to the component</param>
        /// <param name="parameters">Additional parameters to pass to the component</param>
        /// <returns>The dialog options for the created dialog</returns>
        public DialogOptions ShowDialog<TComponent>(string title, object? id = null, params (string Key, object Value)[] parameters) where TComponent : IComponent
        {
            var dialogOptions = new DialogOptions
            {
                Id = Guid.NewGuid(),
                Title = title,
                ComponentType = typeof(TComponent),
                ZIndex = _nextZIndex++,
                Parameters = new Dictionary<string, object>()
            };

            // Add the ID parameter if provided
            if (id != null)
            {
                dialogOptions.Parameters["Id"] = id;
            }

            // Add additional parameters
            if (parameters != null)
            {
                foreach (var (key, value) in parameters)
                {
                    dialogOptions.Parameters[key] = value;
                }
            }

            _dialogs.Add(dialogOptions);
            OnDialogsChanged?.Invoke();

            return dialogOptions;
        }

        /// <summary>
        /// Shows a dialog with the specified component type using dictionary parameters
        /// </summary>
        /// <typeparam name="TComponent">The component type to display</typeparam>
        /// <param name="title">The dialog title</param>
        /// <param name="id">The record ID to pass to the component</param>
        /// <param name="parameters">Additional parameters to pass to the component</param>
        /// <returns>The dialog options for the created dialog</returns>
        public DialogOptions ShowDialog<TComponent>(string title, object? id, Dictionary<string, object>? parameters = null) where TComponent : IComponent
        {
            var dialogOptions = new DialogOptions
            {
                Id = Guid.NewGuid(),
                Title = title,
                ComponentType = typeof(TComponent),
                ZIndex = _nextZIndex++,
                Parameters = parameters ?? new Dictionary<string, object>()
            };

            // Add the ID parameter if provided
            if (id != null)
            {
                dialogOptions.Parameters["Id"] = id;
            }

            _dialogs.Add(dialogOptions);
            OnDialogsChanged?.Invoke();

            return dialogOptions;
        }

        /// <summary>
        /// Closes a specific dialog
        /// </summary>
        /// <param name="dialogId">The ID of the dialog to close</param>
        public void CloseDialog(Guid dialogId)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                _dialogs.TryTake(out _);
                OnDialogsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Closes all open dialogs
        /// </summary>
        public void CloseAllDialogs()
        {
            while (_dialogs.Count > 0)
            {
                _dialogs.TryTake(out _);
            }
            OnDialogsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Options for configuring a dialog
    /// </summary>
    public class DialogOptions
    {
        /// <summary>
        /// Unique identifier for this dialog
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title to display in the dialog header
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The component type to render in the dialog
        /// </summary>
        public Type ComponentType { get; set; } = null!;

        /// <summary>
        /// The z-index for layering dialogs
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Parameters to pass to the component
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Whether the dialog can be closed by clicking outside
        /// </summary>
        public bool CloseOnBackdropClick { get; set; } = true;

        /// <summary>
        /// Custom width for the dialog (e.g., "600px", "80%", "max-w-4xl")
        /// </summary>
        public string? Width { get; set; }

        /// <summary>
        /// Custom height for the dialog (e.g., "400px", "80vh")
        /// </summary>
        public string? Height { get; set; }
    }
}
