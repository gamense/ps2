using System.ComponentModel;

namespace gamense_ps2.ViewModels {

    public class ViewModelBase : INotifyPropertyChanged {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
