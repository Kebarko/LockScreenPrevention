using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace KE.LockScreenPrevention;

/// <summary>
/// Provides utility methods for managing application startup and preventing the system from entering sleep mode.
/// </summary>
internal static class Utils
{
    private const string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    /// <summary>
    /// Adds the application to the Windows startup registry key so that it runs on startup.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="appPath">The full path to the application executable.</param>
    public static void AddToStartup(string appName, string appPath)
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(regKey, true);

        if (key == null)
        {
            key = Registry.CurrentUser.CreateSubKey(regKey);
        }

        key.SetValue(appName, "\"" + appPath + "\"");
        key.Close();
    }

    /// <summary>
    /// Removes the application from the Windows startup registry key.
    /// </summary>
    /// <param name="appName">The name of the application to remove.</param>
    public static void RemoveFromStartup(string appName)
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(regKey, true);

        if (key != null)
        {
            key.DeleteValue(appName, false);
            key.Close();
        }
    }

    /// <summary>
    /// Check if the specified application is set to run on Windows startup.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="appPath">The full path to the application executable.</param>
    /// <returns>
    /// <c>True</c> if the application is set to run on startup; otherwise, <c>False</c>.
    /// </returns>
    public static bool IsInStartup(string appName, string appPath)
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(regKey, false);

        return key != null && (key.GetValue(appName) as string)?.Trim('\"') == appPath;
    }

    /// <summary>
    /// Prevents the system from entering sleep mode by setting the necessary execution state.
    /// </summary>
    public static void Enable()
    {
        SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
    }

    /// <summary>
    /// Allows the system to enter sleep mode by resetting the execution state.
    /// </summary>
    public static void Disable()
    {
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    [Flags]
    private enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }
}
