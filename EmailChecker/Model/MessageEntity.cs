using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using EmailChecker.ViewModel;

namespace EmailChecker.Model
{
    public enum MessageStatus
    {
        New,
        Comapred,
        Acknowledged,
        Alert,
        Different
    }

    public class MessageEntity : INotifyPropertyChanged
    {
        MediaPlayer player = new MediaPlayer();
        public int Number { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }

        private MessageStatus _status;

        public MessageStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public OrderEntity OrderDetails { get; set; }

        public DispatcherTimer RecordCompareTimer { get; set; }

        public bool IsCompared { get; set; }

        public MessageEntity()
        {
            try
            {
                RecordCompareTimer = new DispatcherTimer();
                int acknowledgementTime = int.Parse(EmailServerSettingViewModel.CurrentServerSettings.RecordCompareTime);
                RecordCompareTimer.Interval = new TimeSpan(0, 0, 0, 0, acknowledgementTime);
                RecordCompareTimer.Tick += RecordCompareTimer_Tick;

                Status = MessageStatus.New;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        private void RecordCompareTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // MessageBox.Show("Record Compare timer");
                Status = MessageStatus.Alert;
                if (File.Exists(EmailServerSettingViewModel.CurrentServerSettings.AlertFilePath))
                {
                    player.Open(new Uri(EmailServerSettingViewModel.CurrentServerSettings.AlertFilePath,
                        UriKind.Absolute));
                    player.MediaEnded += player_MediaEnded;
                    player.Play();
                }
                string message =
                string.Format("Account {0} opened an order. No message from the other account.",
                    OrderDetails.AccountNumber);
                MessageBox.Show(message, "Email Check", MessageBoxButton.OK);
                Status = MessageStatus.Acknowledged;
                RecordCompareTimer.Stop();
                if (File.Exists(EmailServerSettingViewModel.CurrentServerSettings.AlertFilePath))
                {
                    player.Stop();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void player_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                player.Position = TimeSpan.Zero;
                player.Play();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
