using Microsoft.Extensions.Logging;
using MahApps.Metro.Controls;

namespace gamense_ps2 {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        private readonly ILogger<MainWindow> _Logger;

        private readonly ToyWrapper _ToyWrapper;

        public MainWindow(ILogger<MainWindow> logger,
            ToyWrapper toyWrapper)
        {

            _Logger = logger;

            InitializeComponent();

            _ToyWrapper = toyWrapper;
        }

    }
}
