using System;
using Mail2Bug;
using Mail2Bug.MessageProcessingStrategies;
using Mail2BugUnitTests.Mocks.Email;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mail2BugUnitTests
{
    [TestClass]
    public class MailHeaderExtractorUnitTest
    {
        [TestMethod]
        public void testGetHtmlHeader()
        {
            var message = getTestMail(true);
            string senderAddress = message.SenderAddress;
            
            string expectedResult = "<b>Von</b>: " + senderAddress + "<br><b>Gesendet</b>: 11.12.2018 11:30:00<br><b>An</b>: to1@test.com;to2@test.com<br><b>CC</b>: cc1@test.com;cc2@test.com<br><b>Betreff</b>: E-Mailsubject<br><hr>";

            var extractor = new MailHeaderExtractor(message);

            Assert.AreEqual(expectedResult, extractor.GetHeaderHtml(), true, "Validate the right headers where extracted");
        }

        [TestMethod]
        public void testGetHtmlHeaderNoCC()
        {
            var message = getTestMail(false);
            string senderAddress = message.SenderAddress;
           
            string expectedResult = "<b>Von</b>: " + senderAddress + "<br><b>Gesendet</b>: 11.12.2018 11:30:00<br><b>An</b>: to1@test.com;to2@test.com<br><b>Betreff</b>: E-Mailsubject<br><hr>";

            var extractor = new MailHeaderExtractor(message);

            Assert.AreEqual(expectedResult, extractor.GetHeaderHtml(), true, "Validate the right headers where extracted");
        }

        [TestMethod]
        public void testGetHtmlBody()
        {
            var config = GetConfig();

            var message = getTestMail(false);
            message.HtmlBody = "<html><head></head><body style=\"cool\">blabla</body></html>";
            string senderAddress = message.SenderAddress;

            string expectedResult = "<html><head></head><body style=\"cool\"><b>Von</b>: " + senderAddress + "<br><b>Gesendet</b>: 11.12.2018 11:30:00<br><b>An</b>: to1@test.com;to2@test.com<br><b>Betreff</b>: E-Mailsubject<br><hr>blabla</body></html>";
            
            Assert.AreEqual(expectedResult, MailHeaderExtractor.GetHtmlBody(config, message, message.HtmlBody), true, "Validate the right headers embbeded in the body");
        }

        [TestMethod]
        public void testGetHtmlBodyConfigOff()
        {
            var config = GetConfig();
            config.WorkItemSettings.AddEmailHeaderToMessageDescription = false;

            var message = getTestMail(false);
            message.HtmlBody = "<html><head></head><body style=\"cool\">blabla</body></html>";
            string senderAddress = message.SenderAddress;

            string expectedResult = message.HtmlBody;

            Assert.AreEqual(expectedResult, MailHeaderExtractor.GetHtmlBody(config, message, message.HtmlBody), true, "Validate the right headers embbeded in the body");
        }

        static IncomingEmailMessageMock getTestMail(bool withCC)
        {
            var message = IncomingEmailMessageMock.CreateWithRandomData(false);

            message.Subject = "E-Mailsubject";
            message.SentOn = DateTime.Parse("2018-12-11T11:30:00+1");
            message.ToAddresses = new string[] { "to1@test.com", "to2@test.com" };
            if (withCC)
            {
                message.CcAddresses = new string[] { "cc1@test.com", "cc2@test.com" };
            }
            else
            {
                message.CcAddresses = new string[] { };
            }

            return message;
        }

        private static Config.InstanceConfig GetConfig()
        {
            var config = new Config.InstanceConfig
            {
                WorkItemSettings = new Config.WorkItemSettings
                {
                    AddEmailHeaderToMessageDescription = true   
                }                
            };

            return config;
        }
    }
}
