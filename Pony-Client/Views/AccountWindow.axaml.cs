using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiteNetLib;
using Pony_Client.Services;
using System;

namespace Pony_Client.Views;

public partial class AccountWindow : Window
{
    // Data //
    string? connectionData;
    // Constructor //
    public AccountWindow()
    {
        InitializeComponent();

        ActivateLogIn.Click += (object? sender, RoutedEventArgs _) =>
        {
            // Debug //
            Success.Text = "Logging In...";
            // Log In Data //
            string connectionData = $"{TagLine.Text}|{UserName.Text}|{Password.Text}";
            // Create Connection //
            if (client is null || client.server.ConnectionState is ConnectionState.Disconnected)
                client = new Client(connectionData, API.LogIn);
            // Update Data //
            this.connectionData = connectionData;
        };

        ActivateSignUp.Click += (object? sender, RoutedEventArgs _) =>
        {
            // Debug //
            Success.Text = "Signing Up...";
            // Sign Up Data //
            string connectionData = $"{TagLine.Text}|{UserName.Text}|{Password.Text}";
            // Create Connection //
            if (client is null || client.server.ConnectionState is ConnectionState.Disconnected)
                client = new Client(connectionData, API.SignUp);
            // Update Data //
            this.connectionData = connectionData;
        };

        PasswordVisible.Click += (object? sender, RoutedEventArgs _) =>
        {
            bool? isChecked = PasswordVisible.IsChecked;
            // Conditions //
            if (isChecked is null)
                return;
            // Settings //
            Password.RevealPassword = (bool)isChecked;
        };

        RunService.Update += (object? sender, UpdateEventArgs _) =>
        {
            // Data //
            ConnectionState? connectionState = client?.server.ConnectionState;
            // Checks //
            if (connectionState is ConnectionState.Connected && !AccountService.SignedIn && connectionData is not null)
            {
                AccountService.LogIn(connectionData);
            }
            // Debug //
            Disconnect.Text = client?.failReason?.ToString();
            Success.Text = client?.server.ConnectionState.ToString();
            // Network Events //
            client?.client.PollEvents();
        };
    }


}