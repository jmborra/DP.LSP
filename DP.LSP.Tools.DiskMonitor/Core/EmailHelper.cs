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
    internal sealed class EmailHelper
    {
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

        public static void Send(IEnumerable<Drive> lowDrives, IEnumerable<Drive> otherDrives)
        {
            var builder = CreateBuilder("DP.LSP.Tools.DiskMon.Template.html");
            builder.Replace(Constants.EmailMachinePlaceholderKey, DiskManager.MachineName);
            builder.Replace(Constants.EmailLowDrivesPlaceholderKey, Compose(lowDrives));

            if (otherDrives.Any())
                builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, Compose(otherDrives));
            else
                builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, "<tr class=\"tb2\"><td colspan=\"2\">None</td></tr>");

            var recipients = ConfigurationHelper.Recipients.Select(r => r.Email);
            if (recipients.Any())
                Send(ConfigurationHelper.MailSender,
                    string.Format("{0} on {1}", ConfigurationHelper.MailSubject, DiskManager.MachineName),
                    builder.ToString(),
                    recipients,
                    true);
            else
            {
                var error = "Unable to send email. No recipients specified in config.";
                LogHelper.Logger.Error(error);
                throw new Exception(error);
            }
        }

        public static void Send(string sender, string subject, string message, IEnumerable<string> recipients, bool priority = false)
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
                var client = new SmtpClient(ConfigurationHelper.SmtpHost, ConfigurationHelper.SmtpPort);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Send(mail);
            }
            catch(Exception e)
            {
                LogHelper.Logger.ErrorFormat("Unable to send email: {0}.", e);
                throw e;
            }
        }

        //TODO: Refactor
        public static void SendReport(IEnumerable<Drive> drives)
        {
            var lastReportDate = ConfigurationHelper.LastReportDate;
            var sendReport = false;
            switch (ConfigurationHelper.ReportingFrequency)
            {
                case ReportingFrequency.Daily: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 1; break;
                case ReportingFrequency.Weekly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 7; break;
                case ReportingFrequency.Monthly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 30; break;
                case ReportingFrequency.Yearly: sendReport = (DateTime.Now - lastReportDate).TotalDays >= 365; break;
            }

            if (sendReport)
            {
                ConfigurationHelper.LastReportDate = DateTime.Now;
                var builder = CreateBuilder("DP.LSP.Tools.DiskMon.Report.html");
                builder.Replace(Constants.EmailMachinePlaceholderKey, DiskManager.MachineName);

                if (drives.Any())
                    builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, Compose(drives));
                else
                    builder.Replace(Constants.EmailOtherDrivesPlaceholderKey, "<tr class=\"tb2\"><td colspan=\"2\">None</td></tr>");

                var recipients = ConfigurationHelper.Recipients.Select(r => r.Email);
                if (recipients.Any())
                    Send(ConfigurationHelper.MailSender,
                        string.Format("Disk Status on {0}", DiskManager.MachineName),
                        builder.ToString(),
                        recipients,
                        false);
                else
                {
                    var error = "Unable to send email. No recipients specified in config.";
                    LogHelper.Logger.Error(error);
                    throw new Exception(error);
                }
            }
        }
    }
}
