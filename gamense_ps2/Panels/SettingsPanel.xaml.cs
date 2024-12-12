using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Theming;
using gamense_ps2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace gamense_ps2.Panels;

public partial class SettingsPanel
{
    private const string SettingsPath = "./gamense_settings.json";

    private readonly Settings _CurrentSettings;

    private readonly ILogger<SettingsPanel> _Logger;

    public SettingsPanel()
    {
        _Logger = App.Services.GetRequiredService<ILogger<SettingsPanel>>();
        _Logger.LogDebug("Creating Settings Panel");
        _CurrentSettings = LoadSettings();
        InitializeComponent();
        ApplyTheme();
    }

    private Settings LoadSettings()
    {
        
        Settings settings;

        try
        {
            
            var contents = File.ReadAllText(SettingsPath);
            
            var trySettings = JsonConvert.DeserializeObject<Settings>(contents);
            
            if (trySettings == null)
            {
                _Logger.LogWarning("Failed to parse settings file");
                throw new Exception();
            }

            settings = trySettings;
        }
        catch (Exception ignored)
        {
            _Logger.LogDebug($"Could not read file {SettingsPath} in {Directory.GetCurrentDirectory()}, creating new settings file.");

            settings = new Settings();
            
            var settingsText = JsonConvert.SerializeObject(settings);
            
            File.WriteAllText(SettingsPath, settingsText);
            
        }
        
        return settings;
    }

    private void ApplyTheme()
    {
        var sThemeName = Enum.GetName(_CurrentSettings.CurrentTheme) ?? "Cyan";

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

    private void SaveSettings()
    {
        var settingsText = JsonConvert.SerializeObject(_CurrentSettings);
        File.WriteAllText(SettingsPath, settingsText);
    }

    private void Theme_Selection_Changed_ComboBox(object sender, SelectionChangedEventArgs e)
    {
        
        var theme = e.AddedItems[0];
        if (theme != null)
        {
            _CurrentSettings.CurrentTheme = (ThemeName)theme;
        
            ApplyTheme();
            SaveSettings();
        }

    }
    
    private void Dark_Mode_RadioButton(object sender, RoutedEventArgs e)
    {

        _CurrentSettings.DarkMode = true;
        ApplyTheme();
        SaveSettings();
        
    }
    
    private void Light_Mode_RadioButton(object sender, RoutedEventArgs e)
    {
        
        _CurrentSettings.DarkMode = false;
        ApplyTheme();
        SaveSettings();

    }
    
}