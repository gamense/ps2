using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using gamense_ps2.Panels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace gamense_ps2.Models;

public class Settings
{
    private const string SettingsPath = "./gamense_settings.json";

    private readonly SettingsValues _SettingsValues;

    public readonly ObservableCollection<PsCharacter> SubscribedCharacters;

    static Settings()
    {
    }

    private Settings()
    {
        SubscribedCharacters = new ObservableCollection<PsCharacter>();
        _SettingsValues = LoadSettings();

        foreach (var charName in _SettingsValues.SubscribedCharacters) SubscribedCharacters.Add(charName);

        SubscribedCharacters.CollectionChanged += (_, _) =>
        {
            _SettingsValues.SubscribedCharacters.Clear();

            foreach (var character in SubscribedCharacters) _SettingsValues.SubscribedCharacters.Add(character);
            
            SaveSettings();
        };
    }

    public static Settings Instance { get; } = new();

    public ThemeName CurrentTheme
    {
        get => _SettingsValues.CurrentTheme;
        set
        {
            _SettingsValues.CurrentTheme = value;
            SaveSettings();
        }
    }

    public bool DarkMode
    {
        get => _SettingsValues.DarkMode;
        set
        {
            _SettingsValues.DarkMode = value;
            SaveSettings();
        }
    }

    private static SettingsValues LoadSettings()
    {
        try
        {
            var contents = File.ReadAllText(SettingsPath);

            var trySettings = JsonConvert.DeserializeObject<SettingsValues>(contents) ?? throw new Exception();
            
            return trySettings;
        }
        catch (Exception)
        {
            return new SettingsValues(ThemeName.Cyan, true, new List<PsCharacter>());
        }
    }

    private void SaveSettings()
    {
        var settingsText = JsonConvert.SerializeObject(this);
        File.WriteAllText(SettingsPath, settingsText);
    }

    private class SettingsValues
    {
        // regardless of what your build tools say, this value may not be readonly
        public IList<PsCharacter> SubscribedCharacters;
        public ThemeName CurrentTheme;
        public bool DarkMode;

        public SettingsValues(ThemeName currentTheme, bool darkMode, IList<PsCharacter> characterNames)
        {
            CurrentTheme = currentTheme;
            DarkMode = darkMode;
            SubscribedCharacters = characterNames;
        }
    }
}