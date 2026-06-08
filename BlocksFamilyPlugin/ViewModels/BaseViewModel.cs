using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BlocksFamilyPlugin.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels.
    /// Provides <see cref="INotifyPropertyChanged"/> and the
    /// <see cref="SetProperty{T}"/> helper that only raises the event on change.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets <paramref name="field"/> to <paramref name="value"/>.
        /// Raises <see cref="PropertyChanged"/> only when the value actually changed.
        /// </summary>
        /// <returns><c>true</c> if the value changed.</returns>
        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
