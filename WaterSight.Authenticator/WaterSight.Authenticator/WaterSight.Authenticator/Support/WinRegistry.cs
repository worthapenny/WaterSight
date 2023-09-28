using Microsoft.Win32;
using Serilog;

namespace WaterSight.Authenticator.Support;

public class WinRegistry
{
    #region Constants
    public const string keyUserName = "SOFTWARE\\WaterSight";
    public const string keyMachineName = "SOFTWARE\\WaterSight";
    public const string PROD_SubKeyName = "BentleyProdOIDCToken";
    public const string QA_SubKeyName = "BentleyQaOIDCToken";
    public const string DEV_SubKeyName = "BentleyDevOIDCToken";
    public const string PROD_ValueUpdatedAtName = "BentleyProdUpdatedAt";
    public const string QA_ValueUpdatedAtName = "BentleyQAUpdatedAt";
    public const string DEV_ValueUpdatedAtName = "BentleyDevUpdatedAt";
    #endregion

    #region Constructor
    public WinRegistry()
    {
    }
    #endregion

    #region Static Methods
    public static bool UpdateRegistry(string token, bool isProd)
    {
        var keyUser = Registry.CurrentUser.OpenSubKey(keyUserName, true);
        var keyMachine = Registry.LocalMachine.OpenSubKey(keyMachineName, true);
        var success = true;

        try
        {
            if (keyUser == null)
            {
                Log.Error($"Failed to get the registry. Creating a new one under CurrentUser: '{keyMachineName}'.");
                try
                {
                    keyUser = Registry.CurrentUser.CreateSubKey(keyUserName, true);
                    Log.Debug($"Registry item created. {keyUser}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"...while creating registry item '{keyUser}' under CurrentUser");
                }
            }
            if (keyMachine == null)
            {
                Log.Error($"Failed to get the registry entry. Not running as administrator? Key = {keyMachine}");
                try
                {
                    keyMachine = Registry.LocalMachine.CreateSubKey(keyMachineName, true);
                    Log.Debug($"Registry item created. {keyMachine}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"...while creating registry item '{keyMachineName}' under LocalMachine");
                }
            }


            if (!isProd)
            {
                keyUser?.SetValue(QA_SubKeyName, token);
                keyUser?.SetValue(QA_ValueUpdatedAtName, DateTime.Now);

                keyUser?.SetValue(DEV_SubKeyName, token);
                keyUser?.SetValue(DEV_ValueUpdatedAtName, DateTime.Now);

                keyMachine?.SetValue(QA_SubKeyName, token);
                keyMachine?.SetValue(QA_ValueUpdatedAtName, DateTime.Now);

                keyMachine?.SetValue(DEV_SubKeyName, token);
                keyMachine?.SetValue(DEV_ValueUpdatedAtName, DateTime.Now);

                Log.Debug($"Registry updated for non-prod keys. Paths: {QA_SubKeyName} and {DEV_SubKeyName}");
            }
            else
            {
                keyUser?.SetValue(PROD_SubKeyName, token);
                keyUser?.SetValue(PROD_ValueUpdatedAtName, DateTime.Now);

                keyMachine?.SetValue(PROD_SubKeyName, token);
                keyMachine?.SetValue(PROD_ValueUpdatedAtName, DateTime.Now);

                Log.Debug($"Registry updatedPaths: {keyUser} and {keyMachine}");
                Log.Debug($"Registry updated for prod keys.  Paths: {PROD_SubKeyName} and {DEV_SubKeyName}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while updating the registry");
            success = false;
        }

        return success;
    }
    #endregion
}
