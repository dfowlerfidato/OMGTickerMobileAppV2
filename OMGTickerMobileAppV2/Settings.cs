// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace OMGTickerMobileAppV2.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(SettingsKey, value);
            }
        }


        private const string UserNameKey = "username_key";
        private static readonly string UserNameDefault = string.Empty;

        private const string AuthTokenKey = "authtoken_key";
        private static readonly string AuthTokenDefault = string.Empty;

        private const string SaltKey = "salt_key";
        private static readonly string SaltDefault = string.Empty;

        private const string DeviceTokenKey = "devicetoken_key";
        private static readonly string SomeIntDefault = string.Empty;

        public static string UserName
        {
            get { return AppSettings.GetValueOrDefault<string>(UserNameKey, UserNameDefault); }
            set { AppSettings.AddOrUpdateValue<string>(UserNameKey, value); }
        }

        public static string AuthToken
        {
            get { return AppSettings.GetValueOrDefault<string>(AuthTokenKey, SomeIntDefault); }
            set { AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value); }
        }

        public static string Salt
        {
            get { return AppSettings.GetValueOrDefault<string>(SaltKey, UserNameDefault); }
            set { AppSettings.AddOrUpdateValue<string>(SaltKey, value); }
        }

        public static string DeviceToken
        {
            get { return AppSettings.GetValueOrDefault<string>(DeviceTokenKey, SomeIntDefault); }
            set { AppSettings.AddOrUpdateValue<string>(DeviceTokenKey, value); }
        }

    }
}