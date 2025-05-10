using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiteNetLib;
using Network;
using Rendering.Services;
using Network.Services;
using System;
using Core.Services;
using static Network.Universal.Global;

namespace Rendering.Views;

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
            AccountService.LogIn(connectionData);
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
            AccountService.SignUp(connectionData);
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
            // Debug //
            Disconnect.Text = CurrentClient?.failReason?.ToString();
            Success.Text = CurrentClient?.server.ConnectionState.ToString();
            // Network Events //
            AccountService.Client?.client.PollEvents();
        };
    }


}