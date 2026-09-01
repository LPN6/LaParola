using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

// Come usare
/*Esempio A: Operazione rapida con pulizia automatica (using)
 * public async Task DownloadFileAsync()
{
    // Viene creata la coppia. Quando il blocco `using` termina, la coppia sparisce da sola.
    using StatusTask status = StatusService.AvviaTask("Download in corso...");
    await Task.Delay(1000); // Simulazione lavoro
    // Aggiorna il messaggio e passa alla percentuale fissa
    status.Update("Estraggo i dati...", percent: 50);
    await Task.Delay(10000);
} // <- Qui status.Dispose() viene chiamato automaticamente e la coppia viene rimossa

Esempio B: Operazione a lungo termine gestita manualmente
private StatusTask? _downloadLungo;

public void IniziaSincronizzazione()
{
    _downloadLungo = StatusService.AvviaTask("Sincronizzazione...", isIndeterminate: true);
}

public void Avanzamento(int percentuale)
{
    _downloadLungo?.Update($"Sincronizzazione: {percentuale}%", percentuale);
}

public void TerminaSincronizzazione()
{
    _downloadLungo?.Dispose(); // Rimuove la coppia dalla StatusBar
    _downloadLungo = null;
}
 */

namespace LaParola.Services
{
    /// <summary>
    /// Rappresenta una singola coppia (Testo + ProgressBar) visibile nella StatusBar.
    /// Implementa IDisposable per consentire la distruzione automatica a fine operazione.
    /// </summary>
    public class StatusTask(string message, Visibility isVisible = Visibility.Visible, bool isIndeterminate = true) : INotifyPropertyChanged, IDisposable
    {
        private string _message = message;
        private bool _isIndeterminate = isIndeterminate;
        private Visibility _isVisible = isVisible;
        private double _progress = 0;

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set { _isIndeterminate = value; OnPropertyChanged(); }
        }

        public Visibility IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(); }
        }

        public double Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Aggiorna il messaggio e/o la percentuale di progresso (da 0 a 100).
        /// </summary>
        public void Update(string message, double? percent = null)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                Message = message;
                if (percent.HasValue)
                {
                    IsIndeterminate = false;
                    Progress = percent.Value;
                }
            });
        }

        /// <summary>
        /// Rimuove la coppia dalla StatusBar.
        /// </summary>
        public void Dispose()
        {
            StatusService.RimuoviTask(this);
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal static class StatusService
    {
        public static ObservableCollection<StatusTask> Tasks { get; } = [];

        /// <summary>
        /// Crea e registra una nuova coppia Testo/ProgressBar nella StatusBar.
        /// </summary>
        public static StatusTask AvviaTask(string messaggioIniziale, Visibility isVisible = Visibility.Visible, bool isIndeterminate = true)
        {
            StatusTask task = new(messaggioIniziale, isVisible, isIndeterminate);

            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                Tasks.Add(task);
            });

            return task;
        }

        public static void RimuoviTask(StatusTask task)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                Tasks.Remove(task);
            });
        }
    }
}