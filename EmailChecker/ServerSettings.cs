using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EmailChecker
{
    [XmlRoot("Server")]
    public class ServerSettings
    {
        [XmlElement(ElementName = "Pop3Server")]
        [DefaultValue("")]
        public string Pop3Server;

        [XmlElement(ElementName = "Pop3ServerPortNumber")]
        [DefaultValue("")]
        public string Pop3ServerPortNumber;

        [XmlElement(ElementName = "UseSSLForPop3Server")]
        [DefaultValue("")]
        public string UseSSLForPop3Server;

        [XmlElement(ElementName = "SaveCopyInServer")]
        [DefaultValue("")]
        public string SaveCopyInServer;

        [XmlElement(ElementName = "UserName")]
        [DefaultValue("")]
        public string UserName;

        [XmlElement(ElementName = "Password")]
        [DefaultValue("")]
        public string Password;

        [XmlElement(ElementName = "EnforceCheckMailFirst")]
        [DefaultValue("")]
        public string EnforceCheckMailFirst;

        [XmlElement(ElementName = "SMTPServer")]
        [DefaultValue("")]
        public string SMTPServer;

        [XmlElement(ElementName = "SMTPServerPortNumber")]
        [DefaultValue("")]
        public string SMTPServerPortNumber;

        [XmlElement(ElementName = "UseSSLForSMTPServer")]
        [DefaultValue("")]
        public string UseSSLForSMTPServer;

        [XmlElement(ElementName = "AlarmFilePath")]
        [DefaultValue("")]
        public string AlarmFilePath;

        [XmlElement(ElementName = "EmailCheckInterval")]
        [DefaultValue("")]
        public string EmailCheckInterval;

        [XmlElement(ElementName = "RecordCompareTime")]
        [DefaultValue("")]
        public string RecordCompareTime;

        [XmlElement(ElementName = "AcknowledgementWaitingTime")]
        [DefaultValue("")]
        public string AcknowledgementWaitingTime;

        [XmlElement(ElementName = "LogFilePath")]
        [DefaultValue("")]
        public string LogFilePath;

        [XmlElement(ElementName = "SendEmailWhenAlarmInitiated")]
        [DefaultValue("")]
        public string SendEmailWhenAlarmInitiated;

        [XmlElement(ElementName = "EmailAddresses")]
        [DefaultValue("")]
        public string EmailAddresses;

        [XmlElement(ElementName = "AlertFilePath")]
        [DefaultValue("")]
        public string AlertFilePath;

        public ServerSettings()
        {
            Pop3Server = string.Empty;
            Pop3ServerPortNumber = string.Empty;
            UseSSLForPop3Server = string.Empty;
            SaveCopyInServer = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            EnforceCheckMailFirst = string.Empty;
            SMTPServer = string.Empty;
            SMTPServerPortNumber = string.Empty;
            UseSSLForSMTPServer = string.Empty;
            AlarmFilePath = string.Empty;
            EmailCheckInterval = string.Empty;
            RecordCompareTime = string.Empty;
            AcknowledgementWaitingTime = string.Empty;
            LogFilePath = string.Empty;
            SendEmailWhenAlarmInitiated = string.Empty;
            EmailAddresses = string.Empty;
            AlertFilePath = string.Empty;
        }
    }
}
