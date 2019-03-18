using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mail2Bug.Email;

namespace Mail2Bug.MessageProcessingStrategies
{    
    public class MailHeaderExtractor
    {
        private readonly IIncomingEmailMessage _emailMessage;

        public MailHeaderExtractor(IIncomingEmailMessage emailMessage)
        {
            _emailMessage = emailMessage;
        }

        /// <summary>
        /// Returns a string with the mail information (SenderAddress, SentOn, ToAddresses, CcAddresses, Subject, A placeholder for attachment names) in html form, to embedd it in workitems (description/history).
        /// </summary>
        /// <returns>A html formatted string containing the important mail properties.</returns>
        /// <remarks>The method is public to make it testable for unit tests.</remarks>
        public string GetHeaderHtml()
        {
            const string pattern = "<b>{0}</b>: {1}<br>";
            StringBuilder header = new StringBuilder();

            header.Append(String.Format(pattern, "Von", _emailMessage.SenderAddress));
            header.Append(String.Format(pattern, "Gesendet", _emailMessage.SentOn.ToString()));
            header.Append(String.Format(pattern, "An", string.Join(";", _emailMessage.ToAddresses)));
            if (_emailMessage.CcAddresses != null && _emailMessage.CcAddresses.Any())
            {
                header.Append(String.Format(pattern, "CC", string.Join(";", _emailMessage.CcAddresses)));
            }
            header.Append(String.Format(pattern, "Betreff", _emailMessage.Subject));
            header.Append(String.Format(pattern, "Anlagen", "<div id=\"{8BA72636-CEA9-4DA6-A653-FD9227511CE1}\"></div>"));
            header.Append("<hr>");

            return header.ToString();
        }

        public static string GetHtmlBody(Config.InstanceConfig config, IIncomingEmailMessage message, string htmlBody)
        {
            string newHtmlBody = "";
            if (config.WorkItemSettings.AddEmailHeaderToMessageDescription)
            {
                var extractor = new MailHeaderExtractor(message);
                newHtmlBody = Regex.Replace(htmlBody, @"(<body.*?>)", "$1" + extractor.GetHeaderHtml());
            }
            else
            {
                newHtmlBody = htmlBody;
            }

            return newHtmlBody;
        }
    }
}
