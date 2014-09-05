using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EmailChecker.Model;
using OpenPop.Pop3;

namespace EmailChecker.ViewModel
{
    public class EmailServerSettingViewModel : ViewModelBase
    {
        #region Member Declaration

        private string pop3Server;
        private string pop3ServerPortNumber;
        private bool useSSLForPop3Server;
        private bool saveCopyInServer;
        private string userName;
        private string password;
        private bool enforceCheckMailFirst;
        private string smtpServer;
        private string smtpServerPortNumber;
        private bool useSSLForSMTPServer;
        private string alarmFilePath;
        private string emailCheckInterval;
        private string recordCompareTime;
        private string acknowledgementWaitingTime;
        private string logFilePath;
        private bool sendEmailWhenAlarmInitiated;
        private string emailAddresses;

        private ObservableCollection<MessageEntity> allEmails;

        private EmailHelper.EmailHelper emailHelper;

        private string lastUpdatedOn;

        private List<string> seenUids;
 
        public string LastUpdatedOn
        {
            get { return lastUpdatedOn; }
            set
            {
                lastUpdatedOn = value; 
                NotifyPropertyChangedEvent("LastUpdatedOn");
            }
        }

        public ObservableCollection<MessageEntity> AllEmails
        {
            get { return allEmails; }
            set
            {
                allEmails = value;
                NotifyPropertyChangedEvent("AllEmails");
            }
        }

        #endregion

        #region Properties

        public string Pop3Server
        {
            get { return pop3Server; }
            set
            {
                pop3Server = value;
                NotifyPropertyChangedEvent("Pop3Server");
            }
        }
        public string Pop3ServerPortNumber
        {
            get { return pop3ServerPortNumber; }
            set
            {
                pop3ServerPortNumber = value;
                NotifyPropertyChangedEvent("Pop3ServerPortNumber");
            }
        }
        public bool UseSSLForPop3Server
        {
            get { return useSSLForPop3Server; }
            set
            {
                useSSLForPop3Server = value;
                NotifyPropertyChangedEvent("UseSSLForPop3Server");
            }
        }
        public bool SaveCopyInServer
        {
            get { return saveCopyInServer; }
            set
            {
                saveCopyInServer = value;
                NotifyPropertyChangedEvent("SaveCopyInServer");
            }
        }
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                NotifyPropertyChangedEvent("UserName");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                NotifyPropertyChangedEvent("Password");
            }
        }
        public bool EnforceCheckMailFirst
        {
            get { return enforceCheckMailFirst; }
            set
            {
                enforceCheckMailFirst = value;
                NotifyPropertyChangedEvent("EnforceCheckMailFirst");
            }
        }
        public string SMTPServer
        {
            get { return smtpServer; }
            set
            {
                smtpServer = value;
                NotifyPropertyChangedEvent("SMTPServer");
            }
        }
        public string SMTPServerPortNumber
        {
            get { return smtpServerPortNumber; }
            set
            {
                smtpServerPortNumber = value;
                NotifyPropertyChangedEvent("SMTPServerPortNumber");
            }
        }
        public bool UseSSLForSMTPServer
        {
            get { return useSSLForSMTPServer; }
            set
            {
                useSSLForSMTPServer = value;
                NotifyPropertyChangedEvent("UseSSLForSMTPServer");
            }
        }
        public string AlarmFilePath
        {
            get { return alarmFilePath; }
            set
            {
                alarmFilePath = value;
                NotifyPropertyChangedEvent("AlarmFilePath");
            }
        }
        public string EmailCheckInterval
        {
            get { return emailCheckInterval; }
            set
            {
                emailCheckInterval = value;
                NewMailCheckerTimer.Interval = new TimeSpan(0, 0, 0, 0, int.Parse(emailCheckInterval));
                NotifyPropertyChangedEvent("EmailCheckInterval");
            }
        }
        public string RecordCompareTime
        {
            get { return recordCompareTime; }
            set
            {
                recordCompareTime = value;
                NotifyPropertyChangedEvent("RecordCompareTime");
            }
        }
        public string AcknowledgementWaitingTime
        {
            get { return acknowledgementWaitingTime; }
            set
            {
                acknowledgementWaitingTime = value;
                NotifyPropertyChangedEvent("AcknowledgementWaitingTime");
            }
        }
        public string LogFilePath
        {
            get { return logFilePath; }
            set
            {
                logFilePath = value;
                NotifyPropertyChangedEvent("LogFilePath");
            }
        }
        public bool SendEmailWhenAlarmInitiated
        {
            get { return sendEmailWhenAlarmInitiated; }
            set
            {
                sendEmailWhenAlarmInitiated = value;
                NotifyPropertyChangedEvent("SendEmailWhenAlarmInitiated");
            }
        }
        public string EmailAddresses
        {
            get { return emailAddresses; }
            set
            {
                emailAddresses = value;
                NotifyPropertyChangedEvent("EmailAddresses");
            }
        }

        public DispatcherTimer NewMailCheckerTimer { get; set; }

        #endregion

        #region Commands

        private ICommand saveServerSettingsCommand;

        public ICommand SaveServerSettingsCommand
        {
            get { return saveServerSettingsCommand; }
            set { saveServerSettingsCommand = value; }
        }

        #endregion
        #region Constructor
        public EmailServerSettingViewModel()
        {
            NewMailCheckerTimer = new DispatcherTimer();

            Pop3Server = Properties.Server.Default.Pop3Server;
            Pop3ServerPortNumber = Properties.Server.Default.Pop3ServerPortNumber;
            UseSSLForPop3Server = Properties.Server.Default.UseSSLForPop3Server;
            SaveCopyInServer = Properties.Server.Default.SaveCopyInServer;
            UserName = Properties.Server.Default.UserName;
            Password = Properties.Server.Default.Password;
            EnforceCheckMailFirst = Properties.Server.Default.EnforceCheckMailFirst;
            SMTPServer = Properties.Server.Default.SMTPServer;
            SMTPServerPortNumber = Properties.Server.Default.SMTPServerPortNumber;
            UseSSLForSMTPServer = Properties.Server.Default.UseSSLForSMTPServer;
            AlarmFilePath = Properties.Server.Default.AlarmFilePath;
            EmailCheckInterval = Properties.Server.Default.EmailCheckInterval;
            RecordCompareTime = Properties.Server.Default.RecordCompareTime;
            AcknowledgementWaitingTime = Properties.Server.Default.AcknowledgementWaitingTime;
            LogFilePath = Properties.Server.Default.LogFilePath;
            SendEmailWhenAlarmInitiated = Properties.Server.Default.SendEmailWhenAlarmInitiated;
            EmailAddresses = Properties.Server.Default.EmailAddresses;

            NewMailCheckerTimer.Tick += NewMailCheckerTimer_Tick;
            NewMailCheckerTimer.Interval = new TimeSpan(0, 0, 0, 0, int.Parse(emailCheckInterval));

            SaveServerSettingsCommand = new RelayCommand(new Action<object>(SaveServerSettings));

            emailHelper = new EmailHelper.EmailHelper();
            emailHelper.InitializePop3Client(pop3Server, Pop3ServerPortNumber, useSSLForPop3Server, userName, password);
            seenUids = new List<string>();
            AllEmails = new ObservableCollection<MessageEntity>();

            emailHelper.FetchUnseenMessages(seenUids, AllEmails);
            LastUpdatedOn = "Last updated on: " + DateTime.Now.ToString();

            NewMailCheckerTimer.Start();
        }


        void NewMailCheckerTimer_Tick(object sender, EventArgs e)
        {
            //CheckNewMail();
            emailHelper.FetchUnseenMessages(seenUids, AllEmails);
            LastUpdatedOn = "Last updated on: " + DateTime.Now.ToString();
        }

        private void CheckNewMail()
        {
            //try
            //{
            //    int counter = 0;
            //    for (int messageIndex = pop3Client.GetMessageCount(); messageIndex >= 1; messageIndex--)
            //    {
            //        var mailMessage = pop3Client.GetMessage(messageIndex).ToMailMessage();
            //        AllEmails.Add(new MessageEntity
            //        {
            //            Number = messageIndex,
            //            Subject = mailMessage.Subject,
            //            Body = mailMessage.Body,
            //            DateSent = pop3Client.GetMessage(messageIndex).Headers.DateSent
            //        });
            //    }
            //    LastUpdatedOn = DateTime.Now.ToString();
            //}
            //catch (Exception exception)
            //{
            //}
        }
        #endregion

        #region Methods

        public void SaveServerSettings(object input)
        {
            Properties.Server.Default.Pop3Server = Pop3Server;

            Properties.Server.Default.Pop3ServerPortNumber = Pop3ServerPortNumber;
            Properties.Server.Default.UseSSLForPop3Server = UseSSLForPop3Server;
            Properties.Server.Default.SaveCopyInServer = SaveCopyInServer;
            Properties.Server.Default.UserName = UserName;
            Properties.Server.Default.Password = Password;
            Properties.Server.Default.EnforceCheckMailFirst = EnforceCheckMailFirst;
            Properties.Server.Default.SMTPServer = SMTPServer;
            Properties.Server.Default.SMTPServerPortNumber = SMTPServerPortNumber;
            Properties.Server.Default.UseSSLForSMTPServer = UseSSLForSMTPServer;
            Properties.Server.Default.AlarmFilePath = AlarmFilePath;
            Properties.Server.Default.EmailCheckInterval = EmailCheckInterval;
            Properties.Server.Default.RecordCompareTime = RecordCompareTime;
            Properties.Server.Default.AcknowledgementWaitingTime = AcknowledgementWaitingTime;
            Properties.Server.Default.LogFilePath = LogFilePath;
            Properties.Server.Default.SendEmailWhenAlarmInitiated = SendEmailWhenAlarmInitiated;
            Properties.Server.Default.EmailAddresses = EmailAddresses;

            Properties.Server.Default.Save();

            emailHelper.InitializePop3Client(Pop3Server,Pop3ServerPortNumber, UseSSLForPop3Server, UserName, Password);
        }
        #endregion
    }
}
