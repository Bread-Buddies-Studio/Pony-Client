using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using Core.Services;
using Network.Services;
using Rendering.Services;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Rendering.Views;

public partial class MainWindow : Window
{
    // Private Data //
    AccountWindow? accountWindow;
    OptionsWindow? optionsWindow;
    CreditsWindow? creditsWindow;
    // Constructor //
    public MainWindow()
    {
        // Initialize Debugger //
        Logger.TryGet(LogEventLevel.Fatal, LogArea.Control)?.Log(this, "Avalonia Infrastructure");
        Debug.WriteLine("System Diagnostics Debug");
        // Initialize Window //
        InitializeComponent();
        #region Buttons
        AccountButton.Click += (object? sender, RoutedEventArgs _) =>
        {
            accountWindow?.Close();
            accountWindow = new AccountWindow();
            accountWindow.Show();
        };
        OptionsButton.Click += (object? sender, RoutedEventArgs _) =>
        {
            optionsWindow?.Close();
            optionsWindow = new OptionsWindow();
            optionsWindow.Show();
        };
        CreditsButton.Click += (object? sender, RoutedEventArgs _) =>
        {
            creditsWindow?.Close();
            creditsWindow = new CreditsWindow();
            creditsWindow.Show();
        };
        LogOutButton.Click += (object? sender, RoutedEventArgs _) =>
        {
            AccountService.LogOut();
        };
        PlayButton.Click += (object? sender, RoutedEventArgs _) =>
        {
            // Conditions //
            //if (!AccountService.SignedIn)
            //    return;
            // Settings //
            MenuPanel.IsVisible = false;
            Service.IsVisible = true;

            string? userName = AccountService.GetUserName();

            if (userName is null)
                return;

            WorldService.LocalPlayer = new(
                Name: userName,
                Position: new Point(200, 200),
                Size: new Size(200, 200)
            ) { Layer = 2 };

            Debug.WriteLine($"UserName: {WorldService.LocalPlayer.Name}");

        };
        #endregion
        RunService.Update += (object? sender, UpdateEventArgs _) =>
        {
            // Settings //
            Service.InvalidateVisual();
            AccountDisplay.Content = $"Account: {AccountService.GetUserName() ?? "None"}";
        };
        // Connections //
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    private void OnKeyDown(object? sender, KeyEventArgs eventData) => InputService.OnKeyDown(sender, eventData);
    private void OnKeyUp(object? sender, KeyEventArgs eventData) => InputService.OnKeyUp(sender, eventData);
}