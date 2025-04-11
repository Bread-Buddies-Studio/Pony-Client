using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Pony_Client.Views;

public partial class OptionsWindow : Window
{
    // Constructor //
    public OptionsWindow()
    {
        InitializeComponent();
        // Setup //
        SetupEvents();
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