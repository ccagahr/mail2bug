﻿using System;
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
