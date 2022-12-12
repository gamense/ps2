using Buttplug;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gamense_ps2 {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private readonly ILogger<MainWindow> _Logger;

        private readonly ToyWrapper _ToyWrapper;

        public MainWindow(ILogger<MainWindow> logger,
            ToyWrapper toyWrapper) {

            _Logger = logger;

            InitializeComponent();

            _ToyWrapper = toyWrapper;
        }

    }
}
