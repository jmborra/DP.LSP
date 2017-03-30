using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Reflection;
using System.IO;

using DP.LSP.Tools.DiskMon.Configuration;


namespace DP.LSP.Tools.DiskMon.Core
{
    interface IEmailHelper
    {
        void SendAlert(IEnumerable<Drive> lowDrives, IEnumerable<Drive> otherDrives);
        void SendReport(IEnumerable<Drive> drives);
    }

    internal class EmailHelper : IEmailHelper
    {
        public void SendAlert(IEnumerable<Drive> lowDrives, IEnumerable<Drive> otherDrives)
        {
            var config = ServiceLocator.Instance.GetService<IConfigurationHelper>();
            var diskManager = ServiceLocator.Instance.GetService<IDiskManager>();

            var builder = CreateBuilder("DP.LSP.Tools.DiskMon.Template.html");
            builder.Replace(Constants.EmailMachinePlaceholderKey, diskManager.MachineName);
            builder.Replace(Constants.EmailLowDrivesPlaceholderKey, Compose(lowDrives));

            if (otherDrives.Any())
                builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, Compose(otherDrives));
            else
                builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, "<tr class=\"tb2\"><td colspan=\"2\">None</td></tr>");

            var recipients = config.Recipients.Select(r => r.Email);
            if (recipients.Any())
                Send(config.MailSender,
                    string.Format("{0} on {1}", config.MailSubject, diskManager.MachineName),
                    builder.ToString(),
                    recipients,
                    true);
            else
            {
                var error = "Unable to send email. No recipients specified in config.";
                LogHelper.Instance.Error(error);
                throw new Exception(error);
            }
        }

        //TODO: Refactor
        public void SendReport(IEnumerable<Drive> drives)
        {
            var config = ServiceLocator.Instance.GetService<IConfigurationHelper>();
            var diskManager = ServiceLocator.Instance.GetService<IDiskManager>();

            var lastReportDate = config.LastReportDate;
            var sendReport = false;
            switch (config.ReportingFrequency)
            {
                case ReportingFrequency.Daily: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 1; break;
                case ReportingFrequency.Weekly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 7; break;
                case ReportingFrequency.Monthly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 30; break;
                case ReportingFrequency.Yearly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 365; break;
            }

            if (sendReport)
            {
                config.LastReportDate = DateTime.Now;
                var builder = CreateBuilder("DP.LSP.Tools.DiskMon.Report.html");
                builder.Replace(Constants.EmailMachinePlaceholderKey, diskManager.MachineName);

                if (drives.Any())
                    builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, Compose(drives));
                else
                    builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, "<tr class=\"tb2\"><td colspan=\"2\">None</td></tr>");

                var recipients = config.Recipients.Select(r => r.Email);
                if (recipients.Any())
                    Send(config.MailSender,
                        string.Format("Disk Status on {0}", diskManager.MachineName),
                        builder.ToString(),
                        recipients,
                        false);
                else
                {
                    var error = "Unable to send email. No recipients specified in config.";
                    LogHelper.Instance.Error(error);
                    throw new Exception(error);
                }
            }
        }

        #region Helper Methods
        private static StringBuilder CreateBuilder(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();

            StringBuilder builder;
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(stream))
                {
                    builder = new StringBuilder(reader.ReadToEnd());
                }
            }

            return builder;
        }

        private static string Compose(IEnumerable<Drive> drives)
        {
            var builder = new StringBuilder();
            var low = drives.All(d => d.IsLow);
            foreach (var drive in drives)
            {
                builder.AppendFormat("<tr class=\"tb{0}\">", (low ? "1" : "2"));
                builder.AppendFormat("<td>{0}</td>", drive.Name);
                builder.AppendFormat("<td>{0}</td>", drive.ToString().Replace(drive.Name, string.Empty));
                builder.AppendLine("<tr>");
            }

            return builder.ToString();
        }

        private static void Send(string sender, string subject, string message, IEnumerable<string> recipients, bool priority = false)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(sender);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = priority ? MailPriority.High : MailPriority.Normal;

            foreach (var recipient in recipients)
                mail.To.Add(recipient);

            try
            {
                var config = ServiceLocator.Instance.GetService<IConfigurationHelper>();
                var client = new SmtpClient(config.SmtpHost, config.SmtpPort);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Send(mail);
            }
            catch (Exception e)
            {
                LogHelper.Instance.ErrorFormat("Unable to send email: {0}.", e);
                throw e;
            }
        }
        #endregion
    }
}
