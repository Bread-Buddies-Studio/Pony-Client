using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pony_Client.Services;
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
static class AccountService
{
    // Public Data //
    public static bool SignedIn { get; private set; }
    // Private Data //
    private static byte? TagLine;
    private static string? UserName = "default";
    private static string? Password;
    // Base Methods //
    public static void LogIn(string accountData)
    {
        // Data Source //
        ReadOnlySpan<string> data = accountData.Split('|');
        // Extract Data //
        if (!byte.TryParse(data[0], out byte tagLine))
            throw new InvalidTaglineException();
        string userName = data[1];
        string passWord = data[2];
        // Assign Data //
        TagLine = tagLine;
        UserName = userName;
        Password = passWord;
        // Update Signed In Value //
        SignedIn = true;
    }

    public static void LogOut()
    {
        // Reset Data //
        TagLine = null;
        UserName = null;
        Password = null;
        // Update Signed In Value //
        SignedIn = false;
    }
    // Getters //
    public static string? GetUserName() => UserName;
}