using System;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Theming;
using gamense_ps2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace gamense_ps2.Panels;

public partial class SettingsPanel
{
    
    private readonly ILogger<SettingsPanel> _Logger;
    private readonly Settings _CurrentSettings;

    public SettingsPanel()
    {
        _Logger = App.Services.GetRequiredService<ILogger<SettingsPanel>>();
        _CurrentSettings = Settings.Instance;
        InitializeComponent();
        ApplyTheme();
    }
    
    private void ApplyTheme()
    {
        var sThemeName = Enum.GetName(_CurrentSettings.CurrentTheme);

        var sDarkMode = _CurrentSettings.DarkMode ? "Dark" : "Light";

        var sThemeId = sDarkMode + "." + sThemeName;

        _Logger.LogDebug($"Applying Theme {sThemeId}");

        var themeName = Enum.GetName(_CurrentSettings.CurrentTheme);
        if (ComboBoxThemeColorSelection.Text != themeName)
        {
            ComboBoxThemeColorSelection.Text = themeName;
        }
        
        if (RadioButtonSettingsDarkMode.IsChecked != _CurrentSettings.DarkMode)
        {
            RadioButtonSettingsDarkMode.IsChecked = _CurrentSettings.DarkMode;
        }
        
        if (RadioButtonSettingsLightMode.IsChecked == _CurrentSettings.DarkMode)
        {
            RadioButtonSettingsLightMode.IsChecked = !_CurrentSettings.DarkMode;
        }
        
        ThemeManager.Current.ChangeTheme((App)Application.Current, sThemeId);
    }

    private void Theme_Selection_Changed_ComboBox(object sender, SelectionChangedEventArgs e)
    {
        
        var theme = e.AddedItems[0];
        if (theme != null)
        {
            _CurrentSettings.CurrentTheme = (ThemeName)theme;
        
            ApplyTheme();

        }

    }
    
    private void Dark_Mode_RadioButton(object sender, RoutedEventArgs e)
    {

        _CurrentSettings.DarkMode = true;
        ApplyTheme();
        
    }
    
    private void Light_Mode_RadioButton(object sender, RoutedEventArgs e)
    {
        
        _CurrentSettings.DarkMode = false;
        ApplyTheme();
        
    }
    
}