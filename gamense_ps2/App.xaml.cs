using gamense_ps2.Census;
using gamense_ps2.Panels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ControlzEx.Theming;
using gamense_ps2.Models;

namespace gamense_ps2 {

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        //private IServiceProvider _Services;
        private readonly IHost _Host;

        [NotNull]
        public static IServiceProvider Services = default!;

        public App() {
            _Host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    ConfigureServices(services);
                })
                .ConfigureAppConfiguration(context => {
                    context.AddJsonFile("appsettings.json");
                })
                .Build();

            Services = _Host.Services;
        }

        private void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<CensusPanel>();
            services.AddSingleton<ConnectPanel>();
            services.AddSingleton<SettingsPanel>();

            services.AddLogging();
            services.AddCensusServices();
            
            services.AddSingleton<CharacterCensus>();
            services.AddSingleton<ToyWrapper>();
            services.AddSingleton<RealtimeStream>();
            services.AddSingleton<Census.EventHandler>();
            services.AddSingleton<Vibrate>();
        }

        protected override async void OnStartup(StartupEventArgs e) {
            await _Host.StartAsync();
            
            MainWindow main = _Host.Services.GetRequiredService<MainWindow>();
            main.Show();

            base.OnStartup(e);
            
        }

        protected override async void OnExit(ExitEventArgs e) {
            await _Host.StopAsync();

            base.OnExit(e);
        }

    }
}
