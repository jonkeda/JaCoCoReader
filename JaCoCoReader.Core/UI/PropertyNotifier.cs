using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JaCoCoReader.Core.Threading;

namespace JaCoCoReader.Core.UI
{
    public class PropertyNotifier : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            if (!Equals(field, value))
            {
                NotifyPropertyChanging(propertyName);

                field = value;

                NotifyPropertyChanged(propertyName);

                return true;
            }
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                ThreadDispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void NotifyPropertyChanging([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanging != null)
            {
                ThreadDispatcher.Invoke(() => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName)));
            }
        }
    }
}
