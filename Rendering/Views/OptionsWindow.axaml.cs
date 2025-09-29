using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Rendering.Views;

public partial class OptionsWindow : Window
{
    // Constructor //
    public OptionsWindow()
    {
        InitializeComponent();
        // Setup //
        SetupEvents();
        foreach (var familyName in FontFamily.FamilyNames)
            SpeechFont.Items.Add
            (
                new ComboBoxItem() { Content = familyName, FontFamily = "Nunito" }
            );
    }

    // Setup Methods //
    public void SetupEvents()
    {
        ThemeSelection.SelectionChanged += (object? sender, SelectionChangedEventArgs eventData) =>
        {
            // Selected Item //
            dynamic? selectedItem = ThemeSelection.SelectedItem;
            // Switch //
            switch(selectedItem?.Content.ToString())
            {
                case "Dark":
                    RequestedThemeVariant = ThemeVariant.Dark;
                    break;

                case "Light":
                    RequestedThemeVariant = ThemeVariant.Light;
                    break;

                default:
                    break;
            }
        };
    }
}