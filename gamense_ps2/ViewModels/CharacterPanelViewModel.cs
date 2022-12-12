using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
