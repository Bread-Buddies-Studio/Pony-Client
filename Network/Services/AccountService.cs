using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using Network.Enums;

namespace Network.Services;
// Exceptions //
[Serializable]
public class UsernameInvalidFormat(string message = "Username invalid format") : Exception(message) { }
[Serializable]
public class PasswordInvalidFormat(string message = "Password invalid format") : Exception(message) { }
[Serializable]
public class AccountExistsException(string message = "Account already exists") : Exception(message) { }
[Serializable]
public class AccountNotFoundException(string message = "Account not found") : Exception(message) { }
[Serializable]
public class InvalidTaglineException(string message = "Invalid tagline") : Exception(message) { }
[Serializable]
public class IncorrectPasswordException(string message = "Incorrect Password") : Exception(message) { }
// Service //
public static class AccountService
{
    // Constants //
    public const char DataSeperator = '|';
    // Public Data //
    public static Client? Client { get; set; }
    public static bool SignedIn 
    {
        get => Client?.Connected ?? false;
    }
    // Private Data //
    private static byte? TagLine = 000;
    private static string? UserName = "default";
    private static string? Password = "default";
    // Base Methods //
    public static void SignUp(string accountData)
    {
        // Data Source //
        ReadOnlySpan<string> data = accountData.Split(DataSeperator).AsSpan();
        // Extract Data //
        if (!byte.TryParse(data[0], out byte tagLine))
            throw new InvalidTaglineException();
        string userName = data[1];
        string passWord = data[2];
        // Assign Data //
        TagLine = tagLine;
        UserName = userName;
        Password = passWord;
        // Delete Old Connection //
        Client?.server.Disconnect();
        // Create Connection //
        if (Client is null || Client.server.ConnectionState is ConnectionState.Disconnected)
            Client = new Client(accountData, API.SignUp);
    }
    public static void LogIn(string accountData)
    {
        // Data Source //
        ReadOnlySpan<string> data = accountData.Split(DataSeperator).AsSpan();
        // Extract Data //
        if (!byte.TryParse(data[0], out byte tagLine))
            throw new InvalidTaglineException();
        string userName = data[1];
        string passWord = data[2];
        // Assign Data //
        TagLine = tagLine;
        UserName = userName;
        Password = passWord;
        // Delete Old Connection //
        Client?.server.Disconnect();
        // Create Connection //
        if (Client is null || Client.server.ConnectionState is ConnectionState.Disconnected)
            Client = new Client(accountData, API.LogIn);
    }
    public static void LogOut()
    {
        // Reset Data //
        TagLine = null;
        UserName = null;
        Password = null;
        // Disconnect //
        Client?.server.Disconnect();
    }
    // Getters //
    public static string? GetUserName() => UserName;
    public static string? GetAccountData() => string.Join(DataSeperator, [TagLine, UserName, Password]);
}