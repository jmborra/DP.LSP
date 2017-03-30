using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using DP.LSP.Tools.DiskMon.Core;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public enum ReportingFrequency { Daily, Weekly, Monthly, Yearly }

    interface IConfigurationHelper
    {
        IEnumerable<Drive> Drives { get; }
        DateTime LastReportDate { get; set; }
        string MailSender { get; }
        string MailSubject { get; }
        IEnumerable<Recipient> Recipients { get; }
        ReportingFrequency ReportingFrequency { get; }
        string SmtpHost { get; }
        int SmtpPort { get; }
        void Load();
    }

    internal class ConfigurationHelper : IConfigurationHelper
    {
        #region Fields
        private static SettingsSection _settings;
        #endregion

        #region Properties
        //TODO: Refactor
        public IEnumerable<Drive> Drives
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

        public DateTime LastReportDate
        {
            get { return Convert.ToDateTime(Properties.Settings.Default[Constants.LastReportDateKey]); }
            set
            {
                Properties.Settings.Default[Constants.LastReportDateKey] = value;
                Properties.Settings.Default.Save();
            }
        }

        public string MailSender
        {
            get { return Get<string>(Constants.MailSenderKey); }
        }

        public string MailSubject
        {
            get { return Get<string>(Constants.MailSubjectKey); }
        }

        public IEnumerable<Recipient> Recipients
        {
            get
            {
                foreach (RecipientElement recipient in _settings.Recipients)
                    yield return new Recipient(recipient.Email);
            }
        }
        public ReportingFrequency ReportingFrequency
        {
            get { return Get<ReportingFrequency>(Constants.ReportingFrequencyKey); }
        }

        public string SmtpHost
        {
            get { return Get<string>(Constants.SmtpHostKey); }
        }

        public int SmtpPort
        {
            get { return Get<int>(Constants.SmtpPortKey); }
        }
        #endregion

        #region Methods
        private T Get<T>(string key)
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
            LogHelper.Instance.Error(error);
            throw new Exception(error);
        }

        public void Load()
        {
            try
            {
                _settings = ConfigurationManager.GetSection("settings") as SettingsSection;
            }
            catch(Exception e)
            {
                LogHelper.Instance.ErrorFormat("Unable to load configuration: {0}.", e);
                throw e;
            }
        } 
        #endregion
    }
}
