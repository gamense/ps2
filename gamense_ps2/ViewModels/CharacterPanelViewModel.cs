namespace gamense_ps2.ViewModels {

    public class CharacterPanelViewModel : ViewModelBase {

        private string _CharacterName = "";
        public string CharacterName {
            get {
                return _CharacterName;
            }
            set {
                _CharacterName = value;
                OnPropertyChanged(nameof(CharacterName));
            }
        }

    }

}
