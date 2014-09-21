using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
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
        private string alertFilePath;
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

        private bool showSettingDialog;

        public bool ShowSettingDialog
        {
            get { return showSettingDialog; }
            set
            {
                showSettingDialog = value;
                NotifyPropertyChangedEvent("ShowSettingDialog");
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

        public string AlertFilePath
        {
            get { return alertFilePath; }
            set
            {
                alertFilePath = value;
                NotifyPropertyChangedEvent("AlertFilePath");
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

        private ICommand cancelServerSettingsCommand;

        public ICommand CancelServerSettingsCommand
        {
            get { return cancelServerSettingsCommand; }
            set { cancelServerSettingsCommand = value; }
        }

        #endregion

        private static ServerSettings currentServerSettings;

        public static ServerSettings CurrentServerSettings
        {
            get { return currentServerSettings; }
            set { currentServerSettings = value; }
        }

        #region Constructor
        public EmailServerSettingViewModel()
        {
            try
            {
                NewMailCheckerTimer = new DispatcherTimer();

                // Reading server setting from configuration file
                ReadServerSettings();

                // Set Log file path
                LogHelper.LogFileName = currentServerSettings.LogFilePath;

                LogHelper.WriteMessage("Read settings from setting. Populating setting objects..");
                Pop3Server = currentServerSettings.Pop3Server;
                Pop3ServerPortNumber = currentServerSettings.Pop3ServerPortNumber;

                bool boolValue = false;
                if (string.IsNullOrEmpty(currentServerSettings.UseSSLForPop3Server) == false)
                    boolValue = bool.Parse(currentServerSettings.UseSSLForPop3Server);
                UseSSLForPop3Server = boolValue;

                if (string.IsNullOrEmpty(currentServerSettings.SaveCopyInServer) == false)
                    boolValue = bool.Parse(currentServerSettings.SaveCopyInServer);
                SaveCopyInServer = boolValue;

                UserName = currentServerSettings.UserName;
                Password = currentServerSettings.Password;

                if (string.IsNullOrEmpty(currentServerSettings.EnforceCheckMailFirst) == false)
                    boolValue = bool.Parse(currentServerSettings.EnforceCheckMailFirst);

                EnforceCheckMailFirst = boolValue;
                SMTPServer = currentServerSettings.SMTPServer;
                SMTPServerPortNumber = currentServerSettings.SMTPServerPortNumber;

                if (string.IsNullOrEmpty(currentServerSettings.UseSSLForSMTPServer) == false)
                    boolValue = bool.Parse(currentServerSettings.UseSSLForSMTPServer);

                UseSSLForSMTPServer = boolValue;
                AlarmFilePath = currentServerSettings.AlarmFilePath;
                AlertFilePath = currentServerSettings.AlertFilePath;
                EmailCheckInterval = currentServerSettings.EmailCheckInterval;
                RecordCompareTime = currentServerSettings.RecordCompareTime;
                AcknowledgementWaitingTime = currentServerSettings.AcknowledgementWaitingTime;
                LogFilePath = currentServerSettings.LogFilePath;

                if (string.IsNullOrEmpty(currentServerSettings.SendEmailWhenAlarmInitiated) == false)
                    boolValue = bool.Parse(currentServerSettings.SendEmailWhenAlarmInitiated);
                SendEmailWhenAlarmInitiated = boolValue;

                EmailAddresses = currentServerSettings.EmailAddresses;

                NewMailCheckerTimer.Tick += NewMailCheckerTimer_Tick;
                NewMailCheckerTimer.Interval = new TimeSpan(0, 0, 0, 0, int.Parse(emailCheckInterval));

                SaveServerSettingsCommand = new RelayCommand(new Action<object>(SaveServerSettings));
                CancelServerSettingsCommand = new RelayCommand(new Action<object>(CancelServerSettings));

                emailHelper = new EmailHelper.EmailHelper();

                emailHelper.InitializePop3Client(pop3Server, Pop3ServerPortNumber, useSSLForPop3Server, userName, password);
                seenUids = new List<string>();
                AllEmails = new ObservableCollection<MessageEntity>();

                emailHelper.FetchUnseenMessages(seenUids, AllEmails);
                LastUpdatedOn = "Last updated on: " + DateTime.Now.ToString();

                //Starting timer for checking new mail
                LogHelper.WriteMessage("Starting timer for checking new mail with interval of :" + emailCheckInterval);
                NewMailCheckerTimer.Start();
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("EmailServerSettingViewModel", exception);
                throw;
            }
        }

        private void ReadServerSettings()
        {
            try
            {
                string settingFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ServerSettings.xml";
                XmlSerializer x = new XmlSerializer(typeof(ServerSettings));
                string xmlData = File.ReadAllText(settingFilePath);
                //string xmlData = File.ReadAllText(@"C:\Users\Balram\Documents\Visual Studio 2013\Projects\EmailChecker\EmailChecker\ServerSettings.xml");
                currentServerSettings = (ServerSettings)x.Deserialize(new StringReader(xmlData));
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("ReadServerSettings", exception);
                throw;
            }
        }


        void NewMailCheckerTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //CheckNewMail();
                LogHelper.WriteMessage("Checking for new mail on server...");
                emailHelper.FetchUnseenMessages(seenUids, AllEmails);
                LastUpdatedOn = "Last updated on: " + DateTime.Now.ToString();
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("NewMailCheckerTimer_Tick", exception);
                throw;
            }
        }

        #endregion

        #region Methods

        public void SaveServerSettings(object input)
        {
            try
            {
                LogHelper.WriteMessage("Saving server settings to file..");

                bool hasServerDetailsChanged = currentServerSettings.Pop3ServerPortNumber != Pop3ServerPortNumber ||
                                               currentServerSettings.Pop3ServerPortNumber != Pop3ServerPortNumber ||
                                               currentServerSettings.UseSSLForPop3Server != UseSSLForPop3Server.ToString() ||
                                               currentServerSettings.UserName != UserName ||
                                               currentServerSettings.Password != Password;

                currentServerSettings.Pop3Server = Pop3Server;
                currentServerSettings.Pop3ServerPortNumber = Pop3ServerPortNumber;
                currentServerSettings.UseSSLForPop3Server = UseSSLForPop3Server.ToString();
                currentServerSettings.SaveCopyInServer = SaveCopyInServer.ToString();
                currentServerSettings.UserName = UserName;
                currentServerSettings.Password = Password;
                currentServerSettings.EnforceCheckMailFirst = EnforceCheckMailFirst.ToString();
                currentServerSettings.SMTPServer = SMTPServer;
                currentServerSettings.SMTPServerPortNumber = SMTPServerPortNumber;
                currentServerSettings.UseSSLForSMTPServer = UseSSLForSMTPServer.ToString();
                currentServerSettings.AlarmFilePath = AlarmFilePath;
                currentServerSettings.AlertFilePath = AlertFilePath;
                currentServerSettings.EmailCheckInterval = EmailCheckInterval;
                currentServerSettings.RecordCompareTime = RecordCompareTime;
                currentServerSettings.AcknowledgementWaitingTime = AcknowledgementWaitingTime;
                currentServerSettings.LogFilePath = LogFilePath;
                currentServerSettings.SendEmailWhenAlarmInitiated = SendEmailWhenAlarmInitiated.ToString();
                currentServerSettings.EmailAddresses = EmailAddresses;

                SaveServerSettingsToXML();
                LogHelper.WriteMessage("Settings saved.");

                if (hasServerDetailsChanged)
                {
                    LogHelper.WriteMessage("Server details has changed. Reconnecting to server...");
                    emailHelper.InitializePop3Client(Pop3Server, Pop3ServerPortNumber, UseSSLForPop3Server, UserName,
                        Password);
                }

                MessageBox.Show("Settings updated!", "Email Checker", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception exception)
            {
                LogHelper.WriteError("SaveServerSettings", exception);
                throw;
            }
        }

        public void CancelServerSettings(object input)
        {
            ShowSettingDialog = !ShowSettingDialog;
        }

        private void SaveServerSettingsToXML()
        {
            try
            {
                string settingFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ServerSettings.xml";
                LogHelper.WriteMessage("Setting file name: " + settingFilePath);
                XmlSerializer x = new XmlSerializer(currentServerSettings.GetType());
                System.IO.StreamWriter file = new System.IO.StreamWriter(settingFilePath);
                x.Serialize(file, currentServerSettings);
                file.Close();
            }
            catch (Exception exception)
            {
                LogHelper.WriteError("SaveServerSettingsToXML", exception);
                throw;
            }
        }
        #endregion
    }
}
