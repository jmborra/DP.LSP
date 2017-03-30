using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using DP.LSP.Tools.DiskMon.Core;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public enum ReportingFrequency { Daily, Weekly, Monthly, Yearly }

    internal sealed class ConfigurationHelper
    {
        #region Fields
        private static SettingsSection _settings;
        #endregion

        #region Properties
        //TODO: Refactor
        public static IEnumerable<Drive> Drives
        {
            get
            {
                if (_settings.Drives.All(d => d.Name.ToUpper() == Constants.DriveAllKey))
                {
                    var threshold = _settings.Drives.First().Threshold;
                    foreach (var drive in DriveInfo.GetDrives())
                        yield return new Drive(drive.Name, threshold);
                }
                else
                {
                    foreach (DriveElement drive in _settings.Drives)
                        if (drive.Name.ToUpper() != Constants.DriveAllKey)
                            yield return new Drive(drive.Name, drive.Threshold);
                }
            }
        }

        internal static DateTime LastReportDate
        {
            get { return Convert.ToDateTime(Properties.Settings.Default[Constants.LastReportDateKey]); }
            set
            {
                Properties.Settings.Default[Constants.LastReportDateKey] = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string MailSender
        {
            get { return Get<string>(Constants.MailSenderKey); }
        }

        public static string MailSubject
        {
            get { return Get<string>(Constants.MailSubjectKey); }
        }

        public static IEnumerable<Recipient> Recipients
        {
            get
            {
                foreach (RecipientElement recipient in _settings.Recipients)
                    yield return new Recipient(recipient.Email);
            }
        }
        public static ReportingFrequency ReportingFrequency
        {
            get { return Get<ReportingFrequency>(Constants.ReportingFrequencyKey); }
        }

        public static string SmtpHost
        {
            get { return Get<string>(Constants.SmtpHostKey); }
        }

        public static int SmtpPort
        {
            get { return Get<int>(Constants.SmtpPortKey); }
        }
        #endregion

        #region Methods
        private static T Get<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(value))            
                try
                {
                    if (typeof(T).IsEnum)
                        return (T)Enum.Parse(typeof(T), value, true);

                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch {}

            var error = string.Format("Value for {0} is invalid or missing.", key);
            LogHelper.Logger.Error(error);
            throw new Exception(error);
        }

        public static void Load()
        {
            try
            {
                _settings = ConfigurationManager.GetSection("settings") as SettingsSection;
            }
            catch(Exception e)
            {
                LogHelper.Logger.ErrorFormat("Unable to load configuration: {0}.", e);
                throw e;
            }
        } 
        #endregion
    }
}
